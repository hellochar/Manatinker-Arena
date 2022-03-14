using System;
using System.Linq;
using UnityEngine;

[RegisteredFragment]
public class Engine : Fragment {
  public override float hpMax => 60;
  public override float outFlowRate => 5;
  public override float weight => 1;
  public override float manaMax => 0;
  public override float inFlowRate => 0;
  public override bool hasInput => false;

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
