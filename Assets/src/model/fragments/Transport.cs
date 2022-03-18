using System.Linq;
using UnityEngine;

public abstract class Transport : Fragment {
}

[RegisteredFragment]
public class Thrusters : Transport {
  public override bool hasOutput => false;
  public float baseSpeedProvided => 3.5f * (0.8f + 0.2f * level);
  public override float weight => Mana / manaMax < 0.5f ? 0 : -2;
  public override float myInFlowRate => 2;
  public override float myManaMax => 1;
  public override float myHpMax => 30;
  public override string Description => $"Weight is -1 while above 50% Mana. Drains 1 mana/sec.";

  // drain 1 mana per second
  public float manaSinkRate => 1;

  public override void Update(float dt) {
    base.Update(dt);
  }

	public override void assignNodeFlows(float dt) {
    // cannot give more mana than I have
    outt.flow = 0;

    // cannot take more mana than I have room for
    // account for 
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
