using System;
using System.Collections.Generic;
using UnityEngine;

[RegisteredFragment]
public class Rapier : MeleeWeapon, IActivatable {
  public override float myInFlowRate => 13;
  public override float myManaMax => 30;
  public override float weight => 3f;
  // per second
  public override (int, int) damageSpread => (22 + level * 3, 22 + level * 3);
  public bool activated = false;
  // activated lagged by one frame
  public bool activeLastFrame = false;
  public float T = 0;
  private float originalAngle;
  public bool isHold => true;
  public virtual float attackTime => 0.5f;
  public virtual float angleSpread => 21;
  public virtual float manaPerCycle => 10 + level;
  public virtual float lerpRate => 60;
  public float startManaRequired => manaPerCycle / 2;
  public event Action OnSwing;

  protected override void PopulateInfoStrings(List<string> lines) {
    base.PopulateInfoStrings(lines);
    lines.Add($"Swing Duration	{attackTime} sec");
  }

  public override string Description => $"Click-and-hold ({manaPerCycle} mana/cycle) - swing ({angleSpread} degree spread).\n\nNo friendly fire.";

  public override void Update(float dt) {
    activeLastFrame = activated;
    if (owner == null) {
      activated = false;
    }
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
      lastDesiredAngleDelta = 0;
      T = 0;
    }
  }

  float currentAngleDelta = 0;
  private float lastDesiredAngleDelta = 0;

  void KeepGoing(float dt) {
    FrameInfo info = new FrameInfo(this, dt);
    ChangeMana(-info.manaRequired);
    currentAngleDelta = info.nextAngleDelta;
    builtinAngle = originalAngle + currentAngleDelta;
    T += dt;

    // we've just swung
    if (info.manaRequired != 0) {
      OnSwing?.Invoke();
    }
    lastDesiredAngleDelta = info.desiredAngleDelta;

    Projectile p = new Projectile() {
      baseSpeed = 0,
      lifeTime = 0.02f,
      damage = info.percentageCycleMoved * info.fullCycleDamage,
      noFriendlyFire = true
    };
    OnShootProjectile?.Invoke(p);
  }

  struct FrameInfo {
    public float t;
    public float fullCycleAngle;
    public float fullCycleDamage;
    public float desiredAngleDelta;
    public float nextAngleDelta;
    public float angleMovement;
    public float percentageCycleMoved;
    public float manaRequired;
    public FrameInfo(Rapier r, float dt) {
      t = (r.T % r.attackTime) / r.attackTime;
      // we want the rapier to deal more damage when it's moving fast. Spread the DPS proportionally over the
      // movement angle

      // if completely active, over one attackTime cycle we start at -angleSpread, move to +angleSpread, then back to -angleSpread.
      // this is a total of 4angleSpread. Our damage over this time is attackTime * dps.
      // we now want to distribute proportionally to how much of the 4anglespread we've actually used up in this delta frame
      fullCycleAngle = (r.angleSpread * 4);
      fullCycleDamage = r.damageSpread.Item1 * r.attackTime;

      desiredAngleDelta = t < 0.5f ? r.angleSpread : -r.angleSpread;

      nextAngleDelta = Mathf.Lerp(r.currentAngleDelta, desiredAngleDelta, r.lerpRate * dt);
      angleMovement = Mathf.Abs(nextAngleDelta - r.currentAngleDelta);
      percentageCycleMoved = angleMovement / fullCycleAngle;

      var changedSwings = r.lastDesiredAngleDelta != desiredAngleDelta;
      manaRequired = changedSwings ? r.manaPerCycle / 2 : 0;
      // percentageCycleMoved * r.manaPerCycle;
    }
  }

  public bool CanActivateInner() {
    // if (!activated) {
    //   return Mana > startManaRequired;
    // }
    var info = new FrameInfo(this, GameModel.main.dt);
    return Mana >= info.manaRequired;
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

[RegisteredFragment]
public class Axe : Rapier {
  public override float myInFlowRate => 20;
  public override float myHpMax => 80;
  public override float myManaMax => 80;
  public override float weight => 8;
  public override float attackTime => 1.5f;
  public override float angleSpread => 61;
  public override float manaPerCycle => 30;
  public override float lerpRate => 10;
  public override (int, int) damageSpread => (36 + level * 4, 47 + level * 5);
}

[RegisteredFragment]
public class Sawblade : MeleeWeapon, IActivatable {
  public override (int, int) damageSpread => (31 + level * 3, 31 + level * 3);
  bool IActivatable.isHold => true;

  bool isActivated = false;
  public float manaPerSecond => 33;
  public override float myHpMax => 45;
  public override float weight => 5;
  public override float myInFlowRate => 20;
  public override float myManaMax => 50;

  // protected override void PopulateInfoStrings(List<string> lines) {
  //   base.PopulateInfoStrings(lines);
  //   lines.Add($"Swing Duration	{attackTime} sec");
  // }

  protected override string dmgString => $"{damageSpread.Item1} dmg/sec";

  public override string Description => $"Click-and-hold ({manaPerSecond} mana/sec) - twist attack.\n\nWarning - can damage yourself!";

  public override void Update(float dt) {
    base.Update(dt);
    if (isActivated) {
      ChangeMana(-manaPerSecond * dt);
      // two rotations a second
      builtinAngle += 720 * dt;
      var p = new Projectile() {
        baseSpeed = 0,
        lifeTime = 0.02f,
        // we have 5 spawn points, but it's unlikely we'll hit everything
        damage = rollDamage() * dt,
      };
      OnShootProjectile?.Invoke(p);
    }
    isActivated = false;
  }

  public void Activate() {
    isActivated = true;
  }

  public bool CanActivateInner() {
    return Mana >= manaPerSecond * GameModel.main.dt;
  }
}