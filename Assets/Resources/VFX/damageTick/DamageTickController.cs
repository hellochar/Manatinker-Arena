using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTickController : MonoBehaviour {
  public float deathTime = 0.1f;
  private SpriteRenderer sr;

  // Start is called before the first frame update
  void Start() {
    Destroy(gameObject, deathTime);
    this.sr = GetComponent<SpriteRenderer>();
  }

  void Update() {
    sr.color = Color.Lerp(sr.color, Color.clear, 0.1f);
    transform.localScale *= 0.95f;
  }
}
