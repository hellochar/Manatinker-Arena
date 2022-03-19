using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetController : MonoBehaviour {
  public ParticleSystem ps;
  public FragmentController fc;
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    var emission = ps.emission;
    var jet = fc.fragment as Jet;
    if (jet == null) {
        return;
    }
    if (jet.isActivated) {
      emission.rateOverTimeMultiplier = 15;
    } else {
      emission.rateOverTimeMultiplier = 0;
    }
  }
}
