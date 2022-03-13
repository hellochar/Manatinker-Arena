using System;
using System.Linq;
using UnityEngine;

public class Engine : Fragment {
  public override float hpMax => 100;
  public override float outFlowRate => 10;
  public override float weight => 0;
  public override float manaMax => 0;
  public override float inFlowRate => 0;
  public override bool hasInput => false;

  public override string Description => "Your main source of Mana. Connect to other Fragments!\n\nCannot be repositioned.";

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
