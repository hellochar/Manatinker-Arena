using System;
using System.Collections.Generic;

public class GameModel {
  public static GameModel main;

  public bool isEditMode = false;
  public Circuit circuit = new Circuit();
  public Floor floor;

  public Action<Fragment> OnFragmentRemoved;
  public Action<Fragment> OnFragmentAdded;

  public IEnumerable<Fragment> Fragments => circuit.Fragments;

  public GameModel() {
  }

  public void AddFragment(params Fragment[] fArr) {
    foreach (var f in fArr) {
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
    pistol1.builtinOffset.Set(1.5f, 0);
    core.connect(pistol1);

    var pistol2 = new Pistol();
    pistol2.owner = player;
    pistol2.builtinOffset.Set(1.5f, 0.5f);
    core.connect(pistol2);

    var pistol3 = new Pistol();
    pistol3.owner = player;
    pistol3.builtinOffset.Set(1.5f, -0.5f);
    core.connect(pistol3);

    main.AddFragment(player, core, pistol1, pistol2, pistol3);
    main.floor = new Floor(25, 25).surroundWithWalls();
  }

  public void simulate(float dt) {
    circuit.simulate(dt);
  }
}

internal class PlayerFragment : Fragment {
}