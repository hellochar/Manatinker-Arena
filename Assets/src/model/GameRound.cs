using System.Collections.Generic;
using System.Linq;
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

  public GameRound(int roundNumber, int spawnsPerTime = 1) {
    this.roundNumber = roundNumber;
    int numEnemiesToSpawn = 3 + roundNumber / 2;
    duration = 42 + roundNumber * 3;
    // duration + 1 will force the last spawn to not naturally happen
    // so that the stronger end-of-round spawn happens
    var timeBetweenSpawns = (duration + 1) / numEnemiesToSpawn;
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
  public void PlaceItems(int numWeapons, int numShields) {
    var main = GameModel.main;

    List<Fragment> utilities = new List<Fragment>();

    if (roundNumber == 0) {
      utilities.Add(spawnRandom<EngineBase>());
    }

    if (roundNumber % 3 == 0) {
      // at least 1
      for (int i = 0; i < Mathf.Max(1, roundNumber / 3); i++) {
        utilities.Add(spawnRandom<Utility>());
      }
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
      var x = main.floor.width / 2 - 4;
      var y = main.floor.height / 2 + yOffset;
      var pos = new Vector2(x, y);
      fragment.builtinOffset = pos;
      main.AddFragment(fragment);
    }

    for(int i = 0; i < numWeapons; i++) {
      var fragment = spawnRandom<Weapon>();
      fragment.builtinAngle = 0;
      var yOffset = (i - (numWeapons - 1) / 2f) * 1.5f;
      var x = main.floor.width / 2;
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
      var x = main.floor.width / 2 + 4 - Mathf.Abs(yOffset * 0.33f);
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
    #if UNITY_EDITOR
    if (Input.GetKey(KeyCode.Period)) {
      dt *= 5;
    }
    #endif
    timeUntilNextSpawn -= dt;
    if (timeUntilNextSpawn < 0) {
      timeUntilNextSpawn += timeBetweenSpawns;
      roundSpawn();
    }
    if ((dt + elapsed) > duration) {
      // do one last spawn!
      if (state == GameRoundState.Active) {
        roundSpawn(true);
      }
      state = GameRoundState.WaitingForClear;
      UpdateWaitingForArenaCleared();
    }
  }

  private void roundSpawn(bool isLastSpawn = false) {
    float fragmentPow = roundNumber;

    bool isStronger = false;
    if (isLastSpawn) {
      // last spawn!
      // either spawn 3 enemies, or spawn one enemy with 2x weapons and shields
      if (Random.value < 0.5f) {
        isStronger = true;
        fragmentPow *= 2;
      } else {
        spawnsPerTime = 3;
        fragmentPow *= 0.8f;
      }
    }

    var enemyNumFragments = Util.Temporal(Mathf.Pow(fragmentPow, 0.65f));
    if (enemyNumFragments < 1) {
      enemyNumFragments = 1;
    }
    for (int i = 0; i < spawnsPerTime; i++) {
      var numWeapons = Random.Range(Mathf.CeilToInt(enemyNumFragments / 2f), enemyNumFragments + 1);
      var numShields = enemyNumFragments - numWeapons;
      // shrink enemies; tighter enemies look cooler and are more focused
      var influence = 0.5f + Mathf.Sqrt(enemyNumFragments) * 0.75f;

      // var yOffset = (i - (spawnsPerTime - 1) / 2f) * 4f;
      // var pos = new Vector2(GameModel.main.floor.width - 4, GameModel.main.floor.height / 2f + yOffset);
      var floor = GameModel.main.floor;
      var pos = new Vector2(Random.Range(3, floor.width - 3), Random.Range(3, floor.height - 3));
      var powerScalar = isStronger ? 1.5f : 1;
      spawnEnemy(numWeapons, numShields, influence, pos, powerScalar);
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
      PlaceItems(3, 3);
    } else {
      if (roundNumber % 3 == 0) {
        var numToPlace = roundNumber / 3;
        PlaceItems(numToPlace, numToPlace);
      }
    }
  }

  public void spawnEnemy(int numWeapons, int numShields, float influence, Vector2 pos, float powerScalar = 1) {
    UnityEngine.Object.Instantiate(VFX.Get("enemySpawn"), pos, Quaternion.identity);
    var main = GameModel.main;
    var floor = main.floor;

    bool isSymmetrical = Random.value < 0.5f;
    bool isCircular = numWeapons > 5 ? Random.value < 0.1 : false;
    bool isAntiDirected = Random.value < 0.2f;
    var weaponType = Random.value < 0.8f ? typeof(Gun) : Random.value < 0.5 ? typeof(MeleeWeapon) : typeof(Weapon);

    var enemy = new Enemy(pos, getAi(weaponType, isCircular ? 180 : isAntiDirected ? 45 : 15, influence));
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

    var hasBlobber = weaponsTypeList.Any(attr => attr.type == typeof(Blobber));
    for (var i = 0; i < numWeapons; i++) {
      var weapon = NewFragmentFrom(weaponsTypeList[i]);
      weapon.owner = enemy;
      weapon.ChangeMana(weapon.manaMax / 2);

      if (isCircular) {
        // numWeapons 1 - angle is 180 + 180 = 360
        // numWeapons 2 - angle is 90, 270 = 90, 270 (rotation = 90)
        // numweapons 3 - angle is 60, 180, 270 + 180, rotation = 
        // we use integer division here
        var angle = (float)(i - numWeapons / 2) / numWeapons * 360f;
        weapon.builtinOffset = Util.fromDeg(angle) * influence;
        weapon.builtinAngle = angle;
      } else {
        // scale with influence, divided by numWeapons
        var spread = Mathf.Max(0.5f, 0.5f * influence / numWeapons);
        if (hasBlobber) {
          spread *= 2;
        }
        if (weapon is Sawblade) {
          spread = Mathf.Max(2, spread);
        }
        var y = (i + 0.5f - numWeapons / 2f) * spread;

        // put weapons outside shield
        var builtinX = numShields > 0 ? influence : influence / 2;

        if (weapon is Sawblade) {
          builtinX = Mathf.Max(2, builtinX);
        }
        weapon.builtinOffset = new Vector2(builtinX, y);
        // angle them 
        if (bIsDirected || isAntiDirected) {
          var target = new Vector2(enemy.ai.desiredDistance, 0);
          var angle = -Vector2.SignedAngle((target - weapon.builtinOffset).normalized, Vector2.right);
          weapon.builtinAngle = angle;
          if (isAntiDirected) {
            weapon.builtinAngle *= -1;
          }
          // fix weird issues where they're facing away from the player
          if (Mathf.Abs(weapon.builtinAngle) > 90) {
            weapon.builtinAngle += 180;
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

  private EnemyAI getAi(System.Type type, float angleThreshold, float influence) {
    return new EnemyAI() {
      baseTurnRate = 1.5f * (1 + roundNumber / 10f * 2f),
      baseSpeed = 7f + roundNumber,
      minActiveDuration = 2 + roundNumber * 0.3f,
      cooldown = 2.5f - roundNumber * 0.1f,
      deltaAngleThreshold = angleThreshold,
      desiredDistance = type == typeof(Gun) ? Random.Range(4, 10) : type == typeof(MeleeWeapon) ? influence + 1 : 5,
      minDistance = 8,
      encumbrance = 2f + 1 * roundNumber,
    };
  }
}