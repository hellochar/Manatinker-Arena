using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadController : MonoBehaviour
{
  public ParticleSystem ps;
  public FragmentController fc;
  public float multiplier = 50;
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    var emission = ps.emission;
    var jumpPad = fc.fragment as JumpPad;
    if (jumpPad == null) {
        return;
    }
    emission.rateOverTimeMultiplier = jumpPad.lastMovementAmount * multiplier;
  }
}
