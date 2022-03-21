using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoveredFragmentController : MonoBehaviour {
  public TMPro.TMP_Text text;
  public InGameUI inGameUI;

  // Update is called once per frame
  public void Update() {
    var editMode = EditModeInputController.instance;
    var hoveredFC = editMode.hovered;
    inGameUI.fC = hoveredFC;
    inGameUI.LateUpdate();
    if (hoveredFC != null) {
      text.text = hoveredFC.fragment.DisplayName;
    }
  }
}
