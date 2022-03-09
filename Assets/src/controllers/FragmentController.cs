using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentController : MonoBehaviour {
  [NonSerialized]
  public Fragment fragment;
  public SpriteRenderer sr;

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
    if (sr == null) {
      sr = transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }
  }

  public virtual void Update() {
    sr.color = Color.Lerp(Color.black, Color.white, fragment.Mana / fragment.manaMax);
  }

  internal void Removed() {
    Destroy(gameObject);
    fragment.controller = null;
  }
}
