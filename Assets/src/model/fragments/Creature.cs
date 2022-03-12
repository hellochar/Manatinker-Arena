using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Creature : Fragment {
  public override float hpMax => 1;
  public override float manaMax => 0;
  public Vector2 startPosition;
  public float startAngle;
  private Rigidbody2D _rb2d;
  private Rigidbody2D rb2d {
    get {
      if (_rb2d == null) {
        _rb2d = controller?.GetComponent<Rigidbody2D>();
      }
      return _rb2d;
    }
  }
  public List<Fragment> children = new List<Fragment>();
  public virtual float speed => 10;
  public virtual float turnRate => 10f;
  public Creature(string name, Vector2 startPosition) : base(name) {
    this.startPosition = startPosition;
  }

  public override void Update(float dt) {
    // do not reparent
  }

  public override void ChangeHP(float diff) {
    throw new System.Exception("Creature itself should not change HP!");
  }

  public void setVelocityDirection(Vector2 inDirection) {
    if (rb2d != null) {
      var dir = inDirection;
      if (dir.magnitude > 1) {
        dir = dir.normalized;
      }
      rb2d.velocity = dir * speed;
    }
  }

  public void setRotation(float targetAngle) {
    if (rb2d != null) {
      var currentAngle = rb2d.rotation;
      // e.g. 10%
      var newAngle = Mathf.LerpAngle(currentAngle, targetAngle, turnRate * Time.deltaTime);
      // 5 degrees
      newAngle = Mathf.MoveTowardsAngle(newAngle, targetAngle, (turnRate / 2) * Time.deltaTime);
      rb2d.SetRotation(newAngle);
    }
  }

  public override void Die() {
    // remove all fragments from your ownership
    var children = new List<Fragment>(this.children);
    foreach (var c in children) {
      // maintain their existing transform
      c.builtinAngle = c.worldRotation;
      c.builtinOffset = c.worldPos;
      c.owner = null;
    }
    base.Die();
  }

  internal void FragmentDied(Fragment fragment) {
    // hack - don't unset fragment's owner since that will trigger the FragmentController to update. Just stop
    // accounting for it on our end 
    children.Remove(fragment);
    // oof
    if (!children.Any(c => c is Engine)) {
      hp = 0;
      Die();
    }
  }
}
