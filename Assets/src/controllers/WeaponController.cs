using UnityEngine;

public class WeaponController : MonoBehaviour {
  private FragmentController fc;
  public Weapon weapon => (Weapon)fc.fragment;
  public GameObject projectilePrefab;
  public Transform spawnPoint;

  void Start() {
    fc = GetComponent<FragmentController>();
    weapon.OnShootProjectile += HandleShootProjectile;
  }

  void Update() {
    if (weapon.CanActivate() && weapon.isPlayerOwned && Input.GetMouseButtonDown(0)) {
      weapon.Activate();
    }
  }

  void HandleShootProjectile(Projectile p) {
    var projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<ProjectileController>();
    projectile.Init(p);
  }
}
