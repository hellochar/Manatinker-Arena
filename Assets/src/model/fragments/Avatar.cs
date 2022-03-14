
using System;
using System.Linq;

// embodiment of the creature
public class Avatar : Fragment {
  public override float myHpMax => 100;
  public override float outFlowRate => 10;
  public override float weight => 0;
  public override float manaMax => 0;
  public override float myInFlowRate => 0;
  public override bool hasInput => false;

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
