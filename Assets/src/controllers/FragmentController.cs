using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentController : MonoBehaviour {
  [NonSerialized]
  public Fragment fragment;
  [ReadOnly]
  public SpriteRenderer spriteRenderer;
  public SpriteRenderer manaCover;
  public SpriteMask mask;
  public GameObject input;
  public GameObject output;
  public static GameObject healthbarPrefab;
  [ReadOnly]
  public GameObject healthbar;
  public Rigidbody2D rb2d;

  private static int globalId = 0;
  public readonly int id = globalId++;
  private static readonly Color onGroundColor = new Color(0.5f, 0.5f, 0.5f);

  void Awake() {
    if (healthbarPrefab == null) {
      healthbarPrefab = Resources.Load<GameObject>("Healthbar");
    }
  }

  public virtual void Init(Fragment fragment) {
    this.fragment = fragment;
    fragment.controller = this;
    if (fragment.owner != null) {
      transform.SetParent(fragment.owner.controller.transform);
    } else {
      transform.SetParent(null);
    }
    UpdateRigidbody(GameModelController.main.isEditMode);
    if (fragment is Creature c) {
      UpdateOffset(c.startPosition);
      UpdateAngle(c.startAngle);
    } else {
      UpdateOffset(fragment.builtinOffset);
      UpdateAngle(fragment.builtinAngle);
    }
  }

  // call when owner or edit mode changes
  public void UpdateRigidbody(bool isEditMode) {
    // never modify creature Rigidbody type
    if (fragment is Creature) {
      return;
    }
    if (fragment.owner == null) {
      rb2d.bodyType = RigidbodyType2D.Dynamic;
      return;
    }

    // while playing, we are kinematic to allow the Creature owner's transform to apply
    if (!isEditMode) {
      rb2d.bodyType = RigidbodyType2D.Kinematic;
      return;
    }

    // We're in edit mode, and we're either player owned or enemy owned
    if (fragment.isPlayerOwned) {
      // we once again use dynamic so they hit each other when dragging
      rb2d.bodyType = RigidbodyType2D.Dynamic;
    } else {
      // for enemies, keep them kinematic
      rb2d.bodyType = RigidbodyType2D.Kinematic;
    }
  }

  internal void UpdateOwner(Creature owner) {
    // we might be very far away. What happens?
    // 1) we immediately go to their side, no problem
    // 2) teleport to your side in the same orientation (lets do this)
    
    // THIS WORKS FOR NOW
    Init(fragment);
  }

  void Start() {
    spriteRenderer = transform.Find("Sprite")?.GetComponent<SpriteRenderer>();
    if (spriteRenderer) {
      spriteRenderer.GetComponent<SpriteMask>().sprite = spriteRenderer.sprite;
    }
    if (manaCover != null) {
      manaCover.sprite = spriteRenderer.sprite;
    }
    if (output == null) {
      output = transform.Find("Output")?.gameObject;
    }
    if (!(fragment is Creature)) {
      healthbar = Instantiate(healthbarPrefab, Vector3.zero, Quaternion.identity, GameModelController.main.healthbars.transform);
      healthbar.GetComponent<HealthbarController>()?.Init(this);
    }
    mask.frontSortingOrder = id - 32766;
    mask.backSortingOrder = mask.frontSortingOrder - 1;
  }

  internal void UpdateOffset(Vector2 offset) {
    transform.localPosition = offset.z(transform.position.z);
  }

  internal void UpdateAngle(float value) {
    transform.localRotation = Quaternion.Euler(0, 0, fragment.builtinAngle);
  }

  public virtual void Update() {
    if (manaCover != null) {
      manaCover.material.SetFloat("_Percentage", fragment.Mana / fragment.manaMax);
      manaCover.color = fragment.owner == null ? onGroundColor : Color.white;
    }
    if (spriteRenderer != null) {
      spriteRenderer.color = fragment.owner == null ? onGroundColor : Color.white;
    }
    // sr.color = Color.Lerp(Color.black, Color.white, fragment.Mana / fragment.manaMax);
  }

  internal void Removed() {
    Destroy(gameObject);
    fragment.controller = null;
  }

  void OnDestroy() {
    if (healthbar) {
      Destroy(healthbar);
    }
  }

  void OnMouseDown() {
    EditModeInputController.instance.mouseDownOnFragment(this);
  }

  void OnMouseUp() {
    EditModeInputController.instance.mouseUpOnFragment(this);
  }
}
