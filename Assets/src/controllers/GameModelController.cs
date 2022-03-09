using System;
using System.Collections.Generic;
using UnityEngine;

public class GameModelController : MonoBehaviour {
  public GameModel model => GameModel.main;

  public Transform wireContainer;
  public GameObject floorPrefab;
  public GameObject wirePrefab;
  [ReadOnly]
  public FloorController floor;
  [SerializeField]
  public FragmentPrefabMapping[] Mapping;

  private Dictionary<System.Type, GameObject> fragmentPrefabs = new Dictionary<Type, GameObject>();

  [ReadOnly]
  public List<FragmentController> fragmentControllers = new List<FragmentController>();
  public HashSet<WireController> wireControllers = new HashSet<WireController>();

  void Awake() {
    foreach(var entry in Mapping) {
      var type = Type.GetType(entry.className);
      fragmentPrefabs.Add(type, entry.prefab);
    }
    GameModel.SetMainToNewGame();
  }

  void Start() {
    Init(GameModel.main);
  }

  void Init(GameModel model) {
    floor = Instantiate(floorPrefab, Vector3.zero, Quaternion.identity).GetComponent<FloorController>();
    floor.Init(model.floor);

    foreach(var f in model.Fragments) {
      HandleFragmentAdded(f);
    }

    foreach(var w in model.Wires) {
      HandleWireAdded(w);
    }

    model.OnFragmentAdded += HandleFragmentAdded;
    model.OnFragmentRemoved += HandleFragmentRemoved;

    model.OnWireAdded += HandleWireAdded;
    model.OnWireRemoved += HandleWireRemoved;
  }

  private void HandleWireRemoved(Wire w) {
    wireControllers.Remove(w.controller);
    w.controller.Removed();
    // wireControllers.
  }

  private void HandleWireAdded(Wire w) {
    var wire = Instantiate(wirePrefab, Vector3.zero, Quaternion.identity, wireContainer);
    var wireController = wire.GetComponent<WireController>();
    wireControllers.Add(wireController);
    wireController.Init(w);
  }

  private void HandleFragmentRemoved(Fragment fragment) {
    fragmentControllers.Remove(fragment.controller);
    fragment.controller.Removed();
  }

  private void HandleFragmentAdded(Fragment fragment) {
    var go = Instantiate(fragmentPrefabs[fragment.GetType()], Vector3.zero, Quaternion.identity);
    FragmentController f = go.GetComponent<FragmentController>();
    fragmentControllers.Add(f);
    f.Init(fragment);
  }

  void Update() {
    GameModel.main.simulate(Time.deltaTime);
  }
}

[Serializable]
public struct FragmentPrefabMapping {
  [SerializeField]
  public string className;
  [SerializeField]
  public GameObject prefab;
}