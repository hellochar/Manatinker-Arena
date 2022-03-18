using UnityEngine;

[RegisteredFragment]
public class Laser : Gun, IActivatable {
  // 7 dps
  public override (int, int) damageSpread => (24, 24);
  public override float myInFlowRate => 15;
  public override float myHpMax => 20;
  public override float myManaMax => 25;
  public override bool isHold => true;
  public float manaDrainWhileActivated => 30;
  public bool needsRecharge = false;

  protected override string dmgString => $"{damageSpread.Item1}/sec";

  public override string Description => "Click-and-hold (30 mana/sec) - shoot laser.\nWhen out of mana, Laser requires a full recharge.";

  static Projectile info = new Projectile() { isRay = true, maxDistance = 15f };

  public override void Update(float dt) {
    if (needsRecharge && Mathf.Abs(Mana - manaMax) < 0.001f) {
      needsRecharge = false;
    }
  }

  public bool CanActivateInner() {
    return !needsRecharge && owner != null;
  }

  public void Activate() {
    var wantedMana = Time.deltaTime * manaDrainWhileActivated;
    var actualMana = Mathf.Min(Mana, wantedMana);
    var powerScale = actualMana / wantedMana;
    if (actualMana < wantedMana) {
      needsRecharge = true;
    }
    ChangeMana(-actualMana);
    // shoot a ray
    Projectile p = info;
    p.damage = rollDamage() * Time.deltaTime * powerScale;
    OnShootProjectile?.Invoke(p);
  }
}
