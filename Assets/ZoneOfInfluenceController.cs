using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneOfInfluenceController : MonoBehaviour {
  public PlayerController pc;

  public LineRenderer lineRenderer;
  float lastRadius = -1;
  private Vector3[] positions = new Vector3[63];
  private AnimationCurve curve;

  void Start() {
    // curve = new AnimationCurve();
    // for (int i = 0; i < positions.Length; i++) {
    //   var isUp = i % 2 == 0;
    //   curve.AddKey(new Keyframe(i / (positions.Length + 1.0f), isUp ? 1 : 0, 1, 0));
    //   // curve.AddKey(i / (positions.Length + 1.0f) + 0.00001f, isUp ? 0 : 1);
    // }
    // lineRenderer.widthCurve = curve;
  }

  // Update is called once per frame
  void Update() {
    lineRenderer.enabled = GameModelController.main.isEditMode;
    if (pc.player.influenceRadius != lastRadius) {
      RebuildLineRenderer(pc.player.influenceRadius);
    }
    transform.localRotation = Quaternion.Euler(0, 0, transform.localEulerAngles.z + 0.01f);
  }

  private void RebuildLineRenderer(float radius) {
    lineRenderer.positionCount = positions.Length;

    for(var i = 0; i < positions.Length; i++) {
      // loop makes it +1
      var angle = i * Mathf.PI * 2f / (positions.Length + 1);
      positions[i] = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
    }
    lineRenderer.SetPositions(positions);

    lastRadius = radius;
  }
}
