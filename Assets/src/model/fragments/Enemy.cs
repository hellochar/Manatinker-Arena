using UnityEngine;

public class Enemy : Creature {
  public Enemy(Vector2 start) : base("enemy-fragment", start) {
  }

  public override void Update(float dt) {
    // rotate towards player and fire
    var player = GameModel.main.player;
    var offset = player.worldPos - this.worldPos;
    var desiredAngle = offset.angleDeg();
    setRotation(desiredAngle);

    // if close enough, fire at player
    if (Mathf.DeltaAngle(worldRotation, desiredAngle) < 10) {
      foreach(var f in children) {
        if (f is Pistol p) {
          if (p.CanActivate()) {
            p.Activate();
          }
        }
      }
    }
  }
}