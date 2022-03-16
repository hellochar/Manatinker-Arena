using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour {

  public Rigidbody2D rb2d;
  public FragmentController fragmentController;
  public GameObject letterE;
  public Player player => (Player)fragmentController.fragment;
  void Start() {
    if (rb2d == null) {
      rb2d = GetComponent<Rigidbody2D>();
    }
    if (fragmentController == null) {
      fragmentController = GetComponent<FragmentController>();
    }
    Camera.main.GetComponent<CameraFollowPlayer>().Player = this.gameObject;
    Camera.main.GetComponent<CameraFollowPlayer>().Teleport();
  }

  void Update() {
    letterE.transform.position = (GameModel.main.player.worldPos + Vector2.up).z(letterE.transform.position.z);
    letterE.transform.eulerAngles = Vector3.zero;

    UpdateAudio();
    if (GameModelController.main.isEditMode) {
      letterE.SetActive(false);
    } else if (GameModel.main.currentRound.state == GameRoundState.Preparing) {
      var hasNearbyFragment = GameModel.main.Fragments.Any(f => !(f is Creature) && f.owner == null && f.distance(GameModel.main.player) < 2);
      letterE.SetActive(hasNearbyFragment);
    } else {
      letterE.SetActive(false);
    }
    if (GameModelController.main.isEditMode || GameModelController.main.hasActiveAnimation) {
      return;
    }

    var dx = Input.GetAxis("Horizontal");
    var dy = Input.GetAxis("Vertical");
    player.setVelocityDirection(new Vector2(dx, dy));

    var s = new Vector2(Screen.width, Screen.height) / 2f;
    var mouseOffset = Input.mousePosition.xy() - s;
    var targetAngle = Vector2.SignedAngle(Vector2.right, mouseOffset);
    player.setRotation(targetAngle);
  }

  AudioSource aso;
  float asoFlow = 0;
  public float asoFlowVolume = 0.02f;
  void UpdateAudio() {
    if (aso == null) {
      aso = GetComponent<AudioSource>();
    }
    if (aso != null) {
      var inOutPercentSum = 0f;
      var num = 0;
      foreach(var c in player.Children) {
        inOutPercentSum += Mathf.Max(c.outputPercent, c.inputPercent);
        num++;
      }
      var overallFlow = inOutPercentSum / num;
      asoFlow = Mathf.Lerp(asoFlow, overallFlow, 0.05f);
      aso.volume = asoFlow * asoFlowVolume;
      aso.pitch = asoFlow;
    }
  }

  // void OnCollisionEnter2D(Collision2D collision) {
  //   Debug.Log("player: collision enter " + collision.gameObject);
  //   // Destroy(gameObject);
  // }

  // void OnTriggerEnter2D(Collider2D col) {
  //   Debug.Log("player: trigger enter " + col.gameObject);
  //   // if (col.gameObject.CompareTag("InLevel")) {
  //   //   var fragmentController = col.gameObject.GetComponent<FragmentController>();
  //   //   if (fragmentController != null) {
  //   //     // we've hit a fragment, deal damage
  //   //     Hit(fragmentController.fragment);
  //   //   }
  //   // }
  // }
}
