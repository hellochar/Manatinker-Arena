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
  void Start() {
  }

  public void Reset() {
    Transition(InputState.Default);
  }

  public void clickGround() {
    inputState.clickGround();
  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      clickGround();
    }
    inputState.update();
    UpdateSelected(inputState.selected);
  }

  private FragmentController lastSelected;
  private void UpdateSelected(FragmentController selected) {
    if (selected == null) {
      selectionRing.enabled = false;
      return;
    }
    if (selected != lastSelected) {
      var selectedWorldSize = selected.worldSize();
      var diameter = Mathf.Max(selectedWorldSize.x, selectedWorldSize.y) * 1.5f;
      ZoneOfInfluenceController.RebuildLineRenderer(selectionRing, positions, diameter / 2);
      lastSelected = selected;
    }
    selectionRing.transform.position = selected.transform.position;
    selectionRing.enabled = true;
  }

  public void Transition(InputState next) {
    inputState.exit();
    inputState = next;
    inputState.enter();
  }

  public void mouseDownOnFragment(FragmentController fc) {
    inputState.mouseDownOnFragment(fc);
  }

  public void mouseUpOnFragment(FragmentController fc) {
    inputState.mouseUpOnFragment(fc);
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
  public override FragmentController selected => _selected;
  bool isNeutral => selected.fragment.owner == null;

  public InputStateSelected(FragmentController fc) {
    this._selected = fc;
  }

  public override string instructions => isNeutral ?
    "Drag into your zone of influence!" :
    $"Selected {selected.fragment.DisplayName}.\nDrag - move.\nMouse-wheel - rotate (hold Alt for more control).\nX - create a wire.";

  public override void clickGround() {
    Transition(InputState.Default);
  }

  public override void update() {
    if (Input.GetMouseButton(0) && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)) {
      Vector2 mousePositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition).xy();
      RaycastHit2D hit = Physics2D.Linecast(mousePositionWorld, mousePositionWorld + new Vector2(0.001f, 0.001f));
      if(hit.transform != null) {
        if (hit.transform.gameObject == selected.gameObject) {
          // player's trying to drag
          Transition(new InputStateDragged(selected));
          return;
        }
      }
    }
    // aka owned by player
    if (!isNeutral) {
      if (Input.GetKeyDown(KeyCode.X)) {
        Transition(new InputStateWireEdit(selected));
      }

      // rotation
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
        var newAngle = selected.fragment.builtinAngle + diff;
        if (shouldRotateSnap) {
          newAngle = Util.Snap(newAngle, amplitude);
        }
        selected.fragment.builtinAngle = newAngle;
      }
    }
  }

  public override void mouseDownOnFragment(FragmentController fc) {
    if (fc == selected) {
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
    tempWire = new Wire(from.fragment, null);
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
    var isConnected = from.fragment.isConnected(to.fragment);
    if (isConnected) {
      from.fragment.disconnect(to.fragment);
    } else {
      from.fragment.connect(to.fragment);
    }
    TransitionBack();
  }

  public override void update() {
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
      var f = dragged.fragment;
      var distanceToPlayer = Vector2.Distance(f.worldPos, player.worldPos);
      return distanceToPlayer > player.influenceRadius;
    }
  }

  void TransitionBack() {
    if (isObjectOutOfInfluence) {
      Transition(InputState.Default);
    } else {
      Transition(new InputStateSelected(dragged));
    }
  }

  public override void exit() {
    var player = GameModel.main.player;
    var f = dragged.fragment;
    // we're too far away, remove from owner
    if (f.isPlayerOwned && isObjectOutOfInfluence) {
      f.disconnectAll();
      f.builtinAngle = f.worldRotation;
      f.builtinOffset = f.worldPos;
      f.owner = null;
    } else if (f.owner == null && !isObjectOutOfInfluence) {
      f.builtinOffset -= player.worldPos;
      f.builtinAngle -= player.worldRotation;
      f.owner = player;
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