using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarController : MonoBehaviour {
  private Fragment fragment => qfc.fragment;
  public Image hpFilled;
  public TMPro.TMP_Text text;
  public bool bIsMana = false;
  private QuickInfoController qfc;

  void Start() {
    qfc = GetComponentInParent<QuickInfoController>();
    Update();
    if (bIsMana && fragment.manaMax == 0) {
      gameObject.SetActive(false);
    }
  }

  void Update() {
    if (bIsMana) {
      text.text = $"{fragment.Mana.ToString("0.#")}/{fragment.manaMax}";
      hpFilled.fillAmount = fragment.Mana / fragment.manaMax;
    } else {
      text.text = $"{fragment.Hp.ToString("0.#")}/{fragment.hpMax}";
      hpFilled.fillAmount = fragment.Hp / fragment.hpMax;
    }
  }
}