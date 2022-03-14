using UnityEngine;

public class Enemy : Creature {
  public EnemyAI ai;
  public Enemy(Vector2 start) : this(start, EnemyAI.Default) {
  }

  public Enemy(Vector2 start, EnemyAI ai) : base(start) {
    this.ai = ai;
  }
  public override float baseTurnRate => ai.baseTurnRate;
  public override float baseSpeed => ai.baseSpeed;

  public float cooldown = 0f;

  public override void Die() {
    GameModel.main.player.gold++;
    base.Die();
  }

  public override void Update(float dt) {
    var player = GameModel.main.player;
    if (player.isDead) {
      return;
    }

    cooldown -= dt;
    if (cooldown > 0) {
      return;
    }

    var offset = player.worldPos - this.worldPos;

    // e.g. 8 units away
    var currentDistance = offset.magnitude;
    // we're 3 units too far
    var distanceOffset = currentDistance - ai.desiredDistance;
    // 10 saturates us to max movespeed until we're within .1 units of the target
    setVelocityDirection(offset.normalized * distanceOffset * 3);

    // rotate towards player and fire
    var desiredAngle = offset.angleDeg();
    setRotation(desiredAngle);

    // if close enough, fire at player
    if (Mathf.Abs(Mathf.DeltaAngle(worldRotation, desiredAngle)) < ai.deltaAngleThreshold && distanceOffset < ai.minDistance) {
      foreach(var f in Children) {
        // fire weapons
        if (f is Weapon w && f is IActivatable p) {
          if (p.CanActivate()) {
            p.Activate();
            // active for at least 2 seconds
            if (cooldown < -ai.minActiveDuration) {
              // then pause for 2 seconds
              cooldown = ai.cooldown;
            }
          }
        }
      }
    }
  }
}

public struct EnemyAI {
  public float baseTurnRate;
  public float baseSpeed;
  public float minActiveDuration;
  public float cooldown;
  public float deltaAngleThreshold;
  public float desiredDistance;
  public float minDistance;

  public static EnemyAI Default = new EnemyAI() {
    baseTurnRate = 2.5f,
    baseSpeed = 10f,
    minActiveDuration = 2,
    cooldown = 2,
    deltaAngleThreshold = 15,
    desiredDistance = 5,
    minDistance = 8,
  };
}

public class EnemyAvatar : Avatar {
  public override float hpMax => _hpMax;
  public float _hpMax;
  public override string DisplayName => "Enemy";

  public EnemyAvatar(float hpMax = 25) {
    _hpMax = hpMax;
    hp = hpMax;
  }
}