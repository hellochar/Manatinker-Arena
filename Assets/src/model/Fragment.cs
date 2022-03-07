using System;
using System.Linq;
using UnityEngine;

class Fragment : MonoBehaviour {
  public Node outt, inn;
  public new string name;
  public float mana = 50;
  public float manaMax = 100;
  public float outFlowRate;
  public float inFlowRate;
  public Fragment(string name, float outFlowRate, float inFlowRate) {
    this.name = name;
    this.outFlowRate = outFlowRate;
    this.inFlowRate = inFlowRate;
    outt = new Node("out" + name);
    inn = new Node("in" + name);
  }

  public void connect(Fragment other) {
    outt.connectInn(other.inn);
  }

	private void ChangeMana(float diff) {
    if (mana + diff > manaMax) {
      Console.WriteLine($"giving {mana} + {diff} = {mana+diff}, max {manaMax}");
    } else if (mana + diff < 0) {
      Console.WriteLine($"taking {-diff}, mana {mana}.");
    }
		this.mana += diff;
	}

  float incomingTotal, outgoingTotal;
  float lastMana;
  // assumes all edges on my nodes are solved
  public void exchange() {
    outgoingTotal = outt.edges.Select(e => e.flow).Sum();
    incomingTotal = inn.edges.Select(e => e.flow).Sum();

    var netManaDiff = incomingTotal - outgoingTotal;
    // System.Console.WriteLine($"incoming {incomingTotal}, outgoing {outgoingTotal}, net {netManaDiff}");

    lastMana = mana;
    ChangeMana(netManaDiff);
  }

	public void assignNodeFlows(float dt) {
    // cannot give more mana than I have
    outt.flow = Math.Min(outFlowRate * dt, mana);

    // cannot take more mana than I have room for
    var room = manaMax - mana;
    inn.flow = Math.Min(inFlowRate * dt, room);
	}

	public override string ToString() {
    var netManaDiff = incomingTotal - outgoingTotal;
		return $"[{name} {netManaDiff.ToString("+#0.000;-#0.000")} {lastMana.ToString("F3")} -->{incomingTotal.ToString("F3")}({inn.flow}) {outgoingTotal.ToString("F3")}({outt.flow})--> {mana.ToString("F3")} (max {manaMax})]";
	}
}
