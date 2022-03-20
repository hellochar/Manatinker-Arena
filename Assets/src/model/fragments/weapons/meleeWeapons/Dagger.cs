using System.Collections.Generic;
using UnityEngine;

[RegisteredFragment]
public class Dagger : MeleeWeapon, IActivatable {
  public override bool hasOutput => false;
  public override float myInFlowRate => 8;
  public override float myManaMax => 6;
  public override float weight => 2f;
  public override (int, int) damageSpread => (7 + level * 1, 9 + level * 2);
  public float timeActivatedLeft = 0;
  public bool isActivated => timeActivatedLeft > 0;
  private Vector2 originalBuiltin;
  public virtual float manaCost => 6;
  public virtual float attackDistance => 0.4f;
  public virtual float attackTime => 0.33f;
  public virtual bool isHold => true;

  protected override void PopulateInfoStrings(List<string> lines) {
    base.PopulateInfoStrings(lines);
    lines.Add($"Reach		{attackDistance} meters");
    lines.Add($"Return Time	{attackTime} sec");
  }

  public override string Description => $"Click ({manaCost} mana) - attack.\n\nDaggers do not have collision nor friendly fire.";

  public override void Update(float dt) {
    if (owner == null) {
      timeActivatedLeft = 0;
    }
    if (timeActivatedLeft > 0) {
      timeActivatedLeft -= dt;
      if (timeActivatedLeft < 0) {
        timeActivatedLeft = 0;
      }
      UpdateBuiltinOffset();
    }
    base.Update(dt);
  }

  public void UpdateBuiltinOffset() {
    var t = timeActivatedLeft / attackTime;
    builtinOffset = originalBuiltin + 
      Util.fromDeg(builtinAngle) * attackDistance * t;
  }

  public bool CanActivateInner() {
    return !isActivated && Mana >= manaCost;
  }

  public void Activate() {
    ChangeMana(-manaCost);
    originalBuiltin = builtinOffset;
    timeActivatedLeft = attackTime;
    UpdateBuiltinOffset();
    Projectile p = new Projectile() { baseSpeed = 0, lifeTime = 0.02f, damage = rollDamage(), noFriendlyFire = true };
    OnShootProjectile?.Invoke(p);
  }
}

[RegisteredFragment]
public class Spear : Dagger {
  public override bool isHold => false;
  public override float manaCost => 16;
  public override float attackDistance => 1.8f;
  public float attackSpeedScalar => 1f - (level - 1) * 0.2f / ((level - 1) * 0.2f + 1);
  public override float attackTime => 1.0f * attackSpeedScalar;
  public override float weight => 3f;
  public override float myInFlowRate => 12;
  public override float myManaMax => 16;
  public override (int, int) damageSpread => (15 + level * 1, 19 + level * 2);

  public override string Description => $"Click ({manaCost} mana) - attack.";
}