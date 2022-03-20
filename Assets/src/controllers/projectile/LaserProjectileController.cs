using UnityEngine;

public class LaserProjectileController : ProjectileControllerBase {
  public LineRenderer lineRenderer;

  public override void Init(Projectile p) {
    base.Init(p);
    if (p.lifeTime > 0) {
      Destroy(gameObject, p.lifeTime);
    }
  }

  public override void Start() {
    base.Start();
    CheckRayCast();
  }

  // destroy after one tick
  void Update() {
    if (projectile.lifeTime == 0) {
      Destroy(gameObject);
    }
  }

  void CheckRayCast() {
    lineRenderer.SetPosition(0, transform.position);

    var rotation = Util.fromDeg(transform.eulerAngles.z);
    var endpoint = transform.position.xy() + rotation * projectile.maxDistance;
    var hits = Physics2D.LinecastAll(transform.position.xy(), endpoint); //, LayerMask.GetMask("UI"));
    foreach(var hit in hits) {
      if (hit.collider != null) {
        var processed = ProcessHit(hit.collider, hit.point);
        if (processed) {
          endpoint = hit.point;
          break;
        }
      }
    }
    lineRenderer.SetPosition(1, endpoint);
  }
}