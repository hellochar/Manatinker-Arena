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
  public override float weight => 0.5f;
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
  public override float weight => 0.5f;
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
  public override float weight => 0.5f;
  public override string Description => "Convert 50% of damage you take into Mana on this Engine.";

  public CalmEngine() {
    Player.OnTakesDamage += HandleTakesDamage;
  }

  public override void Die() {
    Player.OnDealsDamage -= HandleTakesDamage;
    base.Die();
  }

  private void HandleTakesDamage(float amount) {
    if (isPlayerOwned) {
      ChangeMana(amount * 0.5f);
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
  public override float weight => 0.75f;

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