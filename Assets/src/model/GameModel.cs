using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameModel {
  public static GameModel main;

  // in seconds
  public float time = 0;
  public GameRound currentRound;

  internal void GoNextRound() {
    if (currentRound.state == GameRoundState.Preparing) {
      currentRound = new GameRound(currentRound.roundNumber++);
    }
  }

  public Player player;
  public Circuit circuit = new Circuit();
  public Floor floor;

  public Action<Fragment> OnFragmentRemoved;
  public Action<Fragment> OnFragmentAdded;
  public Action<Wire> OnWireAdded;
  public Action<Wire> OnWireRemoved;
  internal List<Enemy> enemies = new List<Enemy>();

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
      if (f is Enemy e) {
        enemies.Add(e);
      }
      circuit.AddFragment(f);
      OnFragmentAdded?.Invoke(f);
    }
  }

  public void RemoveFragment(Fragment f) {
    if (!circuit.HasFragment(f)) {
      Debug.LogWarning("ignoring double remove on " + f, f.controller);
    }
    if (f is Enemy e) {
      enemies.Remove(e);
    }
    f.disconnectAll();
    circuit.RemoveFragment(f);
    OnFragmentRemoved?.Invoke(f);
  }

  public static void SetMainToNewGame() {
    main = new GameModel();
    main.circuit = new Circuit();

    main.floor = new Floor(25, 25).surroundWithWalls();
    main.currentRound = new GameRound(0);
    main.currentRound.GoToPreparing();

    var player = new Player(new Vector2(5, 5));

    var engine = new Engine();
    engine.owner = player;

    var battery = new Battery();
    battery.owner = player;
    battery.builtinOffset = new Vector2(0.25f, 1.5f);
    engine.connect(battery);

    var gun1 = new Pistol();
    gun1.owner = player;
    gun1.builtinOffset = new Vector2(1.5f, 0);
    battery.connect(gun1);

    // var pistol2 = new Pistol();
    // pistol2.owner = player;
    // pistol2.builtinOffset = new Vector2(1.5f, 0.5f);
    // core.connect(pistol2);

    // var pistol3 = new Pistol();
    // pistol3.owner = player;
    // pistol3.builtinOffset = new Vector2(1.5f, -0.5f);
    // core.connect(pistol3);

    main.AddFragment(player, engine, battery, gun1);//, pistol2, pistol3);

  }

  public void simulate(float dt) {
    if (currentRound != null) {
      currentRound.Update(dt);
    }
    circuit.simulate(dt);
    time += dt;
  }
}
