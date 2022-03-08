using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {
  [SerializeField]
  [ReadOnly]
  Projectile p;
  Rigidbody2D rb2d;

  [SerializeField]
  [ReadOnly]
  float distanceTravelled = 0;

  public void Init(Projectile p) {
    this.p = p;
  }

  Vector2 lastPos;
  // Start is called before the first frame update
  void Start() {
    rb2d = GetComponent<Rigidbody2D>();
    var angle = this.transform.rotation.eulerAngles.z;
    rb2d.velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * p.baseSpeed;
    rb2d.SetRotation(angle);
    lastPos = rb2d.position;
  }

  // Update is called once per frame
  void Update() {
    var newPos = rb2d.position;
    distanceTravelled += (newPos - lastPos).magnitude;
    lastPos = newPos;
    if (distanceTravelled > p.maxDistance) {
      Destroy(gameObject);
    }
  }

  // void OnCollisionEnter2D(Collision2D collision) {
  //     Destroy(gameObject);
  // }

  void OnTriggerEnter2D(Collider2D col) {
    Destroy(gameObject);
  }
}
