using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCriticalInfoText : MonoBehaviour {
  TMPro.TMP_Text text;
  // Start is called before the first frame update
  void Start() {
    text = GetComponent<TMPro.TMP_Text>();
  }

  public Gradient gradient;

  // Update is called once per frame
  void Update() {
    var player = GameModel.main.player;
    var speed = player.speed;
    var totalWeight = player.totalWeight;
    var encumbrance = player.encumbranceThreshold;

    // e.g. 3 / 1 => 3 - 1 = 200% slower
    float overloadedAmount = totalWeight / encumbrance;

    var overweightText = overloadedAmount > 1 ? $"({Mathf.RoundToInt(overloadedAmount * 100)}%)" : "";

    var speedPercent = speed / player.baseSpeed;

    var extraSpeedText = speedPercent < 1 ? $"({(speedPercent * 100).ToString("##0")}%)" : "";

    var overloadedT = Mathf.Clamp((overloadedAmount - 1) / 3, 0, 1);
    var encumbranceColor = gradient.Evaluate(overloadedT);

    var weightText = $@"
{totalWeight} of {encumbrance}kg {overweightText}
    ".Trim().AddColor(encumbranceColor);

var speedText = $"Speed {player.speed.ToString("##0.##")} {extraSpeedText}".AddColor(encumbranceColor).Trim();

    text.text = $@"
<sprite=0> {player.gold}
{weightText} • {speedText}
".Trim();
  }
}
