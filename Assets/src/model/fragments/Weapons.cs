using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Fragment {
  public override bool hasOutput => false;
  public override float myOutFlowRate => 0;
  public Action<Projectile> OnShootProjectile;

  public abstract (int, int) damageSpread { get; }
  public virtual bool isHold => false;

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
  public float baseSpeed;
  public float maxDistance;
  public float lifeTime;
  public float damage;
  public string name;
  public float angleSpread;
  internal bool isRay;
  public Creature owner;
  public Fragment fragment;
  public bool ignoreOwner;
}

public abstract class Gun : Weapon {
}

[RegisteredFragment]
public class Pistol : Gun, IActivatable {
  public override (int, int) damageSpread => (8, 12);
  public override float myInFlowRate => 8;
  public override float myHpMax => 15;
  public override float myManaMax => 40;
  public override float weight => 0.5f;
  static Projectile pistolInfo = new Projectile() { baseSpeed = 20, maxDistance = 100 };
  public virtual Projectile info => pistolInfo;

  public virtual float manaCost => 7;

  public virtual bool CanActivateInner() {
    return Mana > manaCost;
  }

  public virtual void Activate() {
    ChangeMana(-10);
    Projectile p = info;
    p.damage = rollDamage();
    OnShootProjectile?.Invoke(p);
  }

  public override string Description => "Click (7 Mana) - fire.\nFast, long ranged bullets.";
}

[RegisteredFragment]
public class Minigun : Pistol {
  public override (int, int) damageSpread => (4, 6);
  public override float myInFlowRate => 16;
  public override float myHpMax => 50;
  public override float myManaMax => 80;
  public override float weight => 1.5f;
  public override Projectile info => new Projectile() { baseSpeed = 15, maxDistance = 40 };
  public override bool isHold => true;
  public override float manaCost => 5;
  private float cooldown = 0;

  public override void Update(float dt) {
    cooldown -= dt;
    base.Update(dt);
  }

  public override bool CanActivateInner() {
    return base.CanActivateInner() && cooldown <= 0;
  }

  public override void Activate() {
    cooldown = 0.25f;
    base.Activate();
  }

  public override string Description => "Click and hold (5 Mana) - fire.";
}


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

[RegisteredFragment]
public class Laser : Gun, IActivatable {
  // 7 dps
  public override (int, int) damageSpread => (24, 24);
  public override float myInFlowRate => 15;
  public override float myHpMax => 20;
  public override float myManaMax => 25;
  public override bool isHold => true;
  public float manaDrainWhileActivated => 30;
  public bool needsRecharge = false;

  protected override string dmgString => $"{damageSpread.Item1}/sec";

  public override string Description => "Click-and-hold (30 mana/sec) - shoot laser.\nWhen out of mana, Laser requires a full recharge.";

  static Projectile info = new Projectile() { isRay = true, maxDistance = 15f };

  public override void Update(float dt) {
    if (needsRecharge && Mathf.Abs(Mana - manaMax) < 0.001f) {
      needsRecharge = false;
    }
  }

  public bool CanActivateInner() {
    return !needsRecharge && owner != null;
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

public abstract class MeleeWeapon : Weapon {}

[RegisteredFragment]
public class Dagger : MeleeWeapon, IActivatable {
  public override bool hasInput => false;
  public override bool hasOutput => false;
  public override float weight => 0.5f;
  public override (int, int) damageSpread => (7, 9);
  public float timeActivatedLeft = 0;
  public bool isActivated => timeActivatedLeft > 0;
  private Vector2 originalBuiltin;
  public float attackDistance = 0.4f;
  public float attackTime = 0.33f;
  public override bool isHold => true;

  public override void Update(float dt) {
    if (timeActivatedLeft > 0) {
      timeActivatedLeft -= dt;
      if (timeActivatedLeft < 0) {
        timeActivatedLeft = 0;
      }
      UpdateBuiltinOffset();
    }
    base.Update(dt);
  }

  public void UpdateBuiltinOffset() {
    var t = timeActivatedLeft / attackTime;
    builtinOffset = originalBuiltin + 
      Util.fromDeg(builtinAngle) * attackDistance * t;
  }

  public bool CanActivateInner() {
    return !isActivated;
  }

  public void Activate() {
    originalBuiltin = builtinOffset;
    timeActivatedLeft = attackTime;
    UpdateBuiltinOffset();
    Projectile p = new Projectile() { baseSpeed = 0, lifeTime = 0.02f, damage = rollDamage(), ignoreOwner = true };
    OnShootProjectile?.Invoke(p);
  }
}

[RegisteredFragment]
public class Rapier : MeleeWeapon, IActivatable {
  public override bool hasInput => false;
  public override bool hasOutput => false;
  public override float weight => 1f;
  // per second
  public override (int, int) damageSpread => (26 + level * 3, 26 + level * 3);
  public bool activated = false;
  // activated lagged by one frame
  public bool activeLastFrame = false;
  public float T = 0;
  private float originalAngle;
  public float attackTime = 0.5f;
  public override bool isHold => true;
  public static float angleSpread = 21;

  public override void Update(float dt) {
    activeLastFrame = activated;
    if (!activated) {
      TurnOff();
    } else {
      KeepGoing(dt);
      activated = false;
    }
    base.Update(dt);
  }

  void TurnOff() {
    if (T != 0) {
      builtinAngle = originalAngle;
      currentAngleDelta = 0;
      T = 0;
    }
  }

  float currentAngleDelta = 0;
  void KeepGoing(float dt) {
    var t = (T % attackTime) / attackTime;
    // we want the rapier to deal more damage when it's moving fast. Spread the DPS proportionally over the
    // movement angle

    // if completely active, over one attackTime cycle we start at -angleSpread, move to +angleSpread, then back to -angleSpread.
    // this is a total of 4angleSpread. Our damage over this time is attackTime * dps.
    // we now want to distribute proportionally to how much of the 4anglespread we've actually used up in this delta frame
    var fullCycleAngle = (angleSpread * 4);
    var fullCycleDamage = damageSpread.Item1 * attackTime;

    var desiredAngleDelta = t < 0.5f ? angleSpread : -angleSpread;

    var nextAngleDelta = Mathf.Lerp(currentAngleDelta, desiredAngleDelta, 30 * dt);
    var angleMovement = Mathf.Abs(nextAngleDelta - currentAngleDelta);
    var percentageCycleMoved = angleMovement / fullCycleAngle;

    currentAngleDelta = nextAngleDelta;
    builtinAngle = originalAngle + currentAngleDelta;
    T += dt;

    Projectile p = new Projectile() {
      baseSpeed = 0,
      lifeTime = 0.02f,
      damage = percentageCycleMoved * fullCycleDamage,
      ignoreOwner = true
    };
    OnShootProjectile?.Invoke(p);
  }

  public bool CanActivateInner() {
    return true;
  }

  public void Activate() {
    // we just started
    if (!activeLastFrame) {
      ActivateStart();
    }
    activated = true;
  }

  private void ActivateStart() {
    originalAngle = builtinAngle;
    T = 0;
  }
}