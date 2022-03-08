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

  public void AddFragment(Fragment f) {
    circuit.AddFragment(f);
    OnFragmentAdded?.Invoke(f);
  }

  public void RemoveFragment(Fragment f) {
    circuit.RemoveFragment(f);
    OnFragmentRemoved?.Invoke(f);
  }

  public static void SetMainToNewGame() {
    main = new GameModel();
    main.circuit = new Circuit();
    main.AddFragment(new PlayerFragment());
    main.floor = new Floor(25, 25).surroundWithWalls();
  }

  public void simulate(float dt) {
    circuit.simulate(dt);
  }
}

internal class PlayerFragment : Fragment {
}