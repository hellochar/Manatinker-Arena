using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentController : MonoBehaviour {
  [NonSerialized]
  public Fragment fragment;
  public SpriteRenderer spriteRenderer;
  public SpriteRenderer manaCover;
  public SpriteMask mask;
  public GameObject input;
  public GameObject output;

  public static float HES = 2;

  public Vector2 worldSize() {
    return spriteRenderer.bounds.size.xy();
  }

  private static int globalId = 0;
  public readonly int id = globalId++;
  public static readonly Color unactivatedColor = new Color32(95, 96, 102, 255);
  // public static readonly Color unactivatedColor = Color.white;

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
    if (spriteRenderer == null) {
      spriteRenderer = transform.Find("Sprite")?.GetComponent<SpriteRenderer>();
    }
    if (spriteRenderer != null) {
      spriteRenderer.GetComponent<SpriteMask>().sprite = spriteRenderer.sprite;
    }
    if (manaCover != null) {
      manaCover.sprite = spriteRenderer.sprite;
    }
    if (output == null) {
      output = transform.Find("Output")?.gameObject;
    }
    mask.frontSortingOrder = id - 32766;
    mask.backSortingOrder = mask.frontSortingOrder - 1;

    if (input != null) {
      inputSR = input.GetComponent<SpriteRenderer>();
      input.SetActive(fragment.hasInput);
    }
    if (output != null) {
      outputSR = output.GetComponent<SpriteRenderer>();
      output.SetActive(fragment.hasOutput);
    }
  }

  internal void UpdateOffset(Vector2 offset) {
    var z = fragment is Creature ? -1 : 0;
    transform.localPosition = offset.z(z);
  }

  internal void UpdateAngle(float value) {
    transform.localRotation = Quaternion.Euler(0, 0, fragment.builtinAngle);
  }

  float currentFlowPercent;
  float currentInputPercent;
  float currentOutputPercent;
  public virtual void Update() {
    if (manaCover != null) {
      manaCover.material.SetFloat("_Percentage", fragment.Mana / fragment.manaMax);
      manaCover.color = fragment.owner == null ? unactivatedColor : Color.white;
    }
    if (spriteRenderer != null) {
      var flowPercent = Mathf.Max(fragment.outputPercent, fragment.inputPercent);
      currentFlowPercent = Mathf.Lerp(currentFlowPercent, flowPercent, 0.05f);
      spriteRenderer.material.SetFloat("_Percentage", currentFlowPercent * HES);
      spriteRenderer.color = fragment.owner == null ? unactivatedColor : Color.white;
    }
    if (input != null) {
      if (fragment.owner == null) {
        inputSR.color = unactivatedColor;
      } else {
        currentInputPercent = Mathf.Lerp(currentInputPercent, (float)fragment.inputPercent * HES, 0.05f);
        inputSR.color = Color.Lerp(Color.black, Color.white, currentInputPercent);
      }
    }
    if (output != null) {
      if (fragment.owner == null) {
        outputSR.color = unactivatedColor;
      } else {
        currentOutputPercent = Mathf.Lerp(currentOutputPercent, (float)fragment.outputPercent * HES, 0.05f);
        outputSR.color = Color.Lerp(Color.black, Color.white, currentOutputPercent);
      }
    }
    // sr.color = Color.Lerp(Color.black, Color.white, fragment.Mana / fragment.manaMax);
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
    Instantiate(VFX.Get("damageTick"), position, Quaternion.identity);
  }
}
