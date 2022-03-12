using UnityEngine;

public abstract class Shield : Fragment {
  public override bool hasInput => false;
  public override bool hasOutput => false;
  public override float inFlowRate => 0;
  public override float outFlowRate => 0;
  public override float manaMax => 0;
}

[RegisteredFragment]
public class Buckler : Shield {
  public override float hpMax => 60;
  public override float mass => 3;
  public Buckler() {
  }
}

[RegisteredFragment]
public class TowerShield : Shield {
  public override float hpMax => 120;
  public override float mass => 7;
  public TowerShield() {
  }
}

[RegisteredFragment]
public class Crenel : Shield {
  public override float hpMax => 50;
  public override float mass => 3;
  public Crenel() {
  }
}

[RegisteredFragment]
public class DotMatrix : Shield {
  public override float hpMax => 60;
  public override float mass => 0.5f;
  public DotMatrix() {
  }
}

[RegisteredFragment]
public class PlowShield : Shield {
  public override float hpMax => 90;
  public override float mass => 5;
  public PlowShield() {
  }
}