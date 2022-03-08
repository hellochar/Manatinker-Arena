using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentController : MonoBehaviour {
  [NonSerialized]
  public Fragment fragment;

  internal void Init(Fragment fragment) {
    fragment.controller = this;
  }

  internal void Removed() {
    Destroy(gameObject);
    fragment.controller = null;
  }
}
