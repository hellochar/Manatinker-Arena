using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentController : MonoBehaviour {
  [NonSerialized]
  public Fragment fragment;
  public SpriteRenderer manaCover;
  public GameObject input;
  public GameObject output;
  public static GameObject healthbarPrefab;
  [ReadOnly]
  public GameObject healthbar;

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
    }
    if (fragment is Creature c) {
      transform.position = c.startPosition.z(transform.position.z);
    } else {
      UpdateOffset(fragment.builtinOffset);
      UpdateAngle(fragment.builtinAngle);
    }
  }

  void Start() {
    if (manaCover != null) {
      var sr = transform.Find("Sprite").GetComponent<SpriteRenderer>();
      manaCover.sprite = sr.sprite;
    }
    if (input == null) {
      input = transform.Find("Input")?.gameObject;
    }
    if (output == null) {
      output = transform.Find("Output")?.gameObject;
    }
    if (!(fragment is Creature)) {
      healthbar = Instantiate(healthbarPrefab, Vector3.zero, Quaternion.identity, GameModelController.main.healthbars.transform);
      healthbar.GetComponent<HealthbarController>()?.Init(this);
    }
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
    }
    // sr.color = Color.Lerp(Color.black, Color.white, fragment.Mana / fragment.manaMax);
  }

  internal void Removed() {
    Destroy(gameObject);
    fragment.controller = null;
  }

  void OnMouseDown() {
    EditModeInputController.instance.mouseDownOnFragment(this);
  }

  void OnMouseUp() {
    EditModeInputController.instance.mouseUpOnFragment(this);
  }
}
