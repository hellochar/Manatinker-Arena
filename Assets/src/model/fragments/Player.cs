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

public class PlayerAvatar : Avatar {
  public override string DisplayName => "Player";
  public override string Description => "Create wires to other Fragments to power them up!\n\nProtect yourself at all costs.\n\nYour Fragments only take 25% damage.";
}
