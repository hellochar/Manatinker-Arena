using System;
using UnityEngine;
using UnityEngine.UI;

public class InGameHealthbarController : MonoBehaviour {
  private FragmentController fC;
  private Fragment fragment => fC.fragment;
  public Image hpFilled;
  private RectTransform rt;

  public void Init(FragmentController fragmentController) {
    this.fC = fragmentController;
  }

  void Start() {
    rt = GetComponent<RectTransform>();
    Update();
  }

  void Update() {
    // set position to above the fragment
    var spriteSize = fC.gameObject.transform.Find("Sprite").localScale;
    var worldOffset = new Vector2(0, spriteSize.y + 0.1f);
    var screenPoint = Camera.main.WorldToScreenPoint(fC.gameObject.transform.position + worldOffset.z());
    rt.anchoredPosition = screenPoint.xy();

    var scale = spriteSize;
    rt.localScale = scale;
    hpFilled.fillAmount = fragment.Hp / fragment.hpMax;
  }
}