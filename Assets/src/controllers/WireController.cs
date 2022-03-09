using System;
using UnityEngine;
using UnityEngine.Splines;

public class WireController : MonoBehaviour {
  public SplineContainer splineContainer;
  Wire wire;
  internal void Init(Wire w) {
    this.wire = w;
    w.controller = this;
    Update();
  }

  void Update() {
    var knot0 = splineContainer.Spline[0];
    knot0.Position = wire.from.controller.transform.position;
    splineContainer.Spline[0] = knot0;

    var knot1 = splineContainer.Spline[1];
    knot1.Position = wire.to.controller.transform.position;
    splineContainer.Spline[1] = knot1;
  }

  internal void Removed() {
    Destroy(gameObject);
  }
}
