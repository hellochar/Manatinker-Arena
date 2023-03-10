using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameModel {
  public static GameModel main;

  public bool playerHasWon = false;
  // in seconds
  public float time = 0;
  public float dt = 0;
  public GameRound currentRound;

  internal void GoNextRound() {
    if (currentRound.state == GameRoundState.Preparing) {
      currentRound = new GameRound(currentRound.roundNumber + 1);
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

    main.floor = new Floor(39 + 2, 24 + 2).surroundWithWalls();
    for (int x = 8; x < main.floor.width; x += 8) {
      var yMin = 13 - 6;
      var yMax = 13 + 6;
      main.floor.tiles[x, yMin] = TileType.WALL;
      main.floor.tiles[x, yMax] = TileType.WALL;
    }
    main.currentRound = new GameRound(0);
    main.currentRound.GoToPreparing();

    var player = new Player(new Vector2(2, main.floor.height / 2));

    var avatar = new PlayerAvatar();
    avatar.owner = player;

    main.AddFragment(player, avatar);//, pistol2, pistol3);
  }

  public void simulate(float dt) {
    this.dt = dt;
    if (currentRound != null) {
      currentRound.Update(dt);
    }
    circuit.simulate(dt);
    time += dt;
  }
}
