using System;
using UnityEngine;
using UnityEngine.Splines;

public class WireController : MonoBehaviour {
  public SplineContainer splineContainer;
  public LineRenderer lineRenderer;
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
    float inRotation;
    if (wire.to == null) {
      knot1.Position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      inRotation = (
        new Vector2(knot0.Position.x, knot0.Position.y) - 
        new Vector2(knot1.Position.x, knot1.Position.y)
      ).angleDeg() * Mathf.Deg2Rad;
    } else {
      var inTransform = wire.to.controller.input?.transform ?? wire.to.controller.transform;
      knot1.Position = inTransform.position;
      // var inRotation = inTransform.localPosition.xy().angleDeg() * Mathf.Deg2Rad;
      inRotation = inTransform.eulerAngles.z * Mathf.Deg2Rad;
    }
    knot1.TangentIn = new Unity.Mathematics.float3(-Mathf.Cos(inRotation), -Mathf.Sin(inRotation), 0) * 0.5f;
    splineContainer.Spline[1] = knot1;

    float flowRate = wire.lastFlow / Time.deltaTime;
    float lerpAmount = flowRate / 2;

    var targetColor = Color.Lerp(Color.black, Color.white, lerpAmount);
    lineRenderer.endColor = lineRenderer.startColor = Color.Lerp(lineRenderer.endColor, targetColor, 0.1f);
  }

  internal void Removed() {
    Destroy(gameObject);
  }
}
