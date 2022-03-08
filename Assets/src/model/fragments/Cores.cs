using System;
using UnityEngine;

public class Core : Fragment {
  public float fuel;
  public virtual float fuelMax => 500;
  public virtual float fuelToManaRatio => 10;
  public override float hpMax => 100;
  public override float manaMax => 100;
  public override float inFlowRate => 0;
  public override float outFlowRate => 25;
  public override float mass => 0;

  public Core() : base("core") {
    this.fuel = fuelMax;
  }

  public override void Update(float dt) {
    var manaToAdd = dt * fuelToManaRatio;
    if (Mana + manaToAdd <= manaMax) {
      fuel -= dt;
      ChangeMana(manaToAdd);
    }
  }
}
