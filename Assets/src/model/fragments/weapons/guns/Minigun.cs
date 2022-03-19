[RegisteredFragment]
public class Minigun : BasicGun, IActivatable {
  public override (int, int) damageSpread => (3, 5);
  public override float myInFlowRate => 16;
  public override float myHpMax => 50;
  public override float myManaMax => 80;
  public override float weight => 6f;
  public override Projectile info => new Projectile() { sizeScalar = 0.75f, baseSpeed = 15, maxDistance = 40 };
  bool IActivatable.isHold => true;
  public override float manaCost => 5;
  private float cooldown = 0;
  public virtual float rateOfFire => 3 + level;

  public override void Update(float dt) {
    cooldown -= dt;
    base.Update(dt);
  }

  public override bool CanActivateInner() {
    return base.CanActivateInner() && cooldown <= 0;
  }

  public override void Activate() {
    cooldown = 1 / rateOfFire;
    base.Activate();
  }

  public override string DescriptionInner => $"Hold to fire {rateOfFire} bullets per second.";
}

[RegisteredFragment]
public class Piddler : Minigun {
  public override Projectile info => new Projectile() { sizeScalar = 1, baseSpeed = 12, maxDistance = 5, angleSpread = 20 };
  public override (int, int) damageSpread => (4, 5);
  public override float manaCost => 2;
  public override float myInFlowRate => 20;
  public override float myManaMax => 8;
  public override float weight => 3;
  public override float rateOfFire => 20;

  public override string DescriptionInner => "Short ranged and inaccurate.";
}