public abstract class BasicGun : Gun, IActivatable {
  public abstract float manaCost { get; }
  public override string Description => $"Click ({manaCost} Mana) - fire.\n{DescriptionInner}";
  public abstract string DescriptionInner { get; }
  public abstract Projectile info { get; }
  public virtual bool CanActivateInner() {
    return Mana >= manaCost;
  }

  public virtual void Activate() {
    ChangeMana(-manaCost);
    Projectile p = info;
    p.damage = rollDamage();
    OnShootProjectile?.Invoke(p);
  }
}

[RegisteredFragment]
public class Pistol : BasicGun {
  public override (int, int) damageSpread => (7, 11);
  public override float myInFlowRate => 6;
  public override float manaCost => 6;
  public override float myHpMax => 15;
  public override float myManaMax => 30;
  public override float weight => 2f;
  public override int costToUpgrade => level;
  public override Projectile info => new Projectile() { baseSpeed = 20, maxDistance = 100 };
  public override string DescriptionInner => "Fast, long ranged bullets.";
}
