using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCriticalInfoText : MonoBehaviour {
  TMPro.TMP_Text text;
  // Start is called before the first frame update
  void Start() {
    text = GetComponent<TMPro.TMP_Text>();
  }

  // Update is called once per frame
  void Update() {
    var player = GameModel.main.player;
    var engine = player.children.Find(f => f is Engine);

    var engineHP = engine.Hp;
    var engineHPMax = engine.hpMax;
    var speed = player.speed;
    var speedBase = player.baseSpeed;
    var weight = player.totalWeight;

    text.text = $@"
Engine HP: {engineHP}/{engineHPMax}
Speed: {speed} / {speedBase}
Weight:  {weight}
    ".Trim();
  }
}
