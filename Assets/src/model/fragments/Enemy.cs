using UnityEngine;

public class Enemy : Creature {
  public Enemy(Vector2 start) : base("enemy-fragment", start) {
  }
  public override float turnRate => 2;

  public float cooldown = 1.5f;

  public override void Update(float dt) {
    var player = GameModel.main.player;
    if (player.isDead) {
      return;
    }
    var offset = player.worldPos - this.worldPos;

    // e.g. 8 units away
    var currentDistance = offset.magnitude;
    var desiredDistance = 5;
    // we're 3 units too far
    var distanceOffset = currentDistance - desiredDistance;
    // 0.25f gives us a gradual slowdown to the target
    setVelocityDirection(offset.normalized * distanceOffset * 0.25f);

    if (cooldown > 0) {
      cooldown -= dt;
      return;
    }

    // rotate towards player and fire
    var desiredAngle = offset.angleDeg();
    setRotation(desiredAngle);

    // if close enough, fire at player
    if (Mathf.DeltaAngle(worldRotation, desiredAngle) < 1) {
      foreach(var f in children) {
        if (f is Pistol p) {
          if (p.CanActivate()) {
            p.Activate();
            // wait for 2 seconds after firing
            cooldown += 2f;
          }
        }
      }
    }
  }
}