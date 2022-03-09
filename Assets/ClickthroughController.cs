using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// if we click here, deselect 
public class ClickthroughController : MonoBehaviour {
  void OnMouseDown() {
    EditModeInputController.instance.Reset();
  }
}
