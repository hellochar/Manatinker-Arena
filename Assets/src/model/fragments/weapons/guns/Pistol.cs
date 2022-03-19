[RegisteredFragment]
public class Pistol : Gun, IActivatable {
  public override (int, int) damageSpread => (7, 11);
  public override float myInFlowRate => 5;
  public override float myHpMax => 15;
  public override float myManaMax => 30;
  public override float weight => 0.5f;
  static Projectile pistolInfo = new Projectile() { baseSpeed = 20, maxDistance = 100 };
  public virtual Projectile info => pistolInfo;

  public virtual float manaCost => 5;

  public virtual bool CanActivateInner() {
    return Mana > manaCost;
  }

  public virtual void Activate() {
    ChangeMana(-manaCost);
    Projectile p = info;
    p.damage = rollDamage();
    OnShootProjectile?.Invoke(p);
  }

  public override string Description => "Click (5 Mana) - fire.\nFast, long ranged bullets.";
}
