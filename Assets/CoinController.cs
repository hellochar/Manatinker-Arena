using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// fly towards player
public class CoinController : MonoBehaviour {
  private FragmentController pc;
  private int delay;

  void Start() {
    this.pc = GameModel.main.player.controller;
  }

  // Update is called once per frame
  void Update() {
    if (Time.time - start < delay * 0.02f) {
      return;
    }
    transform.Rotate(0, 0, 12);
    transform.position = Vector3.Lerp(transform.position, pc.transform.position, 0.01f);
    transform.position = Vector3.MoveTowards(transform.position, pc.transform.position, 0.01f);
    if ((transform.position - pc.transform.position).sqrMagnitude < 0.1f) {
      Destroy(gameObject);
    }
  }

  public float start;
  internal void setDelay(int i) {
    this.delay = i;
    start = Time.time;
  }
}
