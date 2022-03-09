using System;
using UnityEngine;
using UnityEngine.Splines;

public class WireController : MonoBehaviour {
  public SplineContainer splineContainer;
  Wire wire;

  internal void Init(Wire w) {
    this.wire = w;
    w.controller = this;
    // Update();
  }

  void Update() {
    var knot0 = splineContainer.Spline[0];
    var outTransform = wire.from.controller.output.transform;
    knot0.Position = outTransform.position;
    var outRotation = outTransform.eulerAngles.z * Mathf.Deg2Rad;
    knot0.TangentOut = new Unity.Mathematics.float3(Mathf.Cos(outRotation), Mathf.Sin(outRotation), 0) * 0.5f;
    splineContainer.Spline[0] = knot0;

    var knot1 = splineContainer.Spline[1];
    var inTransform = wire.to.controller.input.transform;
    knot1.Position = inTransform.position;
    var inRotation = inTransform.eulerAngles.z * Mathf.Deg2Rad;
    knot1.TangentIn = new Unity.Mathematics.float3(-Mathf.Cos(inRotation), -Mathf.Sin(inRotation), 0) * 0.5f;
    splineContainer.Spline[1] = knot1;
  }

  internal void Removed() {
    Destroy(gameObject);
  }
}
