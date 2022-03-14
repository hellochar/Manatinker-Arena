using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSound : MonoBehaviour {
  public AudioClip[] clips;
  // Start is called before the first frame update
  void Start() {
    var aso = GetComponent<AudioSource>();
    if (aso) {
        aso.clip = clips[Random.Range(0, clips.Length)];
        aso.Play();
    }
  }
}
