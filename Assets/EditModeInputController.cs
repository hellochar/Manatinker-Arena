using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditModeInputController : MonoBehaviour {
  public LineRenderer selectionRing;

  public static EditModeInputController instance;
  public string instructions => inputState.instructions;
  public InputState inputState = InputState.Default;

  void Awake() {
    instance = this;
  }

  private Vector3[] positions = new Vector3[63];

  public void Reset() {
    Transition(InputState.Default);
  }

  public void clickGround() {
    if (GameModelController.main.isEditMode) {
      inputState.clickGround();
    }
  }

  void Update() {
    UpdateHovered();
    // UpdateMouse();
    if (Input.GetKeyDown(KeyCode.Escape)) {
      clickGround();
    }
    inputState.update();
    UpdateSelected(inputState.selected);
  }

  // void UpdateMouse() {
  //   if (Input.GetMouseButtonDown(0)) {
  //     var fc = hovered?.GetComponent<FragmentController>();
  //     if (fc != null) {
  //       mouseDownOnFragment(fc);
  //     } else {
  //       clickGround();
  //     }
  //   } else if (Input.GetMouseButtonUp(0)) {
  //     var fc = hovered?.GetComponent<FragmentController>();
  //     if (fc != null) {
  //       mouseUpOnFragment(fc);
  //     }
  //   }
  // }

  public FragmentController hovered = null;
  private void UpdateHovered() {
    // Debug.Log(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject());
    // Debug.Log(UnityEngine.EventSystems.EventSystem.current.currentInputModule.);
    Vector2 mousePositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition).xy();
    RaycastHit2D hit = Physics2D.Linecast(mousePositionWorld, mousePositionWorld + new Vector2(0.001f, 0.001f));
    hovered = hit.collider?.GetComponentInParent<FragmentController>();
  }

  private FragmentController lastSelected;

  private void UpdateSelected(FragmentController selected) {
    if (selected == null) {
      selectionRing.enabled = false;
    }
    if (selected != lastSelected) {
      if (lastSelected != null) {
        lastSelected.StopSelection();
      }
      if (selected != null) {
        selected.StartSelection();
        var selectedWorldSize = selected.worldSize();
        var diameter = Mathf.Max(selectedWorldSize.x, selectedWorldSize.y) * 1.5f;
        ZoneOfInfluenceController.RebuildLineRenderer(selectionRing, positions, diameter / 2);
        selectionRing.enabled = true;
      }
      lastSelected = selected;
    }
    if (selected != null) {
      selectionRing.transform.position = selected.transform.position;
    }
  }

  public void Transition(InputState next) {
    inputState.exit();
    inputState = next;
    inputState.enter();
    UpdateSelected(inputState.selected);
  }

  public void mouseDownOnFragment(FragmentController fc) {
    if (GameModelController.main.isEditMode) {
      inputState.mouseDownOnFragment(fc);
    }
  }

  public void mouseUpOnFragment(FragmentController fc) {
    if (GameModelController.main.isEditMode) {
      inputState.mouseUpOnFragment(fc);
    }
  }
}

public abstract class InputState {
  public abstract string instructions { get; }
  public static InputStateDefault Default = new InputStateDefault();
  public virtual FragmentController selected => null;
  public static bool canSelect(Fragment f) {
    return f.isPlayerOwned || f.owner == null;
  }

  public virtual void mouseDownOnFragment(FragmentController fc) { }
  public virtual void mouseUpOnFragment(FragmentController fc) { }
  public virtual void clickGround() { }
  public virtual void update() { }
  public virtual void enter() { }
  public virtual void exit() { }
  protected void Transition(InputState other) {
    EditModeInputController.instance.Transition(other);
  }

  public FragmentController getHovered() => EditModeInputController.instance.hovered;
}

public class InputStateDefault : InputState {
  public override string instructions => "Click - select a Fragment.";

  public override void mouseDownOnFragment(FragmentController fc) {
    var fragment = fc.fragment;
    // only allow dragging fragments that the player controls
    if (canSelect(fragment)) {
      Transition(new InputStateSelected(fc));
    }
  }
}

internal class InputStateSelected : InputState {
  private FragmentController _selected;
  private bool isDraggable => !(selected.fragment is Avatar);

  public override FragmentController selected => _selected;
  bool isNeutral => selected.fragment.owner == null;

  public InputStateSelected(FragmentController fc) {
    this._selected = fc;
  }

  public bool canMakeWire => selected.fragment.hasOutput;

  private static string dragText = "Drag - move.\nMouse-wheel - rotate (hold Alt for more control).";
  public override string instructions => isNeutral ?
    "Drag into your zone of influence!" :
    $"{(isDraggable ? dragText : "")}{(canMakeWire ? "\nX - create a wire." : "")}".Trim();

  public override void clickGround() {
    Transition(InputState.Default);
  }

  public override void enter() {
    // possibly grab 
    var f = selected.fragment;
    var player = GameModel.main.player;
    var isObjectOutOfInfluence = f.distance(player) > player.influenceRadius;
    if (f.owner == null && !isObjectOutOfInfluence) {
      f.builtinOffset -= player.worldPos;
      f.builtinAngle -= player.worldRotation;
      f.owner = player;
      player.avatar.connect(f);
    }
  }

  public override void update() {
    if (Input.GetMouseButton(0) && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)) {
      if (getHovered() == selected && isDraggable) {
        // player's trying to drag
        Transition(new InputStateDragged(selected));
        return;
      }
    }
    // aka owned by player
    if (!isNeutral) {
      if (Input.GetKeyDown(KeyCode.X) && canMakeWire) {
        Transition(new InputStateWireEdit(selected));
      }

      // rotation
      if (isDraggable) {
        var shouldRotateSnap = !Input.GetKey(KeyCode.LeftAlt);
        var amplitude = shouldRotateSnap ? 15 : 1;
        var mouseWheelAmount = Input.GetAxis("Mouse ScrollWheel");

        var diff = 0;
        if (mouseWheelAmount < 0) {
          diff = -amplitude;
        } else if (mouseWheelAmount > 0) {
          diff = amplitude;
        }
        if (diff != 0) {
          UnityEngine.Object.Instantiate(VFX.Get("pickupFragment"), selected.transform.position, Quaternion.identity);
          var newAngle = selected.fragment.builtinAngle + diff;
          if (shouldRotateSnap) {
            newAngle = Util.Snap(newAngle, amplitude);
          }
          selected.fragment.builtinAngle = newAngle;
        }
      }
    }
  }

  public override void mouseDownOnFragment(FragmentController fc) {
    if (fc == selected && isDraggable) {
      // starting a drag
      Transition(new InputStateDragged(fc));
      return;
    } else if (canSelect(fc.fragment)) {
      // selecting a different one
      Transition(new InputStateSelected(fc));
      return;
    }
  }
}

internal class InputStateWireEdit : InputState {
  public override string instructions => "Click a Fragment - toggle wire.\nEsc or X - cancel.";
  private FragmentController from;
  public override FragmentController selected => from;
  public Wire tempWire;

  public InputStateWireEdit(FragmentController from) {
    this.from = from;
  }

  public override void enter() {
    tempWire = new Wire(from.fragment, null, null);
    GameModelController.main.HandleWireAdded(tempWire);
  }

  public override void exit() {
    GameModelController.main.HandleWireRemoved(tempWire);
  }

  public override void clickGround() {
    Transition(InputState.Default);
  }

  private void TransitionBack() {
    Transition(new InputStateSelected(from));
  }

  void ToggleWire(FragmentController to) {
    if (!to.fragment.hasInput || !to.fragment.isPlayerOwned) {
      return;
    }
    var isConnected = from.fragment.isConnected(to.fragment);
    if (isConnected) {
      from.fragment.disconnect(to.fragment);
    } else {
      from.fragment.connect(to.fragment);
    }
    TransitionBack();
  }

  public override void update() {
    Fragment to;
    if (getHovered() == null) {
      to = null;
    } else {
      var targetFc = getHovered();
      if (targetFc != null && targetFc.fragment.hasInput && targetFc.fragment.isPlayerOwned) {
        to = targetFc.fragment;
      } else {
        to = null;
      }
    }
    tempWire.to = to;
    if (Input.GetKeyDown(KeyCode.X)) {
      TransitionBack();
    }
  }

  public override void mouseDownOnFragment(FragmentController fc) {
    ToggleWire(fc);
  }
}

internal class InputStateDragged : InputState {
  public override string instructions => $"Alt - disable snapping.";
  private FragmentController dragged;
  public override FragmentController selected => dragged;

  public InputStateDragged(FragmentController fc) {
    this.dragged = fc;
  }

  bool isObjectOutOfInfluence {
    get {
      var player = GameModel.main.player;
      return dragged.fragment.distance(player) > player.influenceRadius;
    }
  }

  void TransitionBack() {
    if (isObjectOutOfInfluence) {
      Transition(InputState.Default);
    } else {
      Transition(new InputStateSelected(dragged));
    }
  }

  public override void enter() {
    // UnityEngine.Object.Instantiate(VFX.Get("pickupFragment"), dragged.transform.position, Quaternion.identity);
  }

  public override void exit() {
    UnityEngine.Object.Instantiate(VFX.Get("pickupFragment"), dragged.transform.position, Quaternion.identity);
    var player = GameModel.main.player;
    var f = dragged.fragment;
    // we're too far away, remove from owner
    if (f.isPlayerOwned && isObjectOutOfInfluence) {
      f.disconnectAll();
      f.builtinAngle = f.worldRotation;
      f.builtinOffset = f.worldPos;
      f.owner = null;
      // give to owner
    } else if (f.owner == null && !isObjectOutOfInfluence) {
      f.builtinOffset -= player.worldPos;
      f.builtinAngle -= player.worldRotation;
      f.owner = player;
      player.avatar.connect(f);
    }
  }

  public override void update() {
    if (!Input.GetMouseButton(0)) {
      TransitionBack();
    }
    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    var fragment = dragged.fragment;
    Vector2 worldOffset = worldPosition.xy();
    if (fragment.owner != null) {
      worldOffset -= fragment.owner.controller.transform.position.xy();
    }
    // hold alt to get precise
    var offset = Input.GetKey(KeyCode.LeftAlt) ? worldOffset : Util.Snap(worldOffset, 0.25f);
    fragment.builtinOffset = offset;
  }

  public override void clickGround() {
    TransitionBack();
  }

  public override void mouseUpOnFragment(FragmentController fc) {
    if (fc != dragged) {
      // weird but ok
      Debug.LogWarning("weird mouseup on non-dragged object");
    }
    TransitionBack();
  }

  public override void mouseDownOnFragment(FragmentController fc) {
    // we're in a weird state.
    TransitionBack();
  }
}