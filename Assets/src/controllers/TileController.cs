using UnityEngine;

public class TileController : MonoBehaviour {
  public void Init(TileType type) {}

  void OnMouseDown() {
    EditModeInputController.instance.Reset();
  }
}
