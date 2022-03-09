using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModelController : MonoBehaviour {
  public static GameModelController main;
  public GameModel model => GameModel.main;

  public Transform wireContainer;
  public GameObject floorPrefab;
  public GameObject wirePrefab;

  public GameObject[] editModeObjects;

  [ReadOnly]
  public FloorController floor;
  [SerializeField]
  public FragmentPrefabMapping[] Mapping;

  private Dictionary<System.Type, GameObject> fragmentPrefabs = new Dictionary<Type, GameObject>();

  [ReadOnly]
  public List<FragmentController> fragmentControllers = new List<FragmentController>();
  public HashSet<WireController> wireControllers = new HashSet<WireController>();

  void Awake() {
    main = this;
    foreach (var entry in Mapping) {
      var type = Type.GetType(entry.className);
      fragmentPrefabs.Add(type, entry.prefab);
    }
    GameModel.SetMainToNewGame();
  }

  void Start() {
    Init(GameModel.main);
    UpdateIsEditMode();
  }

  void Init(GameModel model) {
    floor = Instantiate(floorPrefab, Vector3.zero, Quaternion.identity).GetComponent<FloorController>();
    floor.Init(model.floor);

    foreach (var f in model.Fragments) {
      HandleFragmentAdded(f);
    }

    foreach (var w in model.Wires) {
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

  internal void UpdateIsEditMode() {
    foreach (var o in editModeObjects) {
      o.SetActive(model.isEditMode);
    }
    foreach (var fc in fragmentControllers) {
      // StartCoroutine(WaitForAnimationThenFixRigidbodies());
      if (!(fc.fragment is PlayerFragment)) {
        if (model.isEditMode) {
          // make them clickable
          var rb2d = fc.gameObject.AddComponent<Rigidbody2D>();
          rb2d.bodyType = RigidbodyType2D.Static;
        } else {
          var rb2d = fc.gameObject.GetComponent<Rigidbody2D>();
          if (rb2d) {
            Destroy(rb2d);
          }
        }
      }
    }
  }

  private IEnumerator WaitForAnimationThenFixRigidbodies() {
    if (model.isEditMode) {
      while (GameModel.main.player.controller.GetComponent<PlayerMovementInput>().rb2d.rotation != 0) {
        yield return new WaitForEndOfFrame();
      }
    }
    yield return new WaitForEndOfFrame();
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