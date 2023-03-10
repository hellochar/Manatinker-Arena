using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModelController : MonoBehaviour {
  public static GameModelController main;
  public GameModel model => GameModel.main;

  public Transform wireContainer;
  public GameObject floorPrefab;
  public GameObject wirePrefab;
  public GameObject deathUI;
  public EditModeInputController editModeController;

  public bool _isEditMode;
  public bool isEditMode => _isEditMode;
  public GameObject[] editModeObjects;

  [ReadOnly]
  public FloorController floor;
  public FragmentDatabase Database;

  private Dictionary<System.Type, GameObject> fragmentPrefabs = new Dictionary<Type, GameObject>();

  [ReadOnly]
  public List<FragmentController> fragmentControllers = new List<FragmentController>();
  public HashSet<WireController> wireControllers = new HashSet<WireController>();
  private Coroutine activeAnimation;
  public bool hasActiveAnimation => activeAnimation != null;

  public void EnterEditMode() {
    // set true immediately
    _isEditMode = true;
    GameModel.main.player.controller.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    UpdateIsEditMode(true);
  }

  public void ExitEditMode() {
    editModeController.Reset();
    GameModel.main.player.controller.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    UpdateIsEditMode(false, () => {
      _isEditMode = false;
    });
  }

  void Awake() {
    main = this;
    foreach(var prefab in Database.Items) {
      var type = Type.GetType(prefab.name);
      if (type == null) {
        Debug.LogWarning("Skipping " + prefab.name);
        continue;
      }
      fragmentPrefabs.Add(type, prefab);
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

  public void HandleWireRemoved(Wire w) {
    wireControllers.Remove(w.controller);
    w.controller.Removed();
    // wireControllers.
  }

  public void HandleWireAdded(Wire w) {
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
    var go = Instantiate(fragmentPrefabs[fragment.GetType()]);
    FragmentController f = go.GetComponent<FragmentController>();
    fragmentControllers.Add(f);
    f.Init(fragment);
  }

  internal void UpdateIsEditMode(bool value, Action callback = null) {
    if (hasActiveAnimation || GameModel.main.player.isDead) {
      return;
    }
    foreach (var o in editModeObjects) {
      o.SetActive(value);
    }
    Action cb = () => {
      activeAnimation = null;
      callback?.Invoke();
    };
    var rect = Camera.main.rect;
    rect.x = value ? 0.2f : 0;
    Camera.main.rect = rect;
    if (value) {
      activeAnimation = StartCoroutine(ResetRotationThenUpdateRigidbodies(value, cb));
    } else {
      // UpdateRigidbodies(value);
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

    // UpdateRigidbodies(value);
    cb();
  }

  // private void UpdateRigidbodies(bool value) {
  //   foreach (var child in model.Fragments) {
  //     child.controller.UpdateRigidbody(value);
  //   }
  // }

  void Update() {
    GameModel.main.simulate(Time.deltaTime);

    #if UNITY_EDITOR
    if (Input.GetKeyDown(KeyCode.L)) {
      var frags = RegisteredFragmentAttribute.GetAllFragmentTypes(typeof(Fragment));
      for(var i = 0; i < frags.Count; i++) {
        var f = GameRound.NewFragmentFrom(frags[i]);
        var x = i % 5 * 3;
        var y = (i / 5 - 2.5f) * 3;
        f.builtinOffset = new Vector2(5 + x, y + GameModel.main.floor.height / 2);
        GameModel.main.AddFragment(f);
      }
    }
    if (Input.GetKeyDown(KeyCode.P)) {
      GameModel.main.currentRound.spawnEnemy(7, 7, 1.5f, new Vector2(5, 5));
    }
    #endif
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