using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreController : MonoBehaviour {
  public Fragment fragment;
  public float fuel = 500;
  public float fuelToManaRatio = 10;

  // Start is called before the first frame update
  void Start() {
    fragment = GetComponent<Fragment>();
    fragment.Update = MyUpdate;
  }

  // Update is called once per frame
  void MyUpdate() {
    var manaToAdd = Time.deltaTime * fuelToManaRatio;
    if (fragment.Mana + manaToAdd <= fragment.manaMax) {
      fuel -= Time.deltaTime;
      fragment.ChangeMana(manaToAdd);
    }
  }
}
