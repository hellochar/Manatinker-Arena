using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpSelectedController : MonoBehaviour {
  public TMPro.TMP_Text text;
  private Button button;

  // Start is called before the first frame update
  void Start() {
    button = GetComponent<Button>();
    text = GetComponentInChildren<TMPro.TMP_Text>();
  }

  Player player => GameModel.main.player;
  int cost => selected.fragment.costToUpgrade;

  public FragmentController selected => GameModelController.main.editModeController.inputState.selected;

  void Update() {
    if (selected != null) {
      button.enabled = player.gold >= cost;
      text.text = $"Z - Heal & Level Up {selected.fragment.GetType().Name} to {selected.fragment.level + 1} ({cost} gold)";
      if (Input.GetKeyDown(KeyCode.Z)) {
          TryLevelUp();
      }
    }
  }

  public void TryLevelUp() {
    if (player.gold < cost) {
      LevelUpPlayerController.ShowText("Not enough gold!");
    } else {
      player.gold -= cost;
      selected.fragment.LevelUp();
      LevelUpPlayerController.ShowText($@"
{selected.fragment.DisplayName}

Full Heal.
+20% max HP.
+20% max Mana.
+20% Mana inflow and outflow speed.
".Trim());
    }
  }
}
