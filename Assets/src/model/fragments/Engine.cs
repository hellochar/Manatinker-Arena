using System;
using System.Linq;
using UnityEngine;

public class EngineBase : Fragment {
  public override float myInFlowRate => 0;
  public override bool hasInput => false;

}

[RegisteredFragment]
public class Engine : EngineBase {
  public override float myManaMax => 0;
  public override float myHpMax => 50;
  public override float myOutFlowRate => 6;
  public override float weight => 2f;
  public override string Description => "Mana generator.";

  // not bounded by mana
  public override void assignNodeFlows(float dt) {
    // base.assignNodeFlows(dt);
    outt.flow = outFlowRate * dt;
    inn.flow = 0;
  }

  public override void exchange() {
    // base.exchange();
    outgoingTotal = outt.edges.Select(e => e.flow).Sum();
    incomingTotal = 0;
  }
}

[RegisteredFragment]
public class PainEngine : EngineBase {
  public override float myManaMax => 10;
  public override float myHpMax => 30;
  public override float myOutFlowRate => 8;
  public override float weight => 2f;
  public override string Description => "Convert 33% of damage you deal into Mana on this Engine.";

  public PainEngine() {
    Player.OnDealsDamage += HandleDealsDamage;
  }

  public override void Die() {
    Player.OnDealsDamage -= HandleDealsDamage;
    base.Die();
  }

  private void HandleDealsDamage(float amount) {
    if (isPlayerOwned) {
      ChangeMana(amount * 0.33f);
    }
  }
}

[RegisteredFragment]
public class CalmEngine : EngineBase {
  public override float myManaMax => 50;
  public override float myHpMax => 30;
  public override float myOutFlowRate => 8;
  public override float weight => 2f;
  public override string Description => "Convert 100% of damage you take into Mana on this Engine.";

  public CalmEngine() {
    Player.OnTakesDamage += HandleTakesDamage;
  }

  public override void Die() {
    Player.OnDealsDamage -= HandleTakesDamage;
    base.Die();
  }

  private void HandleTakesDamage(float amount) {
    if (isPlayerOwned) {
      ChangeMana(amount * 1.0f);
    }
  }
}

[RegisteredFragment]
public class TurretEngine : EngineBase {
  private Vector2 lastWorldPos;

  public override string Description => $"Gain {manaWhileStandingStill} Mana/s while standing still.";
  public float manaWhileStandingStill => 9 + 3 * level;
  public override float myHpMax => 60;
  public override float myManaMax => 1;
  public override float myOutFlowRate => 10;
  public override float weight => 3;

  int numFramesStill = 0;
  
  public override void Update(float dt) {
    base.Update(dt);
    if (owner == null) {
      return;
    }

    var position = owner.worldPos;
    var diff = lastWorldPos - position;

    if (diff.sqrMagnitude < 0.0000001f * dt) {
      numFramesStill++;
    } else {
      numFramesStill = 0;
    }

    // account for physics frames being different than logic
    if (numFramesStill >= 5) {
      ChangeMana(dt * manaWhileStandingStill);
    }

    this.lastWorldPos = position;   
  }
}

[RegisteredFragment]
public class FlitterEngine : EngineBase {
  private Vector2 lastWorldPos;

  public override string Description => $"Gain {manaWhileMoving} Mana/s while this engine is moving (rotation counts).";
  public float manaWhileMoving => 5 + 1 * level;
  public override float myHpMax => 10;
  public override float myManaMax => 1;
  public override float myOutFlowRate => 10;
  public override float weight => 1;

  int numFramesStill = 0;

  float movingAmount;
  
  public override void Update(float dt) {
    base.Update(dt);
    if (owner == null) {
      return;
    }

    var position = worldPos;
    var diff = lastWorldPos - position;

    if (diff.sqrMagnitude < 0.0000001f * dt) {
      numFramesStill++;
    } else {
      numFramesStill = 0;
    }

    // account for physics frames being different than logic
    if (numFramesStill > 2) {
      movingAmount = Mathf.Lerp(movingAmount, 0, 10f * dt);
      movingAmount = Mathf.MoveTowards(movingAmount, 0, 10f * dt);
    } else {
      movingAmount = Mathf.Lerp(movingAmount, 1, 10f * dt);
      movingAmount = Mathf.MoveTowards(movingAmount, 1, 10f * dt);
    }

    var manaDiff = (movingAmount - 0.5f) * 2 * dt * manaWhileMoving;
    ChangeMana(manaDiff);
    // if (numFramesStill < 4) {
    //   ChangeMana(dt * manaWhileMoving);
    // } else {
    //   ChangeMana(-dt * manaWhileMoving);
    // }

    this.lastWorldPos = position;   
  }

  public override float Intensity => 1 + movingAmount * 0.1f;
}

[RegisteredFragment]
public class HijackEngine : EngineBase {
  public override string Description => $"Steal {manaStealRate} Mana/s from any enemy Fragments this Engine touches.";
  public float manaStealRate => 10 + 4 * level;
  public override float myHpMax => 50;
  public override float myManaMax => 50;
  public override float myOutFlowRate => 20;
  public override float weight => 2;
  float lastStolenRate;
  
  public override void Update(float dt) {
    base.Update(dt);
    lastStolenRate = 0;
    if (owner is Player p) {
      for (var i = 0; i < p.numContacts; i++) {
        var contact = p.contacts[i];

        // it deals with us
        var myFc = contact.otherCollider.GetComponentInParent<FragmentController>();
        if (myFc != null && myFc.fragment == this) {
          var otherFc = contact.collider.GetComponentInParent<FragmentController>();
          if (otherFc != null) {
            var otherFragment = otherFc.fragment;
            var room = manaMax - Mana;
            var manaToSteal = Mathf.Min(manaStealRate * dt, otherFragment.Mana, room);
            otherFc.fragment.ChangeMana(-manaToSteal);
            ChangeMana(manaToSteal);
            lastStolenRate += manaToSteal / dt;
          }
        }
      }
    }
  }

  public override float Intensity => 0.8f + (lastStolenRate / manaStealRate) * 0.4f;
}