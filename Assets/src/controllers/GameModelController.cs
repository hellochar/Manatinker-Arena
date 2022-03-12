using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModelController : MonoBehaviour {
  public static GameModelController main;
  public GameModel model => GameModel.main;

  public Transform wireContainer;
  public GameObject healthbars;
  public GameObject floorPrefab;
  public GameObject wirePrefab;
  public GameObject deathUI;
  public EditModeInputController editModeController;

  public bool _isEditMode;
  public bool isEditMode => _isEditMode;
  public GameObject[] editModeObjects;

  [ReadOnly]
  public FloorController floor;
  [SerializeField]
  public FragmentPrefabMapping[] Mapping;

  private Dictionary<System.Type, GameObject> fragmentPrefabs = new Dictionary<Type, GameObject>();

  [ReadOnly]
  public List<FragmentController> fragmentControllers = new List<FragmentController>();
  public HashSet<WireController> wireControllers = new HashSet<WireController>();
  private Coroutine activeAnimation;
  public bool hasActiveAnimation => activeAnimation != null;

  public void EnterEditMode() {
    // set true immediately
    _isEditMode = true;
    UpdateIsEditMode(true);
  }

  public void ExitEditMode() {
    editModeController.Reset();
    UpdateIsEditMode(false, () => {
      _isEditMode = false;
    });
  }

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
    foreach (var o in editModeObjects) {
      o.SetActive(isEditMode);
    }
  }

  void Init(GameModel model) {
    floor = Instantiate(floorPrefab).GetComponent<FloorController>();
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
    // there are weird cases where fragments are double-removed but it's fine
    fragment.controller?.Removed();
  }

  private void HandleFragmentAdded(Fragment fragment) {
    var go = Instantiate(fragmentPrefabs[fragment.GetType()], Vector3.zero, Quaternion.identity);
    FragmentController f = go.GetComponent<FragmentController>();
    fragmentControllers.Add(f);
    f.Init(fragment);
  }

  internal void UpdateIsEditMode(bool value, Action callback = null) {
    if (hasActiveAnimation || GameModel.main.player.isDead) {
      return;
    }
    Action cb = () => {
      foreach (var o in editModeObjects) {
        o.SetActive(value);
      }
      activeAnimation = null;
      callback?.Invoke();
    };
    if (value) {
      activeAnimation = StartCoroutine(ResetRotationThenUpdateRigidbodies(value, cb));
    } else {
      UpdateRigidbodies(value);
      cb();
    }
  }

  private IEnumerator ResetRotationThenUpdateRigidbodies(bool value, Action cb) {
    var playerController = model.player.controller;
    var rb2d = playerController.GetComponent<Rigidbody2D>();
    rb2d.SetRotation(0);

    yield return new WaitForSeconds(0.25f);

    // let physics catch up
    while (rb2d.rotation != 0) {
      yield return new WaitForEndOfFrame();
    }

    UpdateRigidbodies(value);
    cb();
  }

  private void UpdateRigidbodies(bool value) {
    foreach (var child in model.Fragments) {
      child.controller.UpdateRigidbody(value);
    }
  }

  void Update() {
    healthbars?.SetActive(Input.GetKey(KeyCode.LeftAlt));
    GameModel.main.simulate(Time.deltaTime);
  }

  public void PlayerDied() {
    deathUI.SetActive(true);
  }

  bool bIsLoading = false;
  public void RetryGame() {
    if (bIsLoading) {
      return;
    }
    bIsLoading = true;
    SceneManager.LoadSceneAsync("Scenes/Arena", LoadSceneMode.Single);
  }
}

[Serializable]
public struct FragmentPrefabMapping {
  [SerializeField]
  public string className;
  [SerializeField]
  public GameObject prefab;
}