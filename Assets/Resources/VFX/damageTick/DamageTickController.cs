using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTickController : MonoBehaviour {
  public float deathTime = 0.5f;
  public SpriteRenderer sr;
  public AudioSource aso;

  public void Init(Projectile p) {
    var scale = Mathf.Sqrt(p.damage) * 10;
    transform.localScale = new Vector3(scale, scale, 1);
    if (p.owner is Enemy) {
      sr.color = new Color32(255, 67, 67, 255);
    }
    var tDamage = Util.Temporal(p.damage);
    if (tDamage < 1) {
      aso.enabled = false;
    } else {
      aso.volume *= Mathf.Clamp(Util.MapLinear(p.damage, 0, 2, 0, 1), 0, 1f);
    }
    // if (fragment.isPlayerOwned) {
    //   dmgTick.GetComponent<AudioSource>().enabled = true;
    // } else {
    //   dmgTick.GetComponent<AudioSource>().enabled = false;
    // }
  }

  // Start is called before the first frame update
  void Start() {
    Destroy(gameObject, deathTime);
  }

  void Update() {
    sr.color = Color.Lerp(sr.color, Color.clear, 0.02f);
    transform.localScale *= 0.95f;
  }
}
