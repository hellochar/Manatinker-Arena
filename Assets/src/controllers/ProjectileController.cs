using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {
  [SerializeField]
  [ReadOnly]
  public Projectile projectile;
  Rigidbody2D rb2d;

  [SerializeField]
  [ReadOnly]
  float distanceTravelled = 0;

  public void Init(Projectile p) {
    this.projectile = p;
  }

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
    distanceTravelled += (newPos - lastPos).magnitude;
    lastPos = newPos;
    if (distanceTravelled > projectile.maxDistance) {
      Destroy(gameObject);
    }
  }

  // projectile is not a collider so this will never happen
  // void OnCollisionEnter2D(Collision2D col) { }

  void OnTriggerEnter2D(Collider2D col) {
    if (col.gameObject.name == "Clickthrough") {
      // ignore
      return;
    }
    // Debug.Log("projectile: trigger enter " + col.gameObject);
    var hitFC = col.gameObject.GetComponentInParent<FragmentController>();
    // we've hit a fragment, process it
    if (hitFC) {
      hitFC.fragment.Hit(projectile);
    }
    Destroy(gameObject);
    // col.gameObject.GetComponentInParent<
  }
}
