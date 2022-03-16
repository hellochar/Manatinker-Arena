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
    var timeAlive = (Time.time - start) - (delay * 0.05f + 1f);
    if (timeAlive < 0) {
      return;
    }
    transform.Rotate(0, 0, 12);
    transform.position = Vector3.Lerp(transform.position, pc.transform.position, 0.03f * timeAlive);
    transform.position = Vector3.MoveTowards(transform.position, pc.transform.position, 0.03f * timeAlive);
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
