using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickInfoContainerController : MonoBehaviour {
  public GameObject prefab;
  public Player player => GameModel.main.player;
  void Start() {
    foreach (var f in player.Children) {
      HandleGetFragment(f);
    }
    player.OnGetFragment += HandleGetFragment;
    player.OnLoseFragment += HandleLoseFragment;
  }

  void Update() {
    if (GameModelController.main.isEditMode) {
      GetComponent<RectTransform>().anchoredPosition = new Vector2(285, -57);
    } else {
      GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }
  }

  private void HandleLoseFragment(Fragment obj) {
  }

  private void HandleGetFragment(Fragment obj) {
    var go = Instantiate(prefab, transform);
    go.GetComponent<QuickInfoController>().Init(obj);
  }
}
