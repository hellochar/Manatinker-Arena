using UnityEngine;

public abstract class Shield : Fragment {
  public override bool hasInput => false;
  public override bool hasOutput => false;
  public override float myInFlowRate => 0;
  public override float myOutFlowRate => 0;
  public override float manaMax => 0;
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