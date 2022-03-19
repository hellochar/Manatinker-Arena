using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverpadController : MonoBehaviour {
  // Start is called before the first frame update
  public ParticleSystem ps;
  public FragmentController fc;
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    var emission = ps.emission;
    if (fc.fragment.weight < 0) {
      emission.rateOverDistanceMultiplier = 2;
    } else {
      emission.rateOverDistanceMultiplier = 0;
    }
  }
}
