using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Creature : Fragment {
  public override float hpMax => 1;
  public override float manaMax => 0;
  public override float weight => 0;
  public override bool hasInput => false;
  public override bool hasOutput => false;
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
  private List<Fragment> children = new List<Fragment>();
  public List<Fragment> Children => children;
  public virtual float baseSpeed => 10;
  public virtual float baseTurnRate => 10f;
  public virtual float encumbranceThreshold => 10;
  public float speed => scaleByEncumbrance(baseSpeed);
  public float turnRate => scaleByEncumbrance(baseTurnRate);
  public float totalWeight;

  public event Action<Fragment> OnGetFragment;
  public event Action<Fragment> OnLoseFragment;

  float scaleByEncumbrance(float v) {
    if (totalWeight < encumbranceThreshold) {
      return v;
    }
    float overloadedAmount = totalWeight / encumbranceThreshold;
    return v / (overloadedAmount);
  }

  public Creature(Vector2 startPosition) {
    this.startPosition = startPosition;
    recomputeTotalMass();
  }

  public override void Update(float dt) {
    recomputeTotalMass();
    // do not reparent
  }

  public void AddChild(Fragment c) {
    children.Add(c);
    OnGetFragment?.Invoke(c);
  }
  public void RemoveChild(Fragment c) {
    children.Remove(c);
    OnLoseFragment?.Invoke(c);
  }

  void recomputeTotalMass() {
    totalWeight = 0f;
    foreach (var c in children) {
      totalWeight += c.weight;
    }
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
    RemoveChild(fragment);
    // oof
    if (!children.Any(c => c is Engine)) {
      hp = 0;
      Die();
    }
  }
}
