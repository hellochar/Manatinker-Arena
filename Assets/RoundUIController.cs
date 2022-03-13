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
  internal static RoundUIController main;
  public GameObject youWin;

  void Awake() {
    main = this;
  }

  void Update() {
    var round = GameModel.main.currentRound;
    nextRoundButton.gameObject.SetActive(round.state == GameRoundState.Preparing && !GameModelController.main.isEditMode);
    nextRoundButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Start Round " + (round.roundNumber + 1) + " / 10";
    if (round.state == GameRoundState.Active) {
      TimeSpan span = TimeSpan.FromSeconds(round.remaining);
      activeText.text = $"Round {round.roundNumber} - {span.ToString(@"mm\:ss")} remaining";
      activeBar.fillAmount = round.remaining / round.duration;
    } else if (round.state == GameRoundState.WaitingForClear) {
      activeText.text = $"Round {round.roundNumber} - {GameModel.main.enemies.Count} enemies left!";
    } else {
      activeBar.fillAmount = 0;
      activeText.text = "";
    }
  }

  public void GoNextRound() {
    Instantiate(VFX.Get("bigSweep"));
    GameModel.main.GoNextRound();
    var roundMarker = Instantiate(roundMarkerPrefab, transform);
    roundMarker.GetComponentInChildren<TMPro.TMP_Text>().text = "Round " + GameModel.main.currentRound.roundNumber;
  }

  internal void RoundFinished() {
    if (GameModel.main.currentRound.roundNumber == 10) {
      GameModel.main.playerHasWon = true;
      youWin.SetActive(true);
    }
    var roundMarker = Instantiate(roundMarkerPrefab, transform);
    roundMarker.GetComponentInChildren<TMPro.TMP_Text>().text = "Round " + GameModel.main.currentRound.roundNumber + " complete!";
  }

  public void keepPlaying() {
    youWin.SetActive(false);
  }
}
