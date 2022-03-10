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

  void OnCollisionEnter2D(Collision2D col) {
    Debug.Log("projectile: collision enter " + col.gameObject);
    // if (col.gameObject.CompareTag("InLevel")) {
    //   var fragmentController = col.gameObject.GetComponent<FragmentController>();
    //   if (fragmentController != null) {
    //     // we've hit a fragment, deal damage
    //     Hit(fragmentController.fragment);
    //   }
    // }
  }

  void OnTriggerEnter2D(Collider2D col) {
    // Debug.Log("projectile: trigger enter " + col.gameObject);
  }

  private void Hit(Fragment fragment) {
    fragment.Hit(projectile);
    Destroy(gameObject);
  }
}
