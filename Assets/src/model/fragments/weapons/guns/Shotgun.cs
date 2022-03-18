using System.Collections;
using UnityEngine;

[RegisteredFragment]
public class Shotgun : Gun, IActivatable {
  public override (int, int) damageSpread => (2, 3);
  public override float myInFlowRate => 6;
  public override float myHpMax => 22;
  public override float myManaMax => 32;
  static Projectile info = new Projectile() { baseSpeed = 9, lifeTime = 1.5f, angleSpread = 45 };

  public bool CanActivateInner() {
    return Mana > 16;
  }

  public void Activate() {
    var numBullets = Mathf.Max(Mathf.Floor(Mana / 2), 8f);
    var manaUsed = numBullets * 2f;
    ChangeMana(-manaUsed);
    IEnumerator ActivateAsync() {
      for (var i = 0; i < numBullets; i++) {
        Projectile p = info;
        p.baseSpeed *= UnityEngine.Random.Range(0.9f, 1 / 0.9f);
        p.damage = rollDamage();
        OnShootProjectile?.Invoke(p);
        yield return new WaitForEndOfFrame();
      }
    }
    controller.StartCoroutine(ActivateAsync());
  }

  protected override string dmgString => $"2-3 x8";
  public override string Description => "Click (16 mana) - fire.\nShoots 8 bullets in a 45 degree spread.";
}
