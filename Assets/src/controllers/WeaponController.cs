using UnityEngine;

public class WeaponController : MonoBehaviour {
  private FragmentController fc;
  public Weapon weapon => (Weapon)fc.fragment;
  public GameObject projectilePrefab;
  public Transform spawnPoint;
  AudioSource aso;
  public AudioSource ShootSound;

  void Start() {
    fc = GetComponent<FragmentController>();
    weapon.OnShootProjectile += HandleShootProjectile;
    if (aso == null) {
      aso = GetComponent<AudioSource>();
    }
  }

  void Update() {
    if (weapon is Laser laser) {
      var playSound = ((IActivatable)laser).PlayerInputCheck() && laser.CanActivate();
      SetAudioActive(playSound);
    } else if (weapon is Spike spike) {
      var playSound = ((IActivatable)spike).PlayerInputCheck() && spike.CanActivate() && spike.owner != null;
      if (playSound) {
        aso?.Play();
      }
    }
  }

  public void SetAudioActive(bool active) {
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
    if (ShootSound != null) {
      // lasers
      var intensity = p.damage / 60;
      ShootSound.volume = Util.MapLinear(intensity * intensity, 0, 1, 0.2f, 1f);
      ShootSound.pitch = Util.MapLinear(intensity * intensity, 0, 1, 0.2f, 1.5f);
      ShootSound.Play();
    }
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
