using UnityEngine;

public class Player : Creature {
  public float influenceRadius = 3;

  public Player(Vector2 start) : base(start) {
  }

  public override void Die() {
    base.Die();
    GameModelController.main.PlayerDied();
  }
}
