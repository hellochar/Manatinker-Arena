using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTextController : MonoBehaviour {
  float timeSurvived;
  int roundsSurvived;
  void Start() {
    roundsSurvived = GameModel.main.round;
    timeSurvived = GameModel.main.time;
    TimeSpan time = TimeSpan.FromSeconds(timeSurvived);

    //here backslash is must to tell that colon is
    //not the part of format, it just a character that we want in output
    string minStr = time.ToString(@"mm");
    string secStr = time.ToString(@"ss");

    var text = GetComponent<TMPro.TMP_Text>();
    text.text = $@"Your mana returns to the Earthen Mother...

Survived {roundsSurvived} rounds over {minStr} minutes {secStr} seconds.";
  }
}
