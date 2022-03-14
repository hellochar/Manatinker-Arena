using UnityEngine;

public abstract class Shield : Fragment {
  // public override bool hasInput => false;
  public override bool hasOutput => false;
  public override float myInFlowRate => 10;
  public override float myManaMax => myHpMax;
  public override float myOutFlowRate => 0;

  public override string Description => "Takes damage from mana first at a 2:1 ratio.";

  public override void ChangeHP(float diff) {
    // take mana damage first
    if (diff < 0) {
      var dmg = -diff;
      var manaCost = dmg * 2;
      var manaUsed = Mathf.Min(Mana, manaCost);
      var dmgTaken = manaUsed / 2;
      diff += dmgTaken;
      ChangeMana(-manaUsed);
      if (diff < 0) {
        base.ChangeHP(diff);
      }
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
  public override float weight => 2;
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
  public override float weight => 1.5f;
  public PlowShield() {
  }
}