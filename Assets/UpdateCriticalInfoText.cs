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
    var engine = player.Children.Find(f => f is Engine);

    var engineHP = engine.Hp;
    var engineHPMax = engine.hpMax;
    var speed = player.speed;
    var speedBase = player.baseSpeed;
    var totalWeight = player.totalWeight;
    var encumbrance = player.encumbranceThreshold;

    // e.g. 3 / 1 => 3 - 1 = 200% slower
    float overloadedAmount = totalWeight / encumbrance;

    var overweightText = overloadedAmount > 1 ? $"({Mathf.RoundToInt((overloadedAmount - 1) * 100)}% overweight)" : "";

    text.text = $@"
Weight: {totalWeight}/{encumbrance} {overweightText}
Speed: {speed.ToString("#0.#")}
    ".Trim();
  }
}
