using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPlayerController : MonoBehaviour {
  public GameObject tempText;
  public TMPro.TMP_Text text;
  private Button button;
  // Start is called before the first frame update
  void Start() {
    button = GetComponent<Button>();
    text = GetComponentInChildren<TMPro.TMP_Text>();
  }

  Player player => GameModel.main.player;
  int cost => 5 + player.level;

  // Update is called once per frame
  void Update() {
    button.enabled = player.gold >= cost;
    text.text = $"Heal & Level Up to {player.level + 1} ({cost} gp)";
  }

  public void TryLevelUp() {
    if (player.gold < cost) {
      ShowText("Not enough gold!");
    } else {
      player.gold -= cost;
      player.LevelUp();
      ShowText($@"
Level {player.level}

Healed 50%!
Max HP +10!
Outflow Rate +2!
Speed +2!
Max weight +1!
Influence radius increased!
".Trim());
    }
  }

  GameObject existingText;
  private void ShowText(string v) {
    if (existingText != null) {
      Destroy(existingText);
    }
    existingText = Instantiate(tempText);
    existingText.GetComponentInChildren<TMPro.TMP_Text>().text = v;
    Destroy(existingText, v.Length / 10f);
  }
}
