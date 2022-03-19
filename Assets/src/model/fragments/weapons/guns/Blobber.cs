
[RegisteredFragment]
public class Blobber : BasicGun {
  public override Projectile info => new Projectile() { sizeScalar = 5, baseSpeed = 5, maxDistance = 20 };
  public override (int, int) damageSpread => (18, 22);
  public override float manaCost => 13;
  public override float myInFlowRate => 13;
  public override float myManaMax => 26;
  public override float weight => 4;

  public override string DescriptionInner => "Large, slow moving bullet.";
}
