using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditModeInputController : MonoBehaviour {
  public static EditModeInputController instance;
  public string instructions => inputState.instructions;
  public InputState inputState = InputState.Default;

  void Awake() {
    instance = this;
  }

  public void Reset() {
    Transition(InputState.Default);
  }

  public void clickGround() {
    inputState.clickGround();
  }

  void Update() {
    inputState.update();
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

  public virtual void mouseDownOnFragment(FragmentController fc) {}
  public virtual void mouseUpOnFragment(FragmentController fc) {}
  public virtual void clickGround() {}
  public virtual void update() {}
  public virtual void enter() {}
  public virtual void exit() {}
  protected void Transition(InputState other) {
    EditModeInputController.instance.Transition(other);
  }
}

public class InputStateDefault : InputState {
  public override string instructions => "Click - select a Fragment.";

  public override void mouseDownOnFragment(FragmentController fc) {
    var fragment = fc.fragment;
    // only allow dragging fragments that the player controls
    if (fragment.isPlayerOwned) {
      Transition(new InputStateSelected(fc));
    }
  }
}

internal class InputStateSelected : InputState {
  private FragmentController selected;

  public InputStateSelected(FragmentController fc) {
    this.selected = fc;
  }

  public override string instructions => $"Selected {selected.fragment.DisplayName}.\nDrag - move.\nMouse-wheel - rotate (hold Alt for more control).\nX - create a wire.";

  public override void clickGround() {
    Transition(InputState.Default);
  }

  public override void update() {
    if (Input.GetMouseButton(0) && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)) {
      // player's trying to drag
      Transition(new InputStateDragged(selected));
    }
    if (Input.GetKeyDown(KeyCode.X)) {
      Transition(new InputStateWireEdit(selected));
    }
    // rotation
    {
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
    } else {
      // selecting a different one
      Transition(new InputStateSelected(fc));
      return;
    }
  }
}

internal class InputStateWireEdit : InputState {
  public override string instructions => "Click a Fragment - toggle wire.\nX - cancel.";
  private FragmentController from;

  public InputStateWireEdit(FragmentController from) {
    this.from = from;
  }

  void ToggleWire(FragmentController to) {
    var isConnected = from.fragment.isConnected(to.fragment);
    if (isConnected) {
      from.fragment.disconnect(to.fragment);
    } else {
      from.fragment.connect(to.fragment);
    }
  }

  public override void update() {
    if (Input.GetKeyDown(KeyCode.X)) {
      Transition(new InputStateSelected(from));
    }
  }

  public override void mouseDownOnFragment(FragmentController fc) {
    ToggleWire(fc);
  }
}

internal class InputStateDragged : InputState {
  public override string instructions => $"Alt - disable snapping.";
  private FragmentController dragged;

  public InputStateDragged(FragmentController fc) {
    this.dragged = fc;
  }

  void TransitionBack() {
    Transition(new InputStateSelected(dragged));
  }

  public override void update() {
    if (!Input.GetMouseButton(0)) {
      TransitionBack();
    }
    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    var fragment = dragged.fragment;
    Vector2 worldOffset = worldPosition.xy() - fragment.owner.controller.transform.position.xy();
    // hold alt to get precise
    var offset = Input.GetKey(KeyCode.LeftAlt) ? worldOffset : Util.Snap(worldOffset, 0.25f);
    fragment.builtinOffset = offset;
  }

  public override void clickGround() {
    base.clickGround();
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