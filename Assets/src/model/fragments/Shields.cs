using UnityEngine;

public abstract class Shield : Fragment {
  public override float inFlowRate => 0;
  public override float outFlowRate => 0;
  public override float manaMax => 0;
  protected Shield(string name) : base(name) {
  }
}

[RegisteredFragment]
public class Buckler : Shield {
  public override float hpMax => 60;
  public override float mass => 3;
  public Buckler() : base("buckler") {
  }
}
