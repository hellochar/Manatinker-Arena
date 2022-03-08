using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentInfo : MonoBehaviour {
  public Fragment fragment;
  public TMPro.TMP_Text text;
  // Start is called before the first frame update
  void Start() {
    if (fragment == null) {
      fragment = GetComponentInParent<Fragment>();
    }
    text = GetComponent<TMPro.TMP_Text>();
  }

  // Update is called once per frame
  void Update() {
    text.text = fragment.ToString();
  }
}
