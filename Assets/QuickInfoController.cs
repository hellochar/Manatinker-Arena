using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickInfoController : MonoBehaviour {
  internal Fragment fragment;
  public Image img;
  public TMPro.TMP_Text Name;

  internal void Init(Fragment obj) {
    fragment = obj;
    img.sprite = fragment.controller.spriteRenderer.sprite;
    Update();
  }

  // Update is called once per frame
  void Update() {
    if (!fragment.isPlayerOwned || fragment.isDead || !fragment.pinInSidebar) {
      Destroy(gameObject);
    } else {
      string extraText = "";
      if (fragment.inputPercent > 0) {
        var inputPercent = fragment.inputPercent;
        extraText += $"→ {(inputPercent * 100).ToString("##0")}%\t\t";
      }
      if (fragment.outputPercent > 0) {
        var outputPercent = fragment.outputPercent;
        extraText += $"{(outputPercent * 100).ToString("##0")}% →";

      }
      Name.text = $"{fragment.DisplayName} {extraText}";
    }
  }
}
