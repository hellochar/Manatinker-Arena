using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {
  public GameObject Player;
  public float mouseMovement = 2f;
  // public float nearModeFov = 30f;
  // float originalFov;
  public float nearModeProjectionSize => Mathf.Min(
    GameModel.main?.player?.influenceRadius * 1.2f + 1 ?? 4,
    this.originalProjectionSize);
  float originalProjectionSize;
  public float lerpRate = 4f;
  private new Camera camera;

  // Start is called before the first frame update
  void Start() {
    camera = GetComponent<Camera>();
    // originalFov = camera.fieldOfView;
    originalProjectionSize = camera.orthographicSize;
  }

  // Update is called once per frame
  void Update() {
    if (Input.GetKeyDown(KeyCode.E)) {
      if (GameModelController.main.isEditMode) {
        GameModelController.main.ExitEditMode();
      } else {
        GameModelController.main.EnterEditMode();
      }
    }
    if (Player == null) {
      return;
    }
    var targetPosition = Player.transform.position.xy();

    if (!GameModelController.main.isEditMode) {
      var screenCenter = new Vector2(Screen.width, Screen.height) / 2f;
      // -1 -> 1
      var mouseOffset = (Input.mousePosition.xy() - screenCenter) / screenCenter;
      targetPosition += mouseOffset * mouseMovement;
    }

    float targetProjectionSize = GameModelController.main.isEditMode ? nearModeProjectionSize : originalProjectionSize;
    camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetProjectionSize, 0.1f);
    // float targetFov = nearMode ? nearModeFov : originalFov;
    // camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFov, 0.2f);

    // if (Vector3.Distance(diffCurrent, diffTarget) > 0.01f) {
    //     diffCurrent = Vector3.Lerp(diffCurrent, diffTarget, 0.2f);
    // transform.position = Vector3.Lerp(transform.position, targetPosition, 0.2f);
    // }
    // var dist = Vector3.Distance(Player.transform.position.xy(), transform.position.xy());
    // var lerpRate = Util.MapLinear(Mathf.Clamp(dist, 1f, 5f), 1f, 5f, 1f, 5f);
    var newPosition = transform.position.xy();
    newPosition = Vector2.Lerp(newPosition, targetPosition, lerpRate * Time.deltaTime);
    transform.position = newPosition.z(transform.position.z);
  }

  internal void Teleport() {
    Camera.main.transform.position = Player.transform.position.xy().z(Camera.main.transform.position.z);
  }
}

