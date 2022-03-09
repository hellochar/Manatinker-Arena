using System;
using UnityEngine;

public abstract class Weapon : Fragment {
  public Action<Projectile> OnShootProjectile;
  protected Weapon(string name) : base("weapon") {
  }

  public abstract (int, int) damageSpread { get; }
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
  static Projectile info = new Projectile() { baseSpeed = 20, maxDistance = 100 };

  public Pistol() : base("pistol") {
  }

  public override void Update(float dt) {
    base.Update(dt);
    if (this.CanActivate()) {
      Activate();
    }
  }

  public bool CanActivateInner() {
    return Input.GetMouseButtonDown(0) && Mana > 10;
  }

  public void Activate() {
    ChangeMana(-10);
    Projectile p = info;
    OnShootProjectile?.Invoke(p);
  }
}

public struct Projectile {
  public float baseSpeed;
  public float maxDistance;
  public string name;
}