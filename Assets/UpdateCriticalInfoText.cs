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
    var speed = player.speed;
    var totalWeight = player.totalWeight;
    var encumbrance = player.encumbranceThreshold;

    // e.g. 3 / 1 => 3 - 1 = 200% slower
    float overloadedAmount = totalWeight / encumbrance;

    var overweightText = overloadedAmount > 1 ? $"({Mathf.RoundToInt((overloadedAmount - 1) * 100)}% overweight)" : "";

    var speedPercent = speed / player.baseSpeed;

    var extraSpeedText = speedPercent < 1 ? $"({(speedPercent * 100).ToString("##0")}%)" : "";

    text.text = $@"
Gold: {player.gold}
Weight: {totalWeight}kg / {encumbrance}kg {overweightText}
Speed: {player.speed} {extraSpeedText}".Trim();
  }
}
