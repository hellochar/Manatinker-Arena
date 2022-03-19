using System;
using UnityEngine;

public interface IActivatable {
  public virtual bool PlayerInputCheck() {
    return isHold ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
  }

  bool CanActivateInner();
  void Activate();
  virtual bool isHold => false;
}

public static class ActivatableExtensions {
  public static bool CanActivate(this IActivatable a) {
    var fragment = a as Fragment;
    var extraRestriction = fragment.isPlayerOwned ? !GameModelController.main.isEditMode : true;
    return a.CanActivateInner() && extraRestriction;
  }
}
