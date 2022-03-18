using UnityEngine;

[RegisteredFragment]
public class Dagger : MeleeWeapon, IActivatable {
  public override bool hasOutput => false;
  public override float myInFlowRate => 8;
  public override float myManaMax => 6;
  public float manaCost => 6;
  public override float weight => 0.5f;
  public override (int, int) damageSpread => (7 + level * 1, 9 + level * 2);
  public float timeActivatedLeft = 0;
  public bool isActivated => timeActivatedLeft > 0;
  private Vector2 originalBuiltin;
  public float attackDistance = 0.4f;
  public float attackTime = 0.33f;
  public override bool isHold => true;

  public override string Description => $"Click ({manaCost} mana) - attack.\nDaggers do not have collision.";

  public override void Update(float dt) {
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
    Projectile p = new Projectile() { baseSpeed = 0, lifeTime = 0.02f, damage = rollDamage(), ignoreOwner = true };
    OnShootProjectile?.Invoke(p);
  }
}
