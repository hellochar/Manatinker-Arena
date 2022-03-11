using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour {
  public Sprite[] sprites;
  public FragmentController fc;
  public SpriteRenderer sr;

  void Start() {
    // scale up until we are at least the bounding size of the fragment's sprite
    var fragmentSprite = fc.spriteRenderer;
    var fragmentSpriteBoundsSize = fragmentSprite.bounds.size.xy();
    var cracksBoundsSize = sr.bounds.size.xy();
    var boundsRatio = fragmentSpriteBoundsSize / cracksBoundsSize;
    var maxRatio = Mathf.Max(boundsRatio.x, boundsRatio.y);
    transform.localScale = (transform.localScale.xy() * maxRatio).z(transform.localScale.z);
    sr.sortingOrder = fc.mask.frontSortingOrder;
  }

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
