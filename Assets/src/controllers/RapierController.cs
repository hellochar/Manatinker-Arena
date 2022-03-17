using UnityEngine;

public class RapierController : MonoBehaviour {
  private FragmentController fc;
  public Rapier rapier => (Rapier)fc.fragment;
  public AudioSource aso;

  void Start() {
    fc = GetComponent<FragmentController>();
    rapier.OnSwing += HandleSwing;
  }

  void HandleSwing() {
    aso.Play();
    aso.pitch = Random.Range(0.9f, 1/0.9f);
    aso.volume = Random.Range(0.1f, 0.11f);
  }
}
