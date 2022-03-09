using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameModel {
  public static GameModel main;

  private bool _isEditMode = false;
  public bool isEditMode {
    get => _isEditMode;
    set {
      if (value) {
        // immediately set editmode to true
        _isEditMode = true;
      }
      var isFalse = value == false;
      GameModelController.main.UpdateIsEditMode(value, () => {
        if (isFalse) {
          _isEditMode = false;
        }
      });
    }
  }

  public Fragment player;
  public Circuit circuit = new Circuit();
  public Floor floor;

  public Action<Fragment> OnFragmentRemoved;
  public Action<Fragment> OnFragmentAdded;
  public Action<Wire> OnWireAdded;
  public Action<Wire> OnWireRemoved;

  public IEnumerable<Fragment> Fragments => circuit.Fragments;

  public IEnumerable<Wire> Wires => Fragments.SelectMany(f => f.wires);

  public GameModel() {
  }

  public void AddFragment(params Fragment[] fArr) {
    foreach (var f in fArr) {
      if (f is Player) {
        if (player != null) {
          throw new Exception("two players");
        }
        player = f;
      }
      circuit.AddFragment(f);
      OnFragmentAdded?.Invoke(f);
    }
  }

  public void RemoveFragment(Fragment f) {
    circuit.RemoveFragment(f);
    OnFragmentRemoved?.Invoke(f);
  }

  public static void SetMainToNewGame() {
    main = new GameModel();
    main.circuit = new Circuit();

    var player = new Player(new Vector2(5, 5));

    var core = new Core();
    core.owner = player;

    var pistol1 = new Pistol();
    pistol1.owner = player;
    pistol1.builtinOffset = new Vector2(1.5f, 0);
    core.connect(pistol1);

    var pistol2 = new Pistol();
    pistol2.owner = player;
    pistol2.builtinOffset = new Vector2(1.5f, 0.5f);
    core.connect(pistol2);

    var pistol3 = new Pistol();
    pistol3.owner = player;
    pistol3.builtinOffset = new Vector2(1.5f, -0.5f);
    core.connect(pistol3);

    main.AddFragment(player, core, pistol1, pistol2, pistol3);
    main.floor = new Floor(25, 25).surroundWithWalls();
  }

  void spawnEnemy() {
    var pos = new Vector2(Random.Range(2, floor.width - 2), Random.Range(2, floor.height - 2));
    var enemy = new Enemy(pos);

    var core = new Core();
    core.owner = enemy;

    var pistol1 = new Pistol();
    pistol1.owner = enemy;
    pistol1.builtinOffset = new Vector2(1.5f, 0);
    core.connect(pistol1);

    main.AddFragment(enemy, core, pistol1);
  }

  float timeUntilNextSpawn = 1;
  public void simulate(float dt) {
    timeUntilNextSpawn -= dt;
    if (timeUntilNextSpawn < 0) {
      timeUntilNextSpawn += 15;
      spawnEnemy();
    }
    circuit.simulate(dt);
  }
}

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

internal class Player : Creature {
  public Player(Vector2 start) : base("player-fragment", start) {
  }
}

internal class Enemy : Creature {
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