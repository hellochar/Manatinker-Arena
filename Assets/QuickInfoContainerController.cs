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
    var rt = transform.parent.GetComponent<RectTransform>();
    if (GameModelController.main.isEditMode) {
      rt.anchoredPosition = new Vector2(300, -64);
    } else {
      rt.anchoredPosition = new Vector2(0, 0);
    }
  }

  private void HandleLoseFragment(Fragment obj) {
  }

  private void HandleGetFragment(Fragment obj) {
    var go = Instantiate(prefab, transform);
    go.GetComponent<QuickInfoController>().Init(obj);
  }
}
