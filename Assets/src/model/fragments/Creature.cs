using System.Collections.Generic;
using UnityEngine;

public class Creature : Fragment {
  public Vector2 startPosition;
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
}
