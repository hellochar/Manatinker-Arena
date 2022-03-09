using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditModeInputController : MonoBehaviour {
  public static EditModeInputController instance;
  public FragmentController selected;
  public string instructions {
    get {
      var s = "Click a piece to select it.\nClick-drag to move.\nClick its outport to create a wire.";
      if (selected != null) {
        s = selected.ToString() + "\n" + s;
      }
      return s;
    }
  }

  // public InputState inputState = new InputStateDefault();
  void Start() {
    instance = this;
  }

  public void Reset() {
    selected = null;
    // inputState = new InputStateDefault();
  }

  Vector2 snap(Vector2 v, float factor) {
    return new Vector2(Mathf.Round(v.x / factor) * factor, Mathf.Round(v.y / factor) * factor);
  }

  // Update is called once per frame
  void Update() {
    if (selected != null && Input.GetMouseButton(0)) {
      Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      var fragment = selected.fragment;
      if (fragment.owner != null) {
        // fragment.owner
        Vector2 worldOffset = worldPosition.xy() - fragment.owner.controller.transform.position.xy();
        Vector2 roundedOffset = snap(worldOffset, 0.25f);
        fragment.builtinOffset = roundedOffset;
      }
    }
    if (selected != null) {
      var rotation = Input.GetAxis("Mouse ScrollWheel");
      if (rotation < 0) {
        selected.fragment.builtinAngle -= 15;
      } else if (rotation > 0) {
        selected.fragment.builtinAngle += 15;
      }
    }
  }

  // public void Transition(InputState next) {
  //   inputState.finish();
  //   inputState = next;
  // }

  public void mouseDown(FragmentController fc) {
    selected = fc;
  //   inputState.mouseDown(fc);
  }

  public void mouseUp(FragmentController fc) {
  }
}

// public abstract class InputState {
//   public abstract string instructions { get; }
//   public virtual void mouseDown(FragmentController fc) {}
//   public virtual void mouseUp(FragmentController fc) {}
//   public virtual void mouseMove() {}
//   public virtual void finish() {}
// }

// public class InputStateDefault : InputState {
//   public override string instructions => "Click a piece to select it.\nClick-drag to move.\nClick its outport to create a wire.";

//   public override void mouseDown(FragmentController fc) {
//     EditModeInputController.instance.Transition(new InputStateSelected(fc));
//   }

//   public override void mouseUp(FragmentController fc) {
//     throw new System.NotImplementedException();
//   }
// }

// internal class InputStateSelected : InputState {
//   private FragmentController fc;

//   public InputStateSelected(FragmentController fc) {
//     this.fc = fc;
//   }

//   public override string instructions => "Drag to move";
// }