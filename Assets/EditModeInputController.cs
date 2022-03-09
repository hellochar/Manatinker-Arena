using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditModeInputController : MonoBehaviour {
  public static EditModeInputController instance;
  public FragmentController selected;
  // public bool creatingWire = false;
  // public bool isDragging = false;
  public string instructions {
    get => inputState.instructions;
    // get {
    //   string s;
    //   if (isDragging) {
    //     s = "Dragging.";
    //   } else {
    //     if (creatingWire) {
    //       if (selected == null) {
    //         s = "Click a Fragment - start a wire.\nX - cancel.";
    //       }
    //       s = "Creating a wire.\nClick another Fragment - create or remove a wire.\nX - cancel.";
    //     } else {
    //       s = "Click - select a Fragment.\nDrag - move a Fragment.\nX - create a wire.";
    //     }
    //   }
    //   if (selected != null) {
    //     s = selected.ToString() + "\n" + s;
    //   }
    //   return s;
    // }
  }

  public InputState inputState = InputState.Default;
  void Start() {
    instance = this;
  }

  public void Reset() {
    Transition(InputState.Default);
    selected = null;
    // isDragging = false;
    // creatingWire = false;
  }

  public void clickGround() {
    inputState.clickGround();
  }

  // Update is called once per frame
  void Update() {
    inputState.update();
    // if (Input.GetKeyDown(KeyCode.X)) {
    //   creatingWire = !creatingWire;
    // }
    // if (selected != null) {
    //   if (Input.GetMouseButton(0)) {
    //     if (isDragging) {
    //     }
    //   } else {
    //     if (isDragging) {
    //       isDragging = false;
    //     }
    //   }
    //   var rotation = Input.GetAxis("Mouse ScrollWheel");
    //   if (rotation < 0) {
    //     selected.fragment.builtinAngle -= 15;
    //   } else if (rotation > 0) {
    //     selected.fragment.builtinAngle += 15;
    //   }
    // }
  }

  public void Transition(InputState next) {
    inputState.exit();
    inputState = next;
    inputState.enter();
  }

  public void mouseDownOnFragment(FragmentController fc) {
    // if (selected == null || selected == fc) {
    //   selected = fc;
    //   isDragging = true;
    //   return;
    // }
    // if (creatingWire && selected != null) {
    //   var isConnected = selected.fragment.isConnected(fc.fragment);
    //   if (isConnected) {
    //     selected.fragment.disconnect(fc.fragment);
    //   } else {
    //     selected.fragment.connect(fc.fragment);
    //   }
    // }
    inputState.mouseDownOnFragment(fc);
  }

  public void mouseUpOnFragment(FragmentController fc) {
    inputState.mouseUpOnFragment(fc);
    // isDragging = false;
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

  public override string instructions => $"Selected {selected.fragment.DisplayName}.\nDrag to move.\nX - create a wire.";

  public override void clickGround() {
    Transition(InputState.Default);
  }

  public override void update() {
    if (Input.GetMouseButton(0) && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)) {
      // player's trying to drag
      Transition(new InputStateDragged(selected));
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

internal class InputStateDragged : InputState {
  public override string instructions => $"Alt - disable snapping.\nRelease - finish drag.";
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