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
  public TMPro.TMP_Text inportflow, outportflow, weight;
  public GameObject levelUp;
  public GameObject Manabar;

  void Start() {
    Update();
  }

  public Fragment fragment => GameModelController.main.editModeController.inputState.selected?.fragment;

  void Update() {
    if (fragment == null) {
      inset.SetActive(false);
    } else {
      levelUp.SetActive(!(fragment is PlayerAvatar));
      inport.SetActive(fragment.hasInput);
      outport.SetActive(fragment.hasOutput);
      inset.SetActive(true);
      fragmentIcon.sprite = fragment.controller.spriteRenderer.sprite;
      fragmentName.text = fragment.DisplayName;
      fragmentInfo.text = fragment.GetInfo();

      weight.text = $"{fragment.weight} kg";

      inportflow.gameObject.SetActive(fragment.hasInput);
      outportflow.gameObject.SetActive(fragment.hasOutput);
      inportflow.text = $"{fragment.inFlowRate.ToString("#0.#")} mana/s";
      outportflow.text = $"{fragment.outFlowRate.ToString("#0.#")} mana/s";

      Manabar.SetActive(fragment.manaMax > 0);
    }
  }
}
