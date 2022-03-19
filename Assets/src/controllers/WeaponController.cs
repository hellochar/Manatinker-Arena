using UnityEngine;

public class WeaponController : MonoBehaviour {
  private FragmentController fc;
  public Weapon weapon => (Weapon)fc.fragment;
  public GameObject projectilePrefab;
  public Transform spawnPoint;
  public Transform[] extraSpawns;
  public AudioSource ShootSound;

  void Start() {
    fc = GetComponent<FragmentController>();
    weapon.OnShootProjectile += HandleShootProjectile;
  }

  void HandleShootProjectile(Projectile p) {
    if (ShootSound != null) {
      // lasers
      var intensity = p.damage / 60;
      ShootSound.volume = Util.MapLinear(intensity * intensity, 0, 1, 0.2f, 1f);
      ShootSound.pitch = Util.MapLinear(intensity * intensity, 0, 1, 0.2f, 1.5f);
      ShootSound.Play();
    }
    if (p.creator == null) {
      p.creator = weapon;
    }
    if (p.owner == null) {
      p.owner = weapon.owner;
    }

    spawn(p, spawnPoint);
    if (extraSpawns != null) {
      foreach(var otherPoint in extraSpawns)
      spawn(p, otherPoint);
    }
  }

  void spawn(Projectile p, Transform spawnPoint) {
    var rotation = spawnPoint.rotation;
    if (p.angleSpread != 0) {
      rotation = rotation * Quaternion.Euler(0, 0, Random.Range(-p.angleSpread / 2, p.angleSpread / 2));
    }
    var projectile = Instantiate(projectilePrefab, spawnPoint.position, rotation).GetComponent<ProjectileControllerBase>();
    projectile.Init(p);
  }
}
