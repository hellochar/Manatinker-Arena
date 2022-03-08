using UnityEngine;

public class WeaponController : FragmentController {
  public Weapon weapon => (Weapon)fragment;
  public GameObject projectilePrefab;
  public Transform spawnPoint;

  void Start() {
    weapon.OnShootProjectile += HandleShootProjectile;
  }

  void HandleShootProjectile(Projectile p) {
    var projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<ProjectileController>();
    projectile.Init(p);
  }

  void Update() {

  }
}
