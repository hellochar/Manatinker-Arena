using UnityEngine;

public class Player : Creature {
  public Player(Vector2 start) : base("player-fragment", start) {
  }

  public override void Die() {
    base.Die();
    GameModelController.main.PlayerDied();
  }
}
