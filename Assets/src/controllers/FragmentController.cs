using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FragmentController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
  public static Color NO_FLOW_COLOR = new Color32(129, 150, 165, 255);
  public static Color FULL_FLOW_COLOR = Color.white;

  [NonSerialized]
  public Fragment fragment;
  public SpriteRenderer spriteRenderer;
  public SpriteMask mask;
  public GameObject input;
  public GameObject output;

  public static float HES = 2;

  public Bounds worldBounds() {
    return spriteRenderer.bounds;
  }
  public Vector2 worldSize() {
    return worldBounds().size.xy();
  }

  private static int globalId = 0;
  public readonly int id = globalId++;

  public virtual void Init(Fragment fragment) {
    this.fragment = fragment;
    fragment.controller = this;
    if (fragment.owner != null) {
      transform.SetParent(fragment.owner.controller.transform);
    } else {
      transform.SetParent(null);
    }
    // UpdateRigidbody(GameModelController.main.isEditMode);
    if (fragment is Creature c) {
      UpdateOffset(c.startPosition);
      UpdateAngle(c.startAngle);
    } else {
      UpdateOffset(fragment.builtinOffset);
      UpdateAngle(fragment.builtinAngle);
    }
  }

  // // call when owner or edit mode changes
  // public void UpdateRigidbody(bool isEditMode) {
  //   // never modify creature Rigidbody type
  //   if (fragment is Creature) {
  //     return;
  //   }
  //   if (fragment.owner == null) {
  //     rb2d.bodyType = RigidbodyType2D.Kinematic;
  //     return;
  //   }

  //   // while playing, we are kinematic to allow the Creature owner's transform to apply
  //   if (!isEditMode) {
  //     rb2d.bodyType = RigidbodyType2D.Kinematic;
  //     return;
  //   }

  //   // We're in edit mode, and we're either player owned or enemy owned
  //   if (fragment.isPlayerOwned) {
  //     // we once again use dynamic so they hit each other when dragging
  //     rb2d.bodyType = RigidbodyType2D.Dynamic;
  //   } else {
  //     // for enemies, keep them kinematic
  //     rb2d.bodyType = RigidbodyType2D.Kinematic;
  //   }
  // }

  internal void StopSelection() {
    Destroy(rb2d);
  }

  private Rigidbody2D rb2d;
  internal void StartSelection() {
    rb2d = gameObject.AddComponent<Rigidbody2D>();
    rb2d.bodyType = RigidbodyType2D.Dynamic;
    rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
    rb2d.drag = 100;
    // Instantiate(VFX.Get("pickupFragment"), transform.position, Quaternion.identity);
  }

  internal void UpdateOwner(Creature owner) {
    // we might be very far away. What happens?
    // 1) we immediately go to their side, no problem
    // 2) teleport to your side in the same orientation (lets do this)
    
    // THIS WORKS FOR NOW
    Init(fragment);
  }

  private SpriteRenderer inputSR;
  private SpriteRenderer outputSR;
  void Start() {
    if (aso == null) {
      aso = GetComponent<AudioSource>();
    }
    if (spriteRenderer == null) {
      spriteRenderer = transform.Find("Sprite")?.GetComponent<SpriteRenderer>();
    }
    if (spriteRenderer != null) {
      spriteRenderer.GetComponent<SpriteMask>().sprite = spriteRenderer.sprite;
    }
    if (output == null) {
      output = transform.Find("Output")?.gameObject;
    }
    mask.frontSortingOrder = id - 32766;
    mask.backSortingOrder = mask.frontSortingOrder - 1;

    inputSR = input.GetComponent<SpriteRenderer>();
    input.SetActive(fragment.hasInput);
    outputSR = output.GetComponent<SpriteRenderer>();
    output.SetActive(fragment.hasOutput);
  }

  internal void UpdateOffset(Vector2 offset) {
    var z = fragment is Creature ? -1 : fragment is EnergyShield ? 0.1f : 0;
    transform.localPosition = offset.z(z);
  }

  internal void UpdateAngle(float value) {
    transform.localRotation = Quaternion.Euler(0, 0, fragment.builtinAngle);
  }

  float currentFlowPercent;
  float currentInputPercent;
  float currentOutputPercent;
  public virtual void Update() {
    var flowPercent = Mathf.Max(fragment.outputPercent, fragment.inputPercent);
    currentFlowPercent = Mathf.Lerp(currentFlowPercent, flowPercent, 0.05f);
    if (spriteRenderer != null) {
      spriteRenderer.material.SetFloat("_Inflow", currentFlowPercent);
      spriteRenderer.material.SetFloat("_ManaPercentage", fragment.ManaPercent);
      spriteRenderer.material.SetFloat("_Intensity", fragment.Intensity);
      // spriteRenderer.color = fragment.owner == null ? unactivatedColor : Color.white;
    }
    if (fragment.hasInput) {
      // if (fragment.owner == null) {
      //   inputSR.color = unactivatedColor;
      // } else {
        currentInputPercent = Mathf.Lerp(currentInputPercent, (float)fragment.inputPercent * HES * 2, 0.05f);
        inputSR.color = Color.Lerp(NO_FLOW_COLOR, FULL_FLOW_COLOR, currentInputPercent);
      // }
    }
    if (fragment.hasOutput) {
      // if (fragment.owner == null) {
      //   outputSR.color = unactivatedColor;
      // } else {
        currentOutputPercent = Mathf.Lerp(currentOutputPercent, (float)fragment.outputPercent * HES * 2, 0.05f);
        outputSR.color = Color.Lerp(NO_FLOW_COLOR, FULL_FLOW_COLOR, currentOutputPercent);
      // }
    }
    UpdateInput();
  }

  public event Action OnActivated;
  void UpdateInput() {
    if (fragment is IActivatable a) {
      if (a.CanActivate() && fragment.isPlayerOwned && a.PlayerInputCheck()) {
        a.Activate();
        OnActivated?.Invoke();
      }
      if (a.isHold) {
        var playerInputCheck = fragment.isPlayerOwned && ((IActivatable)a).PlayerInputCheck();
        var enemyActivatedCheck = fragment.owner is Enemy e && e.activatedThisTurn(a);
        var playSound = (playerInputCheck || enemyActivatedCheck) && a.CanActivate();
        UpdateHoldAudio(playSound);
      }
    }
    if (fragment is Spike spike) {
      var playSound = ((IActivatable)spike).PlayerInputCheck() && spike.CanActivate() && spike.owner != null;
      if (playSound) {
        aso?.Play();
      }
    }
  }

  AudioSource aso;
  public void UpdateHoldAudio(bool active) {
    if (aso == null) {
      return;
    }
    if (active && !aso.isPlaying) {
      aso.Play();
    } else if (!active && aso.isPlaying) {
      aso.Pause();
    }
    aso.pitch = Mathf.Lerp(aso.pitch, active ? 1 : 0, 20 * Time.deltaTime);
  }


  internal void Removed() {
    var explode = Instantiate(VFX.Get("componentExplode"), transform.position, transform.rotation);
    var size = worldSize();
    var maxSize = Mathf.Max(size.x, size.y) * 1.6f;
    explode.transform.localScale *= maxSize;
    Destroy(gameObject);
    fragment.controller = null;
    
    var dirt = Instantiate(VFX.Get("dirt"), transform.position, transform.rotation);
    dirt.transform.localScale *= maxSize;
  }

  internal void OnHit(Projectile p, Vector2 position) {
    var dmgTick = Instantiate(VFX.Get("damageTick"), position.z(-2), Quaternion.identity);
    dmgTick.GetComponent<DamageTickController>().Init(p);
  }

  public void OnPointerDown(PointerEventData eventData) {
    EditModeInputController.instance.mouseDownOnFragment(this);
  }

  public void OnPointerUp(PointerEventData eventData) {
    EditModeInputController.instance.mouseUpOnFragment(this);
  }
}
