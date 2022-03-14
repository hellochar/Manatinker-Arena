using System.Collections.Generic;
using System.Threading.Tasks;
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

  public float timeUntilNextSpawn = 1.5f;
  public float timeBetweenSpawns = 14;
  public readonly float duration;
  private readonly int spawnsPerTime;
  public int roundNumber;

  public GameRound(int roundNumber, float duration = 62, float timeBetweenSpawns = 14, int spawnsPerTime = 1) {
    this.roundNumber = roundNumber;
    this.duration = duration;
    this.spawnsPerTime = spawnsPerTime;
    timeStarted = GameModel.main.time;
  }

  private static List<RegisteredFragmentAttribute> allWeapons;
  private static List<RegisteredFragmentAttribute> allShields;
  public static Weapon randomWeapon() {
    if (allWeapons == null) {
      allWeapons = RegisteredFragmentAttribute.GetAllFragmentTypes<Weapon>();
    }
    var spawn = allWeapons[Random.Range(0, allWeapons.Count)];
    return (Weapon)NewFragmentFrom(spawn);
  }

  public static Shield randomShield() {
    if (allShields == null) {
      allShields = RegisteredFragmentAttribute.GetAllFragmentTypes<Shield>();
    }
    var spawn = allShields[Random.Range(0, allShields.Count)];
    return (Shield)NewFragmentFrom(spawn);
  }

  // scatter random items around
  public static void PlaceItems(int numWeapons, int numShields) {
    var main = GameModel.main;
    for(int i = 0; i < numWeapons; i++) {
      var fragment = randomWeapon();
      fragment.builtinAngle = 0;
      var yOffset = (i - (numWeapons - 1) / 2f) * 1.5f;
      var x = 7;
      var y = main.floor.height / 2 + yOffset;
      var pos = new Vector2(x, y);
      fragment.builtinOffset = pos;
      main.AddFragment(fragment);
    }
    for(int i = 0; i < numShields; i++) {
      var fragment = randomShield();
      fragment.builtinAngle = 0;
      var yOffset = (i - (numShields - 1) / 2f) * 3f;
      var x = 11;
      var y = main.floor.height / 2 + yOffset;
      var pos = new Vector2(x, y);
      fragment.builtinOffset = pos;
      main.AddFragment(fragment);
    }
  }

  private static Fragment NewFragmentFrom(RegisteredFragmentAttribute info) {
    var typeToScatter = info.type;
    var noArgConstructor = typeToScatter.GetConstructor(new System.Type[0]);
    var newFragment = (Fragment)noArgConstructor.Invoke(new object[0]);
    return newFragment;
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
      for(int i = 0; i < spawnsPerTime; i++) {
        var numWeapons = Random.Range(1, roundNumber + 1);
        var numShields = roundNumber - numWeapons;

        // var yOffset = (i - (spawnsPerTime - 1) / 2f) * 4f;
        // var pos = new Vector2(GameModel.main.floor.width - 4, GameModel.main.floor.height / 2f + yOffset);
        var floor = GameModel.main.floor;
        var pos = new Vector2(Random.Range(3, floor.width - 3), Random.Range(3, floor.height - 3));
        spawnEnemy(numWeapons, numShields, pos);
      }
    }
    if ((dt + elapsed) > duration) {
      state = GameRoundState.WaitingForClear;
      UpdateWaitingForArenaCleared();
    }
  }

  private void UpdateWaitingForArenaCleared() {
    if (GameModel.main.enemies.Count == 0) {
      RoundUIController.main.RoundFinished();
      GoToPreparing();
    }
  }

  internal void GoToPreparing() {
    state = GameRoundState.Preparing;
    PlaceItems(3, 3);
  }

  public async void spawnEnemy(int numWeapons, int numShields, Vector2 pos) {
    UnityEngine.Object.Instantiate(VFX.Get("enemySpawn"), pos, Quaternion.identity);
    await Task.Delay(200);
    var main = GameModel.main;
    var floor = main.floor;
    var enemy = new Enemy(pos, getAi());
    enemy.builtinAngle = 180;
    main.AddFragment(enemy);

    var avatar = new EnemyAvatar(25 + roundNumber * 5);
    avatar.owner = enemy;
    main.AddFragment(avatar);

    for(var i = 0; i < numShields; i++) {
      var shield = randomShield();
      shield.owner = enemy;
      main.AddFragment(shield);
      var angle = (i + 0.5f) * 360f / numShields;
      shield.builtinOffset = Util.fromDeg(angle);
      shield.builtinAngle = angle;
    }

    for (var i = 0; i < numWeapons; i++) {
      var weapon = randomWeapon();
      weapon.owner = enemy;
      // 0 1 = 0
      // 0 2 = 
      var y = (i + 0.5f - numWeapons / 2f) * 0.5f;
      // put weapons outside shield
      var builtinX = numShields > 0 ? 2 : 1;
      weapon.builtinOffset = new Vector2(builtinX, y);
      avatar.connect(weapon);
      main.AddFragment(weapon);
    }
  }

  private EnemyAI getAi() {
    return new EnemyAI() {
      baseTurnRate = 2.5f * (1 + roundNumber / 10f),
      baseSpeed = 10f + roundNumber,
      minActiveDuration = 2,
      cooldown = 2 - roundNumber * 0.1f,
      deltaAngleThreshold = 15,
      desiredDistance = 5,
      minDistance = 8,
    };
  }
}