using UnityEngine;

public class CreatureController : MonoBehaviour {
  public FragmentController fc;
  Fragment fragment => fc.fragment;
  public Rigidbody2D rb2d;

  void Start() {
    rb2d = GetComponent<Rigidbody2D>();
  }

  // happens when you hit another component
  void OnCollisionEnter2D(Collision2D collision) {
    // in rare cases this can happen before start?
    var hitFc = collision.collider.gameObject.GetComponentInParent<FragmentController>();
    var myFc = collision.otherCollider.gameObject.GetComponentInParent<FragmentController>();
    if (hitFc && myFc) {
      Debug.Log(fragment.DisplayName + "'s " + myFc.fragment.DisplayName + " collided with " + hitFc.fragment.DisplayName);
    }
  }

  // happens when projectiles (which are triggers) hit 
  // we cannot from here detect which fragment was hit
  // but we do know which thing hit us
  // void OnTriggerEnter2D(Collider2D col) {
  //   Debug.Log(fragment.GetType() + ": trigger enter " + col.gameObject);
  // }
}