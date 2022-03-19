using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : ProjectileControllerBase {
  Rigidbody2D rb2d;
  private SpriteRenderer spriteRenderer;
  [SerializeField]
  [ReadOnly]
  float distanceTravelled = 0;

  Vector2 lastPos;
  // Start is called before the first frame update
  public override void Start() {
    base.Start();
    rb2d = GetComponent<Rigidbody2D>();
    spriteRenderer = GetComponent<SpriteRenderer>();

    var angle = this.transform.rotation.eulerAngles.z;
    rb2d.velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * projectile.baseSpeed;
    rb2d.SetRotation(angle);
    lastPos = rb2d.position;
    var aso = GetComponent<AudioSource>();
    aso.enabled = !projectile.isRay;
    if (aso.enabled) {
      // lower volume for smaller shots
      var volumeScalar = Mathf.Clamp(Util.MapLinear(projectile.damage, 0, 8, 0.3f, 1), 0, 1);
      aso.volume *= volumeScalar;
    }

    // small projectiles start to fade out
    var newColor = spriteRenderer.color;
    newColor.a = projectile.damage < 2 ? projectile.damage / 2 : 1;
    spriteRenderer.color = newColor;

    // lower damage = smaller projectile
    transform.localScale *= Mathf.Sqrt(projectile.damage / 2);
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
    if (ProcessHit(col, transform.position.xy())) {
      Destroy(gameObject);
    }
  }
}
