using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentController : MonoBehaviour {
  [NonSerialized]
  public Fragment fragment;
  public SpriteRenderer manaCover;

  public virtual void Init(Fragment fragment) {
    this.fragment = fragment;
    fragment.controller = this;
    if (fragment.owner != null) {
      transform.SetParent(fragment.owner.controller.transform);
    }
    if (fragment is PlayerFragment) {
      transform.position = new Vector3(5, 5, transform.position.z);
    } else {
      transform.localPosition = fragment.builtinOffset.z(transform.position.z);
      transform.localRotation = Quaternion.Euler(0, 0, fragment.builtinAngle);
    }
  }

  void Start() {
    if (manaCover != null) {
      var sr = transform.Find("Sprite").GetComponent<SpriteRenderer>();
      manaCover.sprite = sr.sprite;
    }
  }

  public virtual void Update() {
    if (manaCover != null) {
      manaCover.material.SetFloat("_Percentage", fragment.Mana / fragment.manaMax);
    }
    // sr.color = Color.Lerp(Color.black, Color.white, fragment.Mana / fragment.manaMax);
  }

  internal void Removed() {
    Destroy(gameObject);
    fragment.controller = null;
  }
}
