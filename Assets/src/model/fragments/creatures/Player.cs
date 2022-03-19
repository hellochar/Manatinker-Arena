using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Creature {
  public static event Action<float> OnDealsDamage;
  public static event Action<float> OnTakesDamage;
  public float influenceRadius => 1.75f + level * 0.25f;
  public override float baseSpeed => (8 + 2 * level);
  public override float encumbranceThreshold => 4 + level * 4;

  public int gold = 5;

  public void dealtDamage(float dmg) {
    OnDealsDamage?.Invoke(dmg);
  }

  public void tookDamage(float dmg) {
    OnTakesDamage?.Invoke(dmg);
  }

  public override void LevelUp() {
    var playerAvatar = (PlayerAvatar)avatar;
    playerAvatar.LevelUp();
    level++;
  }

  public Player(Vector2 start) : base(start) {
  }

  public override void Die() {
    base.Die();
    GameModelController.main.PlayerDied();
  }
}

public class PlayerAvatar : Avatar {
  public override float hpMax => 45 + level * 5;
  public override float outFlowRate => 9 + 1 * level;
  public override string DisplayName => "Player " + level;
  public override string Description => "Create wires to other Fragments to power them up!\n\nProtect yourself at all costs.\n\nYour Fragments only take 25% damage.";


  public override void LevelUp() {
    level++;
    ChangeHP(hpMax / 2);
  }
}
