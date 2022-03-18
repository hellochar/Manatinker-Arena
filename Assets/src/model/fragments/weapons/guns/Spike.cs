[RegisteredFragment]
public class Spike : Gun, IActivatable {
  private float timeCharging = -1;
  bool isChargingMode => timeCharging >= 0;
  public override float myInFlowRate => isChargingMode ? 60 : 0;
  public override float myManaMax => 60;
  public override float myHpMax => 30;
  public override string Description => "Click - Intake up to 60 Mana over one second, then shoot a laser that deals 1 damage per Mana.";
  protected override string dmgString => "1 damage/mana";

  public override (int, int) damageSpread => (1, 1);

  public override void Update(float dt) {
    base.Update(dt);
    if (isChargingMode) {
      timeCharging += dt;
    }
    // fire!
    if (timeCharging >= 1) {
      timeCharging = -1;
      var power = Mana;
      Projectile projectile = new Projectile() {
        isRay = true, 
        lifeTime = 0.50f,
        maxDistance = power * 0.5f,
        damage = Mana,
      };
      OnShootProjectile?.Invoke(projectile);
      ChangeMana(-Mana);
    }
  }

  public void Activate() {
    timeCharging = 0;
  }

  public bool CanActivateInner() {
    return !isChargingMode;
  }
}