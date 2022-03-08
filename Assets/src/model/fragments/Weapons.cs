using System;
using UnityEngine;

public abstract class Weapon : Fragment {
  public Action<Projectile> OnShootProjectile;
  protected Weapon(string name) : base("weapon") {
  }

  public abstract (int, int) damageSpread { get; }
}

public class Pistol : Weapon {
  public override (int, int) damageSpread => (8, 12);
  public override float outFlowRate => 0;
  public override float inFlowRate => 10;
  public override float hpMax => 15;
  public override float manaMax => 100;
  static Projectile info = new Projectile() { baseSpeed = 20, maxDistance = 100 };

  public Pistol(string name) : base("pistol") {
  }

  public override void Update(float dt) {
    base.Update(dt);
    if (Input.GetMouseButtonDown(0) && Mana > 10) {
      ChangeMana(-10);
      Projectile p = info;
      OnShootProjectile?.Invoke(p);
    }
  }
}

public struct Projectile {
  public float baseSpeed;
  public float maxDistance;
  public string name;
}