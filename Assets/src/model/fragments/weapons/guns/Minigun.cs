[RegisteredFragment]
public class Minigun : Pistol, IActivatable {
  public override (int, int) damageSpread => (3, 5);
  public override float myInFlowRate => 16;
  public override float myHpMax => 50;
  public override float myManaMax => 80;
  public override float weight => 1.5f;
  public override Projectile info => new Projectile() { baseSpeed = 15, maxDistance = 40 };
  bool IActivatable.isHold => true;
  public override float manaCost => 5;
  private float cooldown = 0;

  public override void Update(float dt) {
    cooldown -= dt;
    base.Update(dt);
  }

  public override bool CanActivateInner() {
    return base.CanActivateInner() && cooldown <= 0;
  }

  public override void Activate() {
    cooldown = 0.25f;
    base.Activate();
  }

  public override string Description => "Click and hold (5 Mana) - fire.";
}
