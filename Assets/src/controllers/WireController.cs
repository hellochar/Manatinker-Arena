using System;
using System.Collections;
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

  void Start() {
    lineRenderer.endColor = lineRenderer.startColor = FragmentController.NO_FLOW_COLOR;
    StartCoroutine(WireInAnimation());
    var aso = GetComponent<AudioSource>();
    // only on real wires
    aso.enabled = wire.to != null && wire.from != null;
  }

  private IEnumerator WireInAnimation() {
    float duration = 0.5f;
    float endThickness = lineRenderer.startWidth;
    float startThickness = endThickness * 15;
    var start = Time.time;
    lineRenderer.startWidth = lineRenderer.endWidth = startThickness;
    float t;
    do {
      t = Mathf.Clamp((Time.time - start) / duration, 0, 1);
      // lineRenderer.startWidth = lineRenderer.endWidth = Mathf.Lerp(startThickness, endThickness, t);
      lineRenderer.startWidth = lineRenderer.endWidth = Mathf.Lerp(lineRenderer.startWidth, endThickness, 0.02f);
      yield return new WaitForEndOfFrame();
    } while (t < 1);
    lineRenderer.startWidth = lineRenderer.endWidth = endThickness;
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

    float flowRate = wire.lastFlow / GameModel.main.dt;
    float lerpAmount = wire.to == null ? 1 : flowRate * FragmentController.HES * 2;

    var targetColor = Color.Lerp(FragmentController.NO_FLOW_COLOR, FragmentController.FULL_FLOW_COLOR, lerpAmount);
    lineRenderer.endColor = lineRenderer.startColor = Color.Lerp(lineRenderer.endColor, targetColor, 0.05f);
  }

  internal void Removed() {
    Destroy(gameObject);
  }
}
