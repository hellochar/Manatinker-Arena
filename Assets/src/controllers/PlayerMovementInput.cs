using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementInput : MonoBehaviour {

  // Start is called before the first frame update
  public Rigidbody2D rb2d;
  FragmentController fragmentController;
  Player player => (Player)fragmentController.fragment;
  void Start() {
    rb2d = GetComponent<Rigidbody2D>();
    fragmentController = GetComponent<FragmentController>();

    Camera.main.GetComponent<CameraFollowPlayer>().Player = this.gameObject;
  }

  float lastTime = 0;

  // Update is called once per frame
  void Update() {
    if (GameModel.main.isEditMode) {
      return;
    }

    var dx = Input.GetAxis("Horizontal");
    var dy = Input.GetAxis("Vertical");
    player.setVelocityDirection(new Vector2(dx, dy));

    var s = new Vector2(Screen.width, Screen.height) / 2f;
    var mouseOffset = Input.mousePosition.xy() - s;
    var targetAngle = Vector2.SignedAngle(Vector2.right, mouseOffset);
    player.setRotation(targetAngle);
  }
}
