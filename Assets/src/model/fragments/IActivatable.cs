public interface IActivatable {
  bool CanActivateInner();
  void Activate();
}

public static class ActivatableExtensions {
  public static bool CanActivate(this IActivatable a) {
    var fragment = a as Fragment;
    var extraRestriction = fragment.isPlayerOwned ? !GameModelController.main.isEditMode : true;
    return a.CanActivateInner() && extraRestriction;
  }
}
