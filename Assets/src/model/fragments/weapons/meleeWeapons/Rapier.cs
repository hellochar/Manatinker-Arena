using System;
using UnityEngine;

[RegisteredFragment]
public class Rapier : MeleeWeapon, IActivatable {
  public override bool hasOutput => false;
  public override float myInFlowRate => 13;
  public override float myManaMax => 30;
  public override float weight => 3f;
  // per second
  public override (int, int) damageSpread => (26 + level * 3, 26 + level * 3);
  public bool activated = false;
  // activated lagged by one frame
  public bool activeLastFrame = false;
  public float T = 0;
  private float originalAngle;
  public float attackTime = 0.5f;
  public bool isHold => true;
  public static float angleSpread = 21;
  public float manaPerCycle = 10;
  public float startManaRequired => manaPerCycle / 2;
  public event Action OnSwing;

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
      ignoreOwner = true
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
      fullCycleAngle = (angleSpread * 4);
      fullCycleDamage = r.damageSpread.Item1 * r.attackTime;

      desiredAngleDelta = t < 0.5f ? angleSpread : -angleSpread;

      nextAngleDelta = Mathf.Lerp(r.currentAngleDelta, desiredAngleDelta, 60 * dt);
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
    return Mana > info.manaRequired;
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