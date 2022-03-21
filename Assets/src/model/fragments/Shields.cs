using System.Linq;
using UnityEngine;

public abstract class Shield : Fragment {
  // public override bool hasInput => false;
  public override bool hasOutput => false;
  public override float myInFlowRate => 2;
  public override float myManaMax => myHpMax / 2;
  public override float myOutFlowRate => 0;
  public virtual float absorptionPercent => 0.5f;
  public virtual float baseManaToDamageRatio => 3;
  public float ratioScalar => 1f - (level - 1) * 0.1f / ((level - 1) * 0.1f + 1);
  public virtual float manaToDamageRatio => baseManaToDamageRatio * ratioScalar;

  public override string Description => $"Redirects {Mathf.RoundToInt(absorptionPercent * 100)}% of damage to Mana ({manaToDamageRatio} Mana : 1 damage).";

  public override void ChangeHP(float diff) {
    if (!hasInput) {
      base.ChangeHP(diff);
      return;
    }
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

public class MassyShield : Shield {
  public override float baseManaToDamageRatio => 2;
}

[RegisteredFragment]
public class Buckler : MassyShield {
  public override float myHpMax => 30;
  public override float myManaMax => 30;
  public override float weight => 2f;
  public override float absorptionPercent => 0.8f;
  public Buckler() {
  }
}

[RegisteredFragment]
public class TowerShield : MassyShield {
  public override float myInFlowRate => 4;
  public override float myHpMax => 120;
  public override float weight => 10f;
  public TowerShield() {
  }
}

[RegisteredFragment]
public class Crenel : MassyShield {
  public override float myHpMax => 70;
  public override float weight => 2f;
  public Crenel() {
  }
}

[RegisteredFragment]
public class PlowShield : MassyShield {
  public override float myHpMax => 90;
  public override float weight => 6f;
  public override float myInFlowRate => 3;
  public PlowShield() {
  }
}

[RegisteredFragment]
public class DotMatrix : MassyShield {
  public override float myHpMax => 60;
  public override float myManaMax => 60;
  public override float weight => 1f;
  public DotMatrix() {
  }
}

public class EnergyShield : Shield {
  public override string Description => base.Description + "\n\nHas no collision.\n\nNot hit by friendly fire.";
  public override bool hitByFriendlyFire => false;
  public override float absorptionPercent => 1;
  public override float baseManaToDamageRatio => 3;
  public override float Intensity => ManaPercent < 0.99f ? 0.8f : base.Intensity;
}

[RegisteredFragment]
public class DefenseRing : EnergyShield {
  public override float myHpMax => 10;
  public override float myManaMax => 30;
  public override float myInFlowRate => 8;
  public override float weight => 2f;
}

[RegisteredFragment]
public class DefenseBar : EnergyShield {
  public override float myHpMax => 10;
  public override float myManaMax => 30;
  public override float myInFlowRate => 8;
  public override float weight => 2f;
}

[RegisteredFragment]
public class DefenseGrid : EnergyShield {
  public override float myHpMax => 20;
  public override float myManaMax => 200;
  public override float myInFlowRate => 20;
  public override float weight => 3f;
  public override float baseManaToDamageRatio => 4;
}

[RegisteredFragment]
public class PDS : EnergyShield {
  public override float myHpMax => 5;
  public override float myManaMax => 25;
  public override float myInFlowRate => 10;
  public override float weight => 1f;
  public override float absorptionPercent => 1;
  public override float baseManaToDamageRatio => 3;
  public override int costToUpgrade => base.costToUpgrade - 1;
}


[RegisteredFragment]
public class Platemail : Shield {
  public override bool hasInput => false;
  public override float myHpMax => 250;
  public override float myManaMax => 0;
  public override float myInFlowRate => 0;
  public override float weight => 12f;
  public override float absorptionPercent => 0;
  public override float baseManaToDamageRatio => 10;
  public override string Description => $"Just a blocker.";
}