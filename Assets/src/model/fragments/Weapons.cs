using System;
using UnityEngine;

public abstract class Weapon : Fragment {
  public Action<Projectile> OnShootProjectile;
  protected Weapon(string name) : base("weapon") {
  }

  public abstract (int, int) damageSpread { get; }
  public int rollDamage() {
    var (min, max) = damageSpread;
    return UnityEngine.Random.Range(min, max + 1);
  }
}

public interface IActivatable {
  bool CanActivateInner();
  void Activate();
}
public static class ActivatableExtensions {
  public static bool CanActivate(this IActivatable a) {
    return a.CanActivateInner() && !GameModel.main.isEditMode;
  }
}

public class Pistol : Weapon, IActivatable {
  public override (int, int) damageSpread => (8, 12);
  public override float outFlowRate => 0;
  public override float inFlowRate => 7;
  public override float hpMax => 15;
  public override float manaMax => 30;
  static Projectile info = new Projectile() { baseSpeed = 10, maxDistance = 100 };

  public Pistol() : base("pistol") {
  }

  public override void Update(float dt) {
    base.Update(dt);
    if (this.CanActivate()) {
      Activate();
    }
  }

  public bool CanActivateInner() {
    return (isPlayerOwned ? Input.GetMouseButtonDown(0) : true) && Mana > 10;
  }

  public void Activate() {
    ChangeMana(-10);
    Projectile p = info;
    p.damage = rollDamage();
    OnShootProjectile?.Invoke(p);
  }
}

public struct Projectile {
  public float baseSpeed;
  public float maxDistance;
  public float damage;
  public string name;
}