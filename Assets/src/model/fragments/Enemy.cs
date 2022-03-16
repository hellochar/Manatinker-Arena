using UnityEngine;

public class Enemy : Creature {
  public EnemyAI ai;
  public Enemy(Vector2 start, EnemyAI ai) : base(start) {
    this.ai = ai;
  }
  public override float baseTurnRate => ai.baseTurnRate;
  public override float baseSpeed => ai.baseSpeed;
  public override float encumbranceThreshold => ai.encumbrance;

  public float cooldown = 0f;

  public override void Die() {
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
  public float encumbrance;
}

public class EnemyAvatar : Avatar {
  public override string DisplayName => "Enemy";
  public float _hpMax;
  public override float myHpMax => _hpMax;

  private float _outFlowRate;
  public override float outFlowRate => _outFlowRate;

  public EnemyAvatar(float hpMax, float _outFlowRate) {
    this._outFlowRate = _outFlowRate;
    _hpMax = hpMax;
    hp = hpMax;
  }

  public override void Die() {
    var goldEarned = GameModel.main.currentRound.roundNumber;
    if (controller != null) {
      for(int i = 0; i < goldEarned; i++) {
        var t = controller.transform.position + (Random.insideUnitCircle * 0.5f).z();
        var coin = UnityEngine.Object.Instantiate(VFX.Get("coin"), t, Quaternion.identity);
        coin.GetComponent<CoinController>().setDelay(i);
      }
      GameModel.main.player.gold += goldEarned;
    }
    base.Die();
  }
}