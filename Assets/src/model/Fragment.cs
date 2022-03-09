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
  public Vector2 _builtinOffset;
  public Vector2 builtinOffset {
    get => _builtinOffset;
    set {
      _builtinOffset = value;
      controller?.UpdateOffset(value);
    }
  }
  public float _builtinAngle;
  public float builtinAngle {
    get => _builtinAngle;
    set {
      _builtinAngle = value;
      controller?.UpdateAngle(value);
    }
  }

  public List<Wire> wires = new List<Wire>();

  public FragmentController controller;
  public Vector2 worldPos => controller.transform.position.xy();
  public float worldRotation => controller.transform.eulerAngles.z;

  public string DisplayName => GetType().Name;
  public bool isPlayerOwned => owner is PlayerFragment;

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
    if (isConnected(other)) {
      return;
    }
    var wire = new Wire(this, other);
    wires.Add(wire);
    outt.connectInn(other.inn);
    GameModel.main.OnWireAdded?.Invoke(wire);
  }

  public void disconnect(Fragment other) {
    var wire = wires.Find(w => w.to == other);
    if (wire != null) {
      wires.Remove(wire);
      outt.disconnectInn(other.inn);
      GameModel.main.OnWireRemoved?.Invoke(wire);
    }
  }

	public void ChangeMana(float diff) {
    if (mana + diff > manaMax + 0.001) {
      Debug.LogWarning($"giving {mana} + {diff} = {mana+diff}, max {manaMax}");
      mana = manaMax;
      return;
    } else if (mana + diff < -0.001) {
      Debug.LogError($"taking {-diff}, mana {mana}.");
      mana = 0;
      return;
    }
		this.mana += diff;
	}

  public bool isConnected(Fragment other) {
    return wires.Any(w => w.to == other);
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
		return $"{base.ToString()}[{name} {netManaDiff.ToString("+#0.000;-#0.000")} {lastMana.ToString("F3")} -->{incomingTotal.ToString("F3")}({inn.flow}) {outgoingTotal.ToString("F3")}({outt.flow})--> {mana.ToString("F3")} (max {manaMax})]";
	}
}

public class Wire {
  public readonly Fragment from;
  public readonly Fragment to;
  public WireController controller;

  public Wire(Fragment from, Fragment to) {
    this.from = from;
    this.to = to;
  }
}