using UnityEngine;

public class CreatureController : MonoBehaviour {
  FragmentController fc;
  Fragment fragment => fc.fragment;

  void Start() {
    fc = GetComponent<FragmentController>();
  }

  // happens when you hit another component
  void OnCollisionEnter2D(Collision2D collision) {
    Debug.Log(fragment.GetType() + ": collision enter " + collision.gameObject);
  }

  // happens when projectiles (which are triggers) hit 
  // we cannot from here detect which fragment was hit
  // but we do know which thing hit us
  // void OnTriggerEnter2D(Collider2D col) {
  //   Debug.Log(fragment.GetType() + ": trigger enter " + col.gameObject);
  // }
}