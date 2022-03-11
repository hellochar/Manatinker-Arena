using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour {
  public Sprite[] sprites;
  public FragmentController fc;
  public SpriteRenderer sr;

  // Update is called once per frame
  void Update() {
    var hpPercent = fc.fragment.Hp / fc.fragment.hpMax;
    var index = Mathf.FloorToInt((1.0f - hpPercent) * sprites.Length);
    if (index >= sprites.Length) {
      index = sprites.Length - 1;
    }
    var chosenSprite = sprites[index];
    if (sr.sprite != chosenSprite) {
      sr.sprite = chosenSprite;
    }
  }
}
