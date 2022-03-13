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
    // enemies only deal 25% damage
    if (p.owner is Enemy) {
      p.damage /= 4;
    }
    this.projectile = p;
    timeSpawned = Time.time;
  }

  public bool ProcessHit(Collider2D col, Vector2 pos) {
    if (col.gameObject.CompareTag("Projectile")) {
      return false;
    }
    // Debug.Log("projectile: trigger enter " + col.gameObject);
    var hitFC = col.gameObject.GetComponentInParent<FragmentController>();
    // we've hit a fragment, process it
    if (hitFC) {
      hitFC.fragment.Hit(projectile, pos);
      return true;
    }
    var hitWall = col.gameObject.CompareTag("Wall");
    if (hitWall) {
      return true;
    }
    return false;
  }
}
