using UnityEngine;
using UnityEngine.EventSystems;

public class TileController : MonoBehaviour {
  public void Init(TileType type) {}

  void OnMouseDown() {
    EditModeInputController.instance.clickGround();
  }
}
