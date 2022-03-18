using System.Linq;
using UnityEngine;

public abstract class Shield : Fragment {
  // public override bool hasInput => false;
  public override bool hasOutput => false;
  public override float myInFlowRate => 5;
  public override float myManaMax => myHpMax / 2;
  public override float myOutFlowRate => 0;

  public override string Description => "Redirects 50% of damage to Mana (3 Mana : 1 damage).";

  public override void ChangeHP(float diff) {
    // take mana damage first
    if (diff < 0) {
      var dmgToSoak = -diff / 2;
      var manaCost = dmgToSoak * 3;
      var manaUsed = Mathf.Min(Mana, manaCost);
      var dmgTaken = manaUsed / 3;
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
  public override float myHpMax => 60;
  public override float weight => 1;
  public Buckler() {
  }
}

[RegisteredFragment]
public class TowerShield : Shield {
  public override float myHpMax => 120;
  public override float weight => 3;
  public TowerShield() {
  }
}

[RegisteredFragment]
public class Crenel : Shield {
  public override float myHpMax => 50;
  public override float weight => 1;
  public Crenel() {
  }
}

[RegisteredFragment]
public class DotMatrix : Shield {
  public override float myHpMax => 60;
  public override float weight => 0.5f;
  public DotMatrix() {
  }
}

[RegisteredFragment]
public class PlowShield : Shield {
  public override float myHpMax => 90;
  public override float weight => 2f;
  public PlowShield() {
  }
}