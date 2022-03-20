
[RegisteredFragment]
public class Blobber : BasicGun {
  public override Projectile info => new Projectile() { sizeScalar = 5, baseSpeed = 4, maxDistance = 20 };
  public override (int, int) damageSpread => (18, 22);
  public override float manaCost => 14;
  public override float myInFlowRate => 13;
  public override float myManaMax => 26;
  public override float weight => 5;

  public override string DescriptionInner => "Large, slow moving bullet.";
}
