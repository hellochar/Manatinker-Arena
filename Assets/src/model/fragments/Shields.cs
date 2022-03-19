using System.Linq;
using UnityEngine;

public abstract class Shield : Fragment {
  // public override bool hasInput => false;
  public override bool hasOutput => false;
  public override float myInFlowRate => 5;
  public override float myManaMax => myHpMax / 2;
  public override float myOutFlowRate => 0;
  public virtual float absorptionPercent => 0.5f;
  public virtual float manaToDamageRatio => 3;

  public override string Description => $"Redirects {Mathf.RoundToInt(absorptionPercent * 100)}% of damage to Mana ({manaToDamageRatio} Mana : 1 damage).";

  public override void ChangeHP(float diff) {
    // take mana damage first
    if (diff < 0) {
      var dmgToSoak = -diff * absorptionPercent;
      var manaCost = dmgToSoak * manaToDamageRatio;
      var manaUsed = Mathf.Min(Mana, manaCost);
      var dmgTaken = manaUsed / manaToDamageRatio;
      diff += dmgTaken;
      ChangeMana(-manaUsed);
      base.ChangeHP(diff);
    } else {
      base.ChangeHP(diff);
    }
  }
}

[RegisteredFragment]
public class Buckler : Shield {
  public override float myHpMax => 30;
  public override float myManaMax => 30;
  public override float weight => 2f;
  public override float absorptionPercent => 0.8f;
  public Buckler() {
  }
}

[RegisteredFragment]
public class TowerShield : Shield {
  public override float myInFlowRate => 3;
  public override float myHpMax => 120;
  public override float weight => 10f;
  public TowerShield() {
  }
}

[RegisteredFragment]
public class Crenel : Shield {
  public override float myHpMax => 70;
  public override float weight => 2f;
  public Crenel() {
  }
}

[RegisteredFragment]
public class DotMatrix : Shield {
  public override float myHpMax => 10;
  public override float myManaMax => 60;
  public override float weight => 1f;
  public override float absorptionPercent => 1;
  public DotMatrix() {
  }
}

[RegisteredFragment]
public class PlowShield : Shield {
  public override float myHpMax => 90;
  public override float weight => 6f;
  public PlowShield() {
  }
}