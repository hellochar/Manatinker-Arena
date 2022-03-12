using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundUIController : MonoBehaviour {
  public GameObject roundMarkerPrefab;
  public GameObject nextRoundButton;
  public Image activeBar;
  public TMPro.TMP_Text activeText;

  void Update() {
    var round = GameModel.main.currentRound;
    // nextRoundButton.gameObject.SetActive(round.state == GameRoundState.Preparing && !GameModelController.main.isEditMode);
    if (round.state == GameRoundState.Active) {
      TimeSpan span = TimeSpan.FromSeconds(round.elapsed);
      activeText.text = span.ToString(@"mm\:ss");
      activeBar.fillAmount = round.remaining / round.duration;
    } else if (round.state == GameRoundState.WaitingForClear) {
      activeText.text = $"{GameModel.main.enemies} enemies left!";
    } else {
      activeBar.fillAmount = 0;
      activeText.text = "";
    }
  }

  public void GoNextRound() {
    Instantiate(roundMarkerPrefab, transform);
    GameModel.main.GoNextRound();
  }
}
