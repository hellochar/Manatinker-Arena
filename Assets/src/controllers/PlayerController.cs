using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

  public Rigidbody2D rb2d;
  public FragmentController fragmentController;
  public Player player => (Player)fragmentController.fragment;
  void Start() {
    if (rb2d == null) {
      rb2d = GetComponent<Rigidbody2D>();
    }
    if (fragmentController == null) {
      fragmentController = GetComponent<FragmentController>();
    }
    Camera.main.GetComponent<CameraFollowPlayer>().Player = this.gameObject;
  }

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

  // void OnCollisionEnter2D(Collision2D collision) {
  //   Debug.Log("player: collision enter " + collision.gameObject);
  //   // Destroy(gameObject);
  // }

  // void OnTriggerEnter2D(Collider2D col) {
  //   Debug.Log("player: trigger enter " + col.gameObject);
  //   // if (col.gameObject.CompareTag("InLevel")) {
  //   //   var fragmentController = col.gameObject.GetComponent<FragmentController>();
  //   //   if (fragmentController != null) {
  //   //     // we've hit a fragment, deal damage
  //   //     Hit(fragmentController.fragment);
  //   //   }
  //   // }
  // }
}
