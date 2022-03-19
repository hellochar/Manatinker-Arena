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
    if (weapon is Laser a) {
      var playSound = ((IActivatable)a).PlayerInputCheck() && a.CanActivate();
      SetAudioActive(playSound);
    }
  }

  AudioSource aso;
  public void SetAudioActive(bool active) {
    if (aso == null) {
      aso = GetComponent<AudioSource>();
    }
    if (aso == null) {
      return;
    }
    if (active && !aso.isPlaying) {
      aso.Play();
    } else if (!active && aso.isPlaying) {
      aso.Pause();
    }
  }

  void HandleShootProjectile(Projectile p) {
    if (p.fragment == null) {
      p.fragment = weapon;
    }
    if (p.owner == null) {
      p.owner = weapon.owner;
    }
    var rotation = spawnPoint.rotation;
    if (p.angleSpread != 0) {
      rotation = rotation * Quaternion.Euler(0, 0, Random.Range(-p.angleSpread / 2, p.angleSpread / 2));
    }
    var projectile = Instantiate(projectilePrefab, spawnPoint.position, rotation).GetComponent<ProjectileControllerBase>();
    projectile.Init(p);
  }
}
