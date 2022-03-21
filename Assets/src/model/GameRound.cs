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

  public static T spawnRandom<T>() where T : Fragment {
    return (T)NewFragmentFrom(randomFragmentOfType(typeof(T)));
  }

  public static RegisteredFragmentAttribute randomFragmentOfType(System.Type type) {
    var frags = RegisteredFragmentAttribute.GetAllFragmentTypes(type);
    var spawn = frags[Random.Range(0, frags.Count)];
    return spawn;
  }

  // scatter random items around
  public void PlaceItems(int numWeapons, int numShields, int numEngines, int numBattery) {
    var main = GameModel.main;

    List<Fragment> utilities = new List<Fragment>();

    if (roundNumber % 3 == 0)
    {
      utilities.Add(spawnRandom<Transport>());
    }
    for(int i = 0; i < numEngines; i++) {
      utilities.Add(spawnRandom<EngineBase>());
    }
    for (int i = 0; i < numBattery; i++) {
      utilities.Add(new Battery());
    }

    // {
    //   var dagger = new Spike();
    //   dagger.builtinAngle = 0;
    //   var yOffset = 0;
    //   var x = 4;
    //   var y = main.floor.height / 2 + yOffset;
    //   var pos = new Vector2(x, y);
    //   dagger.builtinOffset = pos;
    //   main.AddFragment(dagger);
    // }

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

    for(int i = 0; i < utilities.Count; i++) {
      var fragment = utilities[i];
      var yOffset = (i - (utilities.Count - 1) / 2f) * 1.5f;
      var x = 3;
      var y = main.floor.height / 2 + yOffset;
      var pos = new Vector2(x, y);
      fragment.builtinOffset = pos;
      main.AddFragment(fragment);
    }

    for(int i = 0; i < numWeapons; i++) {
      var fragment = spawnRandom<Weapon>();
      fragment.builtinAngle = 0;
      var yOffset = (i - (numWeapons - 1) / 2f) * 1.5f;
      var x = 7;
      var y = main.floor.height / 2 + yOffset;
      var pos = new Vector2(x, y);
      fragment.builtinOffset = pos;
      main.AddFragment(fragment);
      // spawn another one
      if (fragment is Pistol) {
        fragment.builtinOffset += new Vector2(0, 0.125f);
        var f2 = new Pistol();
        f2.builtinOffset = fragment.builtinOffset + new Vector2(0, 0.25f);
        main.AddFragment(f2);
      }
    }
    for(int i = 0; i < numShields; i++) {
      var fragment = spawnRandom<Shield>();
      fragment.builtinAngle = 0;
      var yOffset = (i - (numShields - 1) / 2f) * 3f;
      var x = 11 - Mathf.Abs(yOffset * 0.33f);
      var y = main.floor.height / 2 + yOffset;
      var pos = new Vector2(x, y);
      fragment.builtinAngle = yOffset * 15;
      fragment.builtinOffset = pos;
      main.AddFragment(fragment);
    }
  }

  public static Fragment NewFragmentFrom(RegisteredFragmentAttribute info) {
    var type = info.type;
    var noArgConstructor = type.GetConstructor(new System.Type[0]);
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
      var enemyNumFragments = Util.Temporal(Mathf.Pow(roundNumber, 0.65f));
      if (enemyNumFragments < 1) {
        enemyNumFragments = 1;
      }

      if (timeUntilNextSpawn > remaining) {
        // last spawn!
        // either spawn 3 enemies, or spawn one enemy with 2x weapons and shields
        if (Random.value < 0.5f) {
          enemyNumFragments *= 2;
        } else {
          spawnsPerTime = 2;
        }
      }
      for(int i = 0; i < spawnsPerTime; i++) {
        var numWeapons = Random.Range(Mathf.CeilToInt(enemyNumFragments / 2), enemyNumFragments + 1);
        var numShields = enemyNumFragments - numWeapons;
        // shrink enemies; tighter enemies look cooler and are more focused
        var influence = Mathf.Sqrt(enemyNumFragments) * 0.75f;

        // var yOffset = (i - (spawnsPerTime - 1) / 2f) * 4f;
        // var pos = new Vector2(GameModel.main.floor.width - 4, GameModel.main.floor.height / 2f + yOffset);
        var floor = GameModel.main.floor;
        var pos = new Vector2(Random.Range(3, floor.width - 3), Random.Range(3, floor.height - 3));
        var powerScalar = spawnsPerTime > 1 ? 1.5f : 1;
        spawnEnemy(numWeapons, numShields, influence, pos, powerScalar);
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

  public void spawnEnemy(int numWeapons, int numShields, float influence, Vector2 pos, float powerScalar = 1) {
    bool isSymmetrical = Random.value < 0.5f;
    UnityEngine.Object.Instantiate(VFX.Get("enemySpawn"), pos, Quaternion.identity);
    var main = GameModel.main;
    var floor = main.floor;
    var weaponType = Random.value < 0.8f ? typeof(Gun) : Random.value < 0.5 ? typeof(MeleeWeapon) : typeof(Weapon);

    bool isCircular = true;
    bool isAntiDirected = Random.value < 0.2f;

    var enemy = new Enemy(pos, getAi(weaponType, isCircular ? 180 : isAntiDirected ? 45 : 15));
    enemy.builtinAngle = 180;
    main.AddFragment(enemy);

    var maxHP = Mathf.RoundToInt(25 + Mathf.Pow(roundNumber, 1.2f) * 5);
    var outflow = Mathf.RoundToInt(8 + Mathf.Pow(roundNumber, 1.2f) * 2f) * powerScalar;
    var avatar = new EnemyAvatar(maxHP, outflow);
    avatar.owner = enemy;
    main.AddFragment(avatar);

    var shieldType = Random.value < 0.5f ? typeof(MassyShield) : Random.value < 0.5f ? typeof(EnergyShield) : typeof(Shield);
    List<RegisteredFragmentAttribute> shieldsTypeList = GetRandomTypeList(shieldType, numShields, true);
    for(var i = 0; i < numShields; i++) {
      var shield = NewFragmentFrom(shieldsTypeList[i]);
      shield.ChangeMana(shield.manaMax);
      shield.owner = enemy;
      main.AddFragment(shield);
      var angle = (i + 0.5f) * 360f / numShields + 180;
      shield.builtinOffset = Util.fromDeg(angle) * influence / 2;
      shield.builtinAngle = angle;
      avatar.connect(shield);
    }

    List<RegisteredFragmentAttribute> weaponsTypeList = GetRandomTypeList(weaponType, numWeapons, true);

    bool bIsDirected = Random.value < 0.5f;

    for (var i = 0; i < numWeapons; i++) {
      var weapon = NewFragmentFrom(weaponsTypeList[i]);
      weapon.owner = enemy;
      weapon.ChangeMana(weapon.manaMax);

      if (isCircular) {
        var angle = (i + 0.5f) * 360f / numWeapons + 180;
        weapon.builtinOffset = Util.fromDeg(angle) * influence;
        weapon.builtinAngle = angle;
      } else {
        // scale with influence, divided by numWeapons
        var spread = Mathf.Max(0.5f, 0.5f * influence / numWeapons);
        var y = (i + 0.5f - numWeapons / 2f) * spread;
        // put weapons outside shield
        var builtinX = numShields > 0 ? influence : influence / 2;
        weapon.builtinOffset = new Vector2(builtinX, y);
        // angle them 
        if (bIsDirected || isAntiDirected) {
          var target = new Vector2(enemy.ai.desiredDistance, 0);
          var angle = -Vector2.SignedAngle((target - weapon.builtinOffset).normalized, Vector2.right);
          weapon.builtinAngle = angle;
          if (isAntiDirected) {
            weapon.builtinAngle *= -1;
          }
        }
      }

      avatar.connect(weapon);
      main.AddFragment(weapon);
    }
  }

  List<RegisteredFragmentAttribute> GetRandomTypeList(System.Type type, int num, bool isSymmetrical) {
    List<RegisteredFragmentAttribute> list = new List<RegisteredFragmentAttribute>();
    for (int i = 0; i < num; i++) {
      if (!isSymmetrical) {
        list.Add(randomFragmentOfType(type));
      } else {
        var oppositeIndex = num - 1 - i;
        if (oppositeIndex < list.Count) {
          list.Add(list[oppositeIndex]);
        } else {
          list.Add(randomFragmentOfType(type));
        }
      }
    }
    return list;
  }

  private EnemyAI getAi(System.Type type, float angleThreshold = 15) {
    return new EnemyAI() {
      baseTurnRate = 1.5f * (1 + roundNumber / 10f * 2f),
      baseSpeed = 7f + roundNumber,
      minActiveDuration = 2 + roundNumber * 0.3f,
      cooldown = 2.5f - roundNumber * 0.1f,
      deltaAngleThreshold = angleThreshold,
      desiredDistance = type == typeof(Gun) ? Random.Range(4, 10) : type == typeof(MeleeWeapon) ? 1 : 5,
      minDistance = 8,
      encumbrance = 2f + 1 * roundNumber,
    };
  }
}