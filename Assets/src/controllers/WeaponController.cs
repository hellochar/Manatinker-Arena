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
    if (weapon is IActivatable a) {
      var activationCheck = weapon.isHold ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
      if (a.CanActivate() && weapon.isPlayerOwned && activationCheck) {
        a.Activate();
      }
    }
  }

  void HandleShootProjectile(Projectile p) {
    var rotation = spawnPoint.rotation;
    if (p.angleSpread != 0) {
      rotation = rotation * Quaternion.Euler(0, 0, Random.Range(-p.angleSpread / 2, p.angleSpread / 2));
    }
    var projectile = Instantiate(projectilePrefab, spawnPoint.position, rotation).GetComponent<ProjectileControllerBase>();
    projectile.Init(p);
  }
}
