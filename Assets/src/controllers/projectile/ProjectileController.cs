using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : ProjectileControllerBase {
  Rigidbody2D rb2d;

  [SerializeField]
  [ReadOnly]
  float distanceTravelled = 0;

  Vector2 lastPos;
  // Start is called before the first frame update
  void Start() {
    rb2d = GetComponent<Rigidbody2D>();
    var angle = this.transform.rotation.eulerAngles.z;
    rb2d.velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * projectile.baseSpeed;
    rb2d.SetRotation(angle);
    lastPos = rb2d.position;
  }

  // Update is called once per frame
  void Update() {
    var newPos = rb2d.position;
    lastPos = newPos;
    if (projectile.lifeTime > 0 && elapsed > projectile.lifeTime) {
      Destroy(gameObject);
      return;
    }
    distanceTravelled += (newPos - lastPos).magnitude;
    if (projectile.maxDistance > 0 && distanceTravelled > projectile.maxDistance) {
      Destroy(gameObject);
    }
  }

  // projectile is not a collider so this will never happen
  // void OnCollisionEnter2D(Collision2D col) { }

  void OnTriggerEnter2D(Collider2D col) {
    if (ProcessHit(col)) {
      Destroy(gameObject);
    }
  }
}
