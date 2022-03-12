using UnityEngine;

public abstract class ProjectileControllerBase : MonoBehaviour {
  [SerializeField]
  [ReadOnly]
  public Projectile projectile;

  [SerializeField]
  [ReadOnly]
  public float timeSpawned = 0;
  public float elapsed => Time.time - timeSpawned;

  public void Init(Projectile p) {
    this.projectile = p;
    timeSpawned = Time.time;
  }

  public void ProcessHit(Collider2D col) {
    if (col.gameObject.name == "Clickthrough") {
      // ignore
      return;
    }
    if (col.gameObject.CompareTag("Projectile")) {
      return;
    }
    // Debug.Log("projectile: trigger enter " + col.gameObject);
    var hitFC = col.gameObject.GetComponentInParent<FragmentController>();
    // we've hit a fragment, process it
    if (hitFC) {
      hitFC.fragment.Hit(projectile);
    }
  }
}
