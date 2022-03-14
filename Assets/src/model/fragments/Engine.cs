using System;
using System.Linq;
using UnityEngine;

public class EngineBase : Fragment {
  public override float myInFlowRate => 0;
  public override bool hasInput => false;

}

[RegisteredFragment]
public class Engine : Fragment {
  public override float myManaMax => 0;
  public override float myHpMax => 60;
  public override float myOutFlowRate => 5;
  public override float weight => 1;
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
public class PainEngine : Fragment {
  public override float myManaMax => 50;
  public override float myHpMax => 10;
  public override float myOutFlowRate => 8;
  public override float weight => 0.5f;
  public override string Description => "Convert 20% of damage you deal into Mana on this Engine.";

  public PainEngine() {
    Player.OnDealsDamage += HandleDealsDamage;
  }

  public override void Die() {
    Player.OnDealsDamage -= HandleDealsDamage;
    base.Die();
  }

  private void HandleDealsDamage(float amount) {
    if (isPlayerOwned) {
      ChangeMana(amount * 0.2f);
    }
  }
}