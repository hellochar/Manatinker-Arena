public interface IActivatable {
  bool CanActivateInner();
  void Activate();
}

public static class ActivatableExtensions {
  public static bool CanActivate(this IActivatable a) {
    return a.CanActivateInner() && !GameModel.main.isEditMode;
  }
}
