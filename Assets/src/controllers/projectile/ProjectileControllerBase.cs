using UnityEngine;

public abstract class ProjectileControllerBase : MonoBehaviour {
  [SerializeField]
  [ReadOnly]
  public Projectile projectile;

  [SerializeField]
  [ReadOnly]
  public float timeSpawned = 0;
  public float elapsed => Time.time - timeSpawned;

  public virtual void Init(Projectile p) {
    this.projectile = p;
    timeSpawned = Time.time;
  }

  public virtual void Start() {
    var count = Util.Temporal(projectile.damage);
    if (count > 0) {
      var fx = Instantiate(VFX.Get("projectileStart"), transform.position, transform.rotation);
      var ps = fx.GetComponent<ParticleSystem>();
      var emission = ps.emission;
      var burst0 = emission.GetBurst(0);
      burst0.count = count;
      var main = ps.main;
      emission.SetBurst(0, burst0);
    }
  }

  public bool ProcessHit(Collider2D col, Vector2 pos) {
    if (col.gameObject.CompareTag("Projectile")) {
      return false;
    }
    var hitWall = col.gameObject.CompareTag("Wall");
    if (hitWall) {
      return true;
    }
    // Debug.Log("projectile: trigger enter " + col.gameObject);
    var hitFC = col.gameObject.GetComponentInParent<FragmentController>();
    // we've hit a fragment, process it
    if (hitFC) {
      if (projectile.ignoreOwner && hitFC.fragment.owner == projectile.owner) {
        return false;
      }
      hitFC.fragment.Hit(projectile, pos);
      return true;
    }
    return false;
  }
}
