using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Fragment {
  public Node outt, inn;
  public new string name;
  [SerializeField]
  [ReadOnly]
  private float mana;
  public float Mana => mana;
  private float hp;
  public float Hp => hp;
  public virtual float manaMax => 100;
  public virtual float outFlowRate => 0;
  public virtual float inFlowRate => 0;
  public virtual float mass => 1;
  public virtual float weight => (1 + builtinOffset.magnitude) * mass;

  public virtual float hpMax => 30;
  public bool isBroken = false;

  // e.g. the player
  public Fragment owner;
  public Vector2 builtinOffset;
  public float builtinAngle;

  public FragmentController controller;
  public Vector2 worldPos => controller.transform.position.xy();
  public float worldRotation => controller.transform.eulerAngles.z;

  public Fragment() : this("") {
  }

  public Fragment(string name) {
    this.name = name;
    outt = new Node("out" + name);
    inn = new Node("in" + name);
    hp = hpMax;
  }

  public virtual void Update(float dt) {}

  public void connect(Fragment other) {
    outt.connectInn(other.inn);
  }

	public void ChangeMana(float diff) {
    if (mana + diff > manaMax) {
      Console.WriteLine($"giving {mana} + {diff} = {mana+diff}, max {manaMax}");
      mana = manaMax;
      return;
    } else if (mana + diff < 0) {
      Console.WriteLine($"taking {-diff}, mana {mana}.");
      mana = 0;
      return;
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
