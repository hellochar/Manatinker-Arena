using System.Linq;
using UnityEngine;

public abstract class Transport : Utility {}

[RegisteredFragment]
public class Hoverpad : Transport {
  public override bool hasOutput => false;
  public override float weight => Mana / manaMax < 0.5f ? 4 : -8;
  public override float myInFlowRate => 4;
  public override float myManaMax => 4;
  public override float myHpMax => 30;
  public override string Description => $"Weight is -8 while above 50% Mana. Loses 2 mana/sec.";

  // drain 1 mana per second
  public float manaSinkRate => 2;

	public override void assignNodeFlows(float dt) {
    outt.flow = 0;

    // account for manaSinkRate
    var room = manaMax - Mana + manaSinkRate * dt;
    inn.flow = Mathf.Min(inFlowRate * dt, room);
	}

  public override void exchange() {
    outgoingTotal = manaSinkRate * GameModel.main.dt;
    incomingTotal = inn.edges.Select(e => e.flow).Sum();

    var netManaDiff = incomingTotal - outgoingTotal;

    lastMana = Mana;
    ChangeMana(netManaDiff);
  }
}

public interface ISpeedModifier {
  public abstract float deltaSpeed { get; }
}

[RegisteredFragment]
public class JumpPad : Transport, IActivatable {
  public override bool hasOutput => false;
  public override float weight => 2f;
  public override float myInFlowRate => 5;
  public override float myManaMax => 10;
  public override float myHpMax => 30;
  public override string Description => $"Space (10 Mana) - Jump 4 units in the Jet's direction over 1 second.";
  float timeElapsed = -1;
  bool active => timeElapsed >= 0;
  // public float deltaSpeed => active ? speedFn(timeElapsed) : 0;
  public float unitsToMove => 4 * owner.encumbranceScalar;
  public float lastMovementAmount;

  float speedFn(float t) {
    return EasingFunction.EaseOutQuint(0, 1, t);
  }

  public void Activate() {
    timeElapsed = 0;
    ChangeMana(-myManaMax);
    // owner.controller.transform.position += (Util.fromDeg(owner.controller.transform.eulerAngles.z) * 2f).z(0);
  }

  public override void Update(float dt) {
    base.Update(dt);
    if (timeElapsed >= 0) {
      timeElapsed += dt;
      var delta = speedFn(timeElapsed + dt) - speedFn(timeElapsed);
      delta *= unitsToMove;
      owner.forceMovement(Util.fromDeg(worldRotation) * delta);
      lastMovementAmount = delta / dt;
    } else {
      lastMovementAmount = 0;
    }
    // reset
    if (timeElapsed >= 1) {
      timeElapsed = -1;
    }
  }

  public bool CanActivateInner() {
    return Mana >= myManaMax;
  }

  bool IActivatable.PlayerInputCheck() {
    return Input.GetKeyDown(KeyCode.Space);
  }
}

[RegisteredFragment]
public class Jet : Transport, IActivatable {
  public override bool hasOutput => false;
  public override float weight => 2f;
  public override float myInFlowRate => 5;
  public override float myManaMax => 10;
  public override float myHpMax => 30;
  public float manaUpkeep => 10 + level;
  public override string Description => $"Space (hold, {manaUpkeep} mana/sec) - push yourself {power} units/sec in the direction the Jets are facing.";
  public float power => 2 + level * 0.5f;

  public bool isActivated = false;

  bool IActivatable.isHold => true;
  public void Activate() {
    isActivated = true;
    ChangeMana(-manaUpkeep * GameModel.main.dt);
    if (owner != null) {
      owner.forceMovement(Util.fromDeg(worldRotation) * power * GameModel.main.dt);
    }
  }

  public override void Update(float dt) {
    base.Update(dt);
    isActivated = false;
  }

  public bool CanActivateInner() {
    return Mana >= manaUpkeep * GameModel.main.dt;
  }

  bool IActivatable.PlayerInputCheck() {
    return Input.GetKey(KeyCode.Space);
  }
}