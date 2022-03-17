using UnityEngine;

public class DaggerController : MonoBehaviour {
  private FragmentController fc;
  public Weapon dagger => (Weapon)fc.fragment;
  public AudioSource aso;

  void Start() {
    fc = GetComponent<FragmentController>();
    dagger.OnShootProjectile += HandleShootProjectile;
  }

  void HandleShootProjectile(Projectile p) {
    aso.Play();
    aso.pitch = Random.Range(0.9f, 1/0.9f);
    aso.volume = Random.Range(0.15f, 0.25f);
  }
}
