using System;
using System.Collections.Generic;

public abstract class Weapon : Fragment {
  public override bool hasOutput => false;
  public override float myOutFlowRate => 0;
  public Action<Projectile> OnShootProjectile;

  public abstract (int, int) damageSpread { get; }

  protected Weapon() : base() {
  }
  public int rollDamage() {
    var (min, max) = damageSpread;
    return UnityEngine.Random.Range(min, max + 1);
  }
  protected override void PopulateInfoStrings(List<string> lines) {
    lines.Add($"Damage		{dmgString}");
  }

  protected virtual string dmgString {
    get {
      var (min, max) = damageSpread;
      return (min == max) ? min.ToString() : min + " - " + max;
    }
  }
}

public struct Projectile {
  public float sizeScalar;
  public float baseSpeed;
  public float maxDistance;
  public float lifeTime;
  public float damage;
  public string name;
  public float angleSpread;
  internal bool isRay;
  public Creature owner;
  public Fragment creator;
  public bool ignoreOwner;
}
