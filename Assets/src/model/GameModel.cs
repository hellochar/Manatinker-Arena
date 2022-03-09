using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameModel {
  public static GameModel main;

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

  public Fragment player;
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
      if (f is PlayerFragment) {
        if (player != null) {
          throw new Exception("two players");
        }
        player = f;
      }
      circuit.AddFragment(f);
      OnFragmentAdded?.Invoke(f);
    }
  }

  public void RemoveFragment(Fragment f) {
    circuit.RemoveFragment(f);
    OnFragmentRemoved?.Invoke(f);
  }

  public static void SetMainToNewGame() {
    main = new GameModel();
    main.circuit = new Circuit();

    var player = new PlayerFragment();

    var core = new Core();
    core.owner = player;

    var pistol1 = new Pistol();
    pistol1.owner = player;
    pistol1.builtinOffset = new Vector2(1.5f, 0);
    core.connect(pistol1);

    var pistol2 = new Pistol();
    pistol2.owner = player;
    pistol2.builtinOffset = new Vector2(1.5f, 0.5f);
    core.connect(pistol2);

    var pistol3 = new Pistol();
    pistol3.owner = player;
    pistol3.builtinOffset = new Vector2(1.5f, -0.5f);
    core.connect(pistol3);

    main.AddFragment(player, core, pistol1, pistol2, pistol3);
    main.floor = new Floor(25, 25).surroundWithWalls();
  }

  public void simulate(float dt) {
    circuit.simulate(dt);
  }
}

internal class PlayerFragment : Fragment {
  public PlayerFragment() : base("player-fragment") {
  }
}