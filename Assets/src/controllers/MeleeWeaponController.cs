using UnityEngine;

public class MeleeWeaponController : MonoBehaviour {
  private FragmentController fc;
  public Dagger dagger => (Dagger)fc.fragment;
  public GameObject projectilePrefab;

  void Start() {
    fc = GetComponent<FragmentController>();
  }

  void Update() {
    if (dagger is IActivatable a) {
      var activationCheck = dagger.isHold ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
      if (a.CanActivate() && dagger.isPlayerOwned && activationCheck) {
        a.Activate();
        // HandleShootProjectile()
      }
      // if (weapon.isHold) {
      //   var playSound = activationCheck && a.CanActivate();
      //   SetAudioActive(playSound);
      // }
    }
  }

  // void HandleShootProjectile(Projectile p) {
  //   if (p.owner == null) {
  //     p.owner = weapon.owner;
  //   }
  //   var rotation = spawnPoint.rotation;
  //   if (p.angleSpread != 0) {
  //     rotation = rotation * Quaternion.Euler(0, 0, Random.Range(-p.angleSpread / 2, p.angleSpread / 2));
  //   }
  //   var projectile = Instantiate(projectilePrefab, spawnPoint.position, rotation).GetComponent<ProjectileControllerBase>();
  //   var fx = Instantiate(VFX.Get("projectileStart"), spawnPoint.position, rotation);
  //   var ps = fx.GetComponent<ParticleSystem>();
  //   var emission = ps.emission;
  //   var burst0 = emission.GetBurst(0);
  //   burst0.count = Mathf.CeilToInt(p.damage);
  //   var main = ps.main;
  //   // main.startSize = main.startSize.constant * p.damage / 2f;
  //   emission.SetBurst(0, burst0);
  //   projectile.Init(p);
  // }
}
