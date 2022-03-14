using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FragmentInfoController : MonoBehaviour {
  public GameObject inset;
  public Image fragmentIcon;
  public TMPro.TMP_Text fragmentName;
  public TMPro.TMP_Text fragmentInfo;
  public GameObject inport, outport;
  public GameObject levelUp;

  void Start() {
    Update();
  }

  void Update() {
    var selected = GameModelController.main.editModeController.inputState.selected;
    if (selected == null) {
      inset.SetActive(false);
    } else {
      levelUp.SetActive(!(selected.fragment is PlayerAvatar));
      inport.SetActive(selected.fragment.hasInput);
      outport.SetActive(selected.fragment.hasOutput);
      inset.SetActive(true);
      fragmentIcon.sprite = selected.spriteRenderer.sprite;
      // fragmentIcon.SetNativeSize();
      fragmentName.text = selected.fragment.DisplayName;
      fragmentInfo.text = selected.fragment.GetInfo();
    }
  }
}
