using System;
using UnityEngine;

public abstract class Weapon : Fragment {
  public override bool hasOutput => false;
  public override float outFlowRate => 0;
  public Action<Projectile> OnShootProjectile;

  public abstract (int, int) damageSpread { get; }
  public virtual bool isHold => false;

  protected Weapon() : base() {
  }
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
  internal bool isRay;
}

[RegisteredFragment]
public class Pistol : Weapon, IActivatable {
  public override (int, int) damageSpread => (8, 12);
  public override float inFlowRate => 7;
  public override float hpMax => 15;
  public override float manaMax => 30;
  static Projectile info = new Projectile() { baseSpeed = 20, maxDistance = 100 };

  public override void Update(float dt) {
    base.Update(dt);
  }

  public bool CanActivateInner() {
    return Mana > 10;
  }

  public void Activate() {
    ChangeMana(-10);
    Projectile p = info;
    p.damage = rollDamage();
    OnShootProjectile?.Invoke(p);
  }
}

[RegisteredFragment]
public class Shotgun : Weapon, IActivatable {
  public override (int, int) damageSpread => (2, 4);
  public override float inFlowRate => 8;
  public override float hpMax => 22;
  public override float manaMax => 32;
  static Projectile info = new Projectile() { baseSpeed = 10, lifeTime = 1.5f, angleSpread = 45 };

  public bool CanActivateInner() {
    return Mana > 16;
  }

  public void Activate() {
    var numBullets = Mathf.Max(Mathf.Floor(Mana / 2), 8f);
    var manaUsed = numBullets * 2f;
    ChangeMana(-manaUsed);
    for (var i = 0; i < numBullets; i++) {
      Projectile p = info;
      p.baseSpeed *= UnityEngine.Random.Range(0.9f, 1 / 0.9f);
      p.damage = rollDamage();
      OnShootProjectile?.Invoke(p);
    }
  }
}

[RegisteredFragment]
public class Laser : Weapon, IActivatable {
  // 7 dps
  public override (int, int) damageSpread => (15, 15);
  public override float inFlowRate => 25;
  public override float hpMax => 18;
  public override float manaMax => 25;
  public override bool isHold => true;
  public float manaDrainWhileActivated => 30;
  public bool needsRecharge = false;

  static Projectile info = new Projectile() { isRay = true, maxDistance = 15f };

  public override void Update(float dt) {
    if (needsRecharge && Mathf.Abs(Mana - manaMax) < 0.001f) {
      needsRecharge = false;
    }
  }

  public bool CanActivateInner() {
    return !needsRecharge;
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