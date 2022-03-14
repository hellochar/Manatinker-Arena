using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayController : MonoBehaviour {
  public Image black;
  bool bIsLoading = false;
  public void OnPlay() {
    if (bIsLoading) {
      return;
    }
    bIsLoading = true;
    SceneManager.LoadSceneAsync("Scenes/Arena", LoadSceneMode.Single);
  }

  void Update() {
    if (bIsLoading) {
      black.color = Color.Lerp(black.color, Color.black, 0.5f);
    }
  }
}
