using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Creature {
  public float influenceRadius = 2;
  public override float encumbranceThreshold => 2;
  public override float baseSpeed => 10;

  public Player(Vector2 start) : base(start) {
  }

  public override void Die() {
    base.Die();
    GameModelController.main.PlayerDied();
  }
}
