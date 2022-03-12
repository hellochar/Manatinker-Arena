using System.Collections.Generic;
using UnityEngine;

public enum GameRoundState {
  Active,
  WaitingForClear,
  Preparing,
};

public class GameRound {
  public GameRoundState state = GameRoundState.Active;
  // only meaningful in Spawning and WaitingForClear
  public float elapsed => GameModel.main.time - timeStarted;
  public float remaining => duration - elapsed;
  float timeStarted;

  public float timeUntilNextSpawn = 3;
  public float timeBetweenSpawns = 15;
  public readonly float duration;
  public int roundNumber;

  public GameRound(int roundNumber, float duration = 60, float timeBetweenSpawns = 15) {
    this.roundNumber = roundNumber;
    this.duration = duration;
    timeStarted = GameModel.main.time;
  }

  // scatter random items around
  public static void ScatterItems() {
    var main = GameModel.main;
    var spawnInfos = RegisteredFragmentAttribute.GetAllFragmentTypes();
    // var fragments = new List<Fragment>() { new Pistol(), new Pistol(), new Engine(), new Battery() };
    var numToScatter = 15;
    for(var i = 0; i < numToScatter; i++) {
      var info = spawnInfos[Random.Range(0, spawnInfos.Count)];
      var typeToScatter = info.type;
      var noArgConstructor = typeToScatter.GetConstructor(new System.Type[0]);
      var newFragment = (Fragment) noArgConstructor.Invoke(new object[0]);
      newFragment.builtinAngle = Random.Range(0, 360);
      var pos = new Vector2(Random.Range(2, main.floor.width - 2), Random.Range(2, main.floor.height - 2));
      newFragment.builtinOffset = pos;
      main.AddFragment(newFragment);
    }
  }

  public void Update(float dt) {
    if (state == GameRoundState.WaitingForClear) {
      UpdateWaitingForArenaCleared();
    } else if (state == GameRoundState.Active) {
      UpdateActive(dt);
    }
  }

  private void UpdateActive(float dt) {
    timeUntilNextSpawn -= dt;
    if (timeUntilNextSpawn < 0) {
      timeUntilNextSpawn += timeBetweenSpawns;
      spawnEnemy();
    }
    if ((dt + elapsed) > duration) {
      state = GameRoundState.WaitingForClear;
      UpdateWaitingForArenaCleared();
    }
  }

  private void UpdateWaitingForArenaCleared() {
    if (GameModel.main.enemies.Count == 0) {
      GoToPreparing();
    }
  }

  internal void GoToPreparing() {
    state = GameRoundState.Preparing;
    ScatterItems();
  }

  public void spawnEnemy() {
    var main = GameModel.main;
    var floor = main.floor;
    var pos = new Vector2(Random.Range(2, floor.width - 2), Random.Range(2, floor.height - 2));
    var enemy = new Enemy(pos);
    enemy.builtinAngle = Random.Range(0, 360f);
    main.AddFragment(enemy);

    var engine = new Engine();
    engine.owner = enemy;
    main.AddFragment(engine);

    var pistol1 = new Pistol();
    pistol1.owner = enemy;
    pistol1.builtinOffset = new Vector2(1.5f, 0);
    engine.connect(pistol1);
    main.AddFragment(pistol1);
  }
}