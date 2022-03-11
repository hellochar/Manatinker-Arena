using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameModel {
  public static GameModel main;

  // in seconds
  public float time = 0;
  public int round;
  private bool _isEditMode = false;
  public bool isEditMode {
    get => _isEditMode;
    set {
      if (value) {
        // immediately set editmode to true
        _isEditMode = true;
      }
      var isFalse = value == false;
      GameModelController.main.UpdateIsEditMode(value, () => {
        if (isFalse) {
          _isEditMode = false;
        }
      });
    }
  }

  public Player player;
  public Circuit circuit = new Circuit();
  public Floor floor;

  public Action<Fragment> OnFragmentRemoved;
  public Action<Fragment> OnFragmentAdded;
  public Action<Wire> OnWireAdded;
  public Action<Wire> OnWireRemoved;

  public IEnumerable<Fragment> Fragments => circuit.Fragments;

  public IEnumerable<Wire> Wires => Fragments.SelectMany(f => f.wires);

  public GameModel() {
  }

  public void AddFragment(params Fragment[] fArr) {
    foreach (var f in fArr) {
      if (f is Player p) {
        if (player != null) {
          throw new Exception("two players");
        }
        player = p;
      }
      circuit.AddFragment(f);
      OnFragmentAdded?.Invoke(f);
    }
  }

  public void RemoveFragment(Fragment f) {
    if (!circuit.HasFragment(f)) {
      Debug.LogWarning("ignoring double remove on " + f, f.controller);
    }
    circuit.RemoveFragment(f);
    OnFragmentRemoved?.Invoke(f);
  }

  public static void SetMainToNewGame() {
    main = new GameModel();
    main.circuit = new Circuit();

    main.floor = new Floor(25, 25).surroundWithWalls();

    var player = new Player(new Vector2(5, 5));

    var engine = new Engine();
    engine.owner = player;

    var battery = new Battery();
    battery.owner = player;
    battery.builtinOffset = new Vector2(0.25f, 1.5f);
    engine.connect(battery);

    var pistol1 = new Pistol();
    pistol1.owner = player;
    pistol1.builtinOffset = new Vector2(1.5f, 0);
    battery.connect(pistol1);

    // var pistol2 = new Pistol();
    // pistol2.owner = player;
    // pistol2.builtinOffset = new Vector2(1.5f, 0.5f);
    // core.connect(pistol2);

    // var pistol3 = new Pistol();
    // pistol3.owner = player;
    // pistol3.builtinOffset = new Vector2(1.5f, -0.5f);
    // core.connect(pistol3);

    main.AddFragment(player, engine, battery, pistol1);//, pistol2, pistol3);

    // scatter random items around
    var fragments = new List<Fragment>() { new Pistol(), new Pistol(), new Engine(), new Battery() };
    for(var i = 0; i < fragments.Count(); i++) {
      var newFragment = fragments[i];
      newFragment.builtinAngle = Random.Range(0, 360);
      var pos = new Vector2(Random.Range(2, main.floor.width - 2), Random.Range(2, main.floor.height - 2));
      newFragment.builtinOffset = pos;
      main.AddFragment(newFragment);
    }
  }

  void spawnEnemy() {
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

  float timeUntilNextSpawn = 1;

  public void simulate(float dt) {
    timeUntilNextSpawn -= dt;
    if (timeUntilNextSpawn < 0) {
      timeUntilNextSpawn += 15;
      spawnEnemy();
    }
    circuit.simulate(dt);
    time += dt;
  }
}
