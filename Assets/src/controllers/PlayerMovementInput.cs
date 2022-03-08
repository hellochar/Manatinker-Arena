using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementInput : MonoBehaviour {
  public float velocity = 25;

  // Start is called before the first frame update
  Rigidbody2D rb2d;
  void Start() {
    rb2d = GetComponent<Rigidbody2D>();
    Camera.main.GetComponent<CameraFollowPlayer>().Player = this.gameObject;
  }

  float lastTime = 0;

  // Update is called once per frame
  void Update() {
    var dx = Input.GetAxis("Horizontal");
    var dy = Input.GetAxis("Vertical");
    rb2d.velocity = new Vector2(dx, dy) * velocity;

    var s = new Vector2(Screen.width, Screen.height) / 2f;
    var mouseOffset = Input.mousePosition.xy() - s;


    var currentAngle = rb2d.rotation;
    var targetAngle = Vector2.SignedAngle(Vector2.right, mouseOffset);
    // rb2d.SetRotation(angle);
    // rb2d.SetRotation(Mathf.MoveTowardsAngle(currentAngle, angle, 360 * Time.deltaTime));
    var newAngle = Mathf.LerpAngle(currentAngle, targetAngle, 10f * Time.deltaTime);
    newAngle = Mathf.MoveTowardsAngle(newAngle, targetAngle, 360 * Time.deltaTime);
    rb2d.SetRotation(newAngle);

    // var now = Time.time;
    // var diff = now - lastTime;
    // lastTime = now;
    // Debug.Log((diff * 10000).ToString("##.#"));
    // Debug.Log(Time.deltaTime);
  }
}
