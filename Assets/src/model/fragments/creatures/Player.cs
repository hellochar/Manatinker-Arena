using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Creature {
  public static event Action<float> OnDealsDamage;
  public static event Action<float> OnTakesDamage;
  public float influenceRadius => 1.75f + level * 0.25f;
  public override float baseSpeed => (9 + 1 * level);
  public override float encumbranceThreshold => 4 + level * 4;
  public int gold = 5;
  public ContactPoint2D[] contacts = new ContactPoint2D[16];
  public int numContacts;

  public override void Update(float dt) {
    base.Update(dt);
    if (rigidbody != null) {
      numContacts = rigidbody.GetContacts(contacts);
    }
    // for (var i = 0; i < numContacts; i++) {
    //   var contact = contacts[i];
    //   var fragmentController = contact.collider.GetComponentInParent<FragmentController>();
    //   if (fragmentController != null) {
    //     fragmentController.fragment.ChangeHP(-1 * dt);
    //   }
    // }
  }

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
  public override float outFlowRate => 8 + 1 * level;
  public override string DisplayName => "Player " + level;
  public override string Description => "Create wires to other Fragments to power them up!\n\nProtect yourself at all costs.\n\nYour Fragments only take 25% damage.";

  public override void Update(float dt) {
    base.Update(dt);
  }

  public override void LevelUp() {
    level++;
    ChangeHP(hpMax / 2);
  }
}
