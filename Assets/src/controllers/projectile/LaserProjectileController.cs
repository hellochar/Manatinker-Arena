using UnityEngine;

public class LaserProjectileController : ProjectileControllerBase {
  public LineRenderer lineRenderer;
  public override void Start() {
    base.Start();
    CheckRayCast();
  }

  // destroy after one tick
  void Update() {
    Destroy(gameObject);
  }

  void CheckRayCast() {
    lineRenderer.SetPosition(0, transform.position);

    var rotation = Util.fromDeg(transform.eulerAngles.z);
    var endpoint = transform.position.xy() + rotation * projectile.maxDistance;
    var hit = Physics2D.Linecast(transform.position.xy(), endpoint); //, LayerMask.GetMask("UI"));
    if (hit.collider != null) {
      ProcessHit(hit.collider, hit.point);
      endpoint = hit.point;
    }
    lineRenderer.SetPosition(1, endpoint);
  }
}