using System;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour {
  public FragmentController fC;
  private Fragment fragment => fC.fragment;
  private RectTransform rt;
  public Vector2 offset;
  public Vector2 sizeScalar;

  void Start() {
    rt = GetComponent<RectTransform>();
    LateUpdate();
  }

  public void LateUpdate() {
    if (fC != null) {
      // set position to above the fragment
      var spriteBounds = fC.worldBounds();
      var worldOffset = new Vector2(
        Mathf.Lerp(spriteBounds.min.x, spriteBounds.max.x, sizeScalar.x),
        Mathf.Lerp(spriteBounds.min.y, spriteBounds.max.y, sizeScalar.y)) + offset;
      // var worldOffset = spriteSize * sizeScalar + offset;
      var screenPoint = Camera.main.WorldToScreenPoint(worldOffset.z());
      rt.position = screenPoint;
      // rt.anchoredPosition = screenPoint.xy();
    }
  }
}