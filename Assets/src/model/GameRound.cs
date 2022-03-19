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
  private int spawnsPerTime;
  public int roundNumber;

  public GameRound(int roundNumber, float duration = 62, int spawnsPerTime = 1) {
    this.roundNumber = roundNumber;
    int numEnemiesToSpawn = 3 + roundNumber / 2;
    var timeBetweenSpawns = (duration - 2) / numEnemiesToSpawn;
    this.duration = duration;
    this.spawnsPerTime = spawnsPerTime;
    timeStarted = GameModel.main.time;
  }

  private static List<RegisteredFragmentAttribute> allWeapons;
  private static List<RegisteredFragmentAttribute> allShields;

  public static T spawnRandom<T>() where T : Fragment {
    var frags = RegisteredFragmentAttribute.GetAllFragmentTypes<T>();
    var spawn = frags[Random.Range(0, frags.Count)];
    return (T)NewFragmentFrom(spawn);
  }

  // scatter random items around
  public void PlaceItems(int numWeapons, int numShields, int numEngines, int numBattery) {
    var main = GameModel.main;

    if (roundNumber % 3 == 0)
    {
      var fragment = spawnRandom<Transport>();
      // var fragment = new Jet();
      fragment.builtinOffset = new Vector2(2, main.floor.height / 2 - 1);
      main.AddFragment(fragment);
    }

    {
      var dagger = new Spike();
      dagger.builtinAngle = 0;
      var yOffset = 0;
      var x = 4;
      var y = main.floor.height / 2 + yOffset;
      var pos = new Vector2(x, y);
      dagger.builtinOffset = pos;
      main.AddFragment(dagger);
    }

    // {
    //   var rapier = new Rapier();
    //   rapier.builtinAngle = 0;
    //   var yOffset = 1;
    //   var x = 4;
    //   var y = main.floor.height / 2 + yOffset;
    //   var pos = new Vector2(x, y);
    //   rapier.builtinOffset = pos;
    //   main.AddFragment(rapier);
    // }

    for(int i = 0; i < numWeapons; i++) {
      var fragment = spawnRandom<Weapon>();
      fragment.builtinAngle = 0;
      var yOffset = (i - (numWeapons - 1) / 2f) * 1.5f;
      var x = 7;
      var y = main.floor.height / 2 + yOffset;
      var pos = new Vector2(x, y);
      fragment.builtinOffset = pos;
      main.AddFragment(fragment);
    }
    for(int i = 0; i < numShields; i++) {
      var fragment = spawnRandom<Shield>();
      fragment.builtinAngle = 0;
      var yOffset = (i - (numShields - 1) / 2f) * 3f;
      var x = 11;
      var y = main.floor.height / 2 + yOffset;
      var pos = new Vector2(x, y);
      fragment.builtinOffset = pos;
      main.AddFragment(fragment);
    }
    for(int i = 0; i < numEngines; i++) {
      var fragment = spawnRandom<EngineBase>();
      fragment.builtinAngle = 0;
      var yOffset = (i - (numEngines - 1) / 2f) * 3f;
      var x = 15;
      var y = main.floor.height / 2 + yOffset;
      var pos = new Vector2(x, y);
      fragment.builtinOffset = pos;
      main.AddFragment(fragment);
    }

    if (numBattery > 0) {
      var fragment = new Battery();
      fragment.builtinOffset = new Vector2(15, main.floor.height / 2 - 1);
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
    // fastmode
    if (Input.GetKey(KeyCode.Period)) {
      dt *= 5;
    }
    timeUntilNextSpawn -= dt;
    if (timeUntilNextSpawn < 0) {
      timeUntilNextSpawn += timeBetweenSpawns;
      var enemyPower = Util.Temporal(0.2f + roundNumber * 0.8f);

      if (timeUntilNextSpawn > remaining) {
        // last spawn!
        // either spawn 3 enemies, or spawn one enemy with 2x weapons and shields
        if (Random.value < 0.5f) {
          enemyPower *= 2;
        } else {
          spawnsPerTime = 2;
        }
      }
      for(int i = 0; i < spawnsPerTime; i++) {
        var numWeapons = Random.Range(1, enemyPower + 1);
        var numShields = enemyPower - numWeapons;

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
    if (roundNumber == 0) {
      PlaceItems(3, 3, 1, 1);
    } else {
      if (roundNumber % 3 == 0) {
        var numToPlace = roundNumber / 3;
        PlaceItems(numToPlace, numToPlace, 1, 1);
      }
    }
  }

  public void spawnEnemy(int numWeapons, int numShields, Vector2 pos) {
    UnityEngine.Object.Instantiate(VFX.Get("enemySpawn"), pos, Quaternion.identity);
    var main = GameModel.main;
    var floor = main.floor;
    var weaponType = Random.value < 0.8f ? WeaponType.Guns : Random.value < 0.5 ? WeaponType.Melee : WeaponType.Mixed;

    var enemy = new Enemy(pos, getAi(weaponType));
    enemy.builtinAngle = 180;
    main.AddFragment(enemy);

    var maxHP = Mathf.RoundToInt(25 + Mathf.Pow(roundNumber, 1.2f) * 5);
    var outflow = Mathf.RoundToInt(8 + Mathf.Pow(roundNumber, 1.2f) * 1.4f);
    var avatar = new EnemyAvatar(maxHP, outflow);
    avatar.owner = enemy;
    main.AddFragment(avatar);

    for(var i = 0; i < numShields; i++) {
      var shield = spawnRandom<Shield>();
      shield.ChangeMana(shield.manaMax);
      shield.owner = enemy;
      main.AddFragment(shield);
      var angle = (i + 0.5f) * 360f / numShields + 180;
      shield.builtinOffset = Util.fromDeg(angle);
      shield.builtinAngle = angle;
      avatar.connect(shield);
    }

    for (var i = 0; i < numWeapons; i++) {
      var weapon = weaponType == WeaponType.Guns ? spawnRandom<Gun>() : weaponType == WeaponType.Melee ? spawnRandom<MeleeWeapon>() : spawnRandom<Weapon>();
      weapon.owner = enemy;
      weapon.ChangeMana(weapon.manaMax);
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

  private EnemyAI getAi(WeaponType weaponType) {
    return new EnemyAI() {
      baseTurnRate = 1.5f * (1 + roundNumber / 10f * 2f),
      baseSpeed = 7f + roundNumber,
      minActiveDuration = 2 + roundNumber * 0.3f,
      cooldown = 2.5f - roundNumber * 0.1f,
      deltaAngleThreshold = 15,
      desiredDistance = weaponType == WeaponType.Guns ? Random.Range(5, 8) : weaponType == WeaponType.Melee ? 1 : 5,
      minDistance = 8,
      encumbrance = 2f + 1 * roundNumber,
    };
  }
}

enum WeaponType { Melee, Guns, Mixed };