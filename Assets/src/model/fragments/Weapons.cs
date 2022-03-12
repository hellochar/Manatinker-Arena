using System;
using UnityEngine;

public abstract class Weapon : Fragment, IActivatable {
  public Action<Projectile> OnShootProjectile;
  protected Weapon(string name) : base("weapon") {
  }

  public abstract (int, int) damageSpread { get; }
  public abstract void Activate();
  public abstract bool CanActivateInner();

  public int rollDamage() {
    var (min, max) = damageSpread;
    return UnityEngine.Random.Range(min, max + 1);
  }
}

public struct Projectile {
  public float baseSpeed;
  public float maxDistance;
  public float lifeTime;
  public float damage;
  public string name;
  public float angleSpread;
}

[RegisteredFragment]
public class Pistol : Weapon {
  public override (int, int) damageSpread => (8, 12);
  public override bool hasOutput => false;
  public override float outFlowRate => 0;
  public override float inFlowRate => 7;
  public override float hpMax => 15;
  public override float manaMax => 30;
  static Projectile info = new Projectile() { baseSpeed = 10, maxDistance = 100 };

  public Pistol() : base("pistol") {
  }

  public override void Update(float dt) {
    base.Update(dt);
  }

  public override bool CanActivateInner() {
    return Mana > 10;
  }

  public override void Activate() {
    ChangeMana(-10);
    Projectile p = info;
    p.damage = rollDamage();
    OnShootProjectile?.Invoke(p);
  }
}

[RegisteredFragment]
public class Shotgun : Weapon {
  public override (int, int) damageSpread => (2, 4);
  public override bool hasOutput => false;
  public override float outFlowRate => 0;
  public override float inFlowRate => 8;
  public override float hpMax => 22;
  public override float manaMax => 32;
  static Projectile info = new Projectile() { baseSpeed = 15, maxDistance = 50, lifeTime = 1.5f, angleSpread = 45 };

  public Shotgun() : base("shotgun") {
  }

  public override bool CanActivateInner() {
    return Mana > 16;
  }

  public override void Activate() {
    var numBullets = Mathf.Max(Mathf.Floor(Mana / 2), 8f);
    var manaUsed = numBullets * 2f;
    ChangeMana(-manaUsed);
    for(var i = 0; i < numBullets; i++) {
      Projectile p = info;
      p.baseSpeed *= UnityEngine.Random.Range(0.8f, 1/0.8f);
      p.damage = rollDamage();
      OnShootProjectile?.Invoke(p);
    }
  }
}
