using System;
using System.Linq;
using UnityEngine;

public class Engine : Fragment {
  public float fuel;
  public virtual float fuelMax => 500;
  public virtual float manaPerFuel => 10;

  public override float hpMax => 15;
  public override float outFlowRate => 25;
  public override float mass => 1;
  public override float manaMax => 0;
  public override float inFlowRate => 0;

  public Engine() : base("engine") {
    this.fuel = fuelMax;
  }

  public override void Update(float dt) {
    var manaToAdd = dt * manaPerFuel;
    if (Mana + manaToAdd <= manaMax) {
      fuel -= dt;
      ChangeMana(manaToAdd);
    }
  }

  // not bounded by mana
  public override void assignNodeFlows(float dt) {
    // base.assignNodeFlows(dt);
    if (fuel <= 0) {
      outt.flow = 0;
    } else {
      outt.flow = outFlowRate * dt;
    }
    inn.flow = 0;
  }

  public override void exchange() {
    // base.exchange();
    outgoingTotal = outt.edges.Select(e => e.flow).Sum();
    incomingTotal = 0;
    // use fuel
    var fuelUsed = outgoingTotal / manaPerFuel;
    fuel = Mathf.Clamp(fuel - fuelUsed, 0, fuelMax);
  }
}