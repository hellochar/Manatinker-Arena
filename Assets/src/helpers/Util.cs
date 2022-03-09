using UnityEngine;

public static class Util {
  public static float MapLinear(float x, float in_min, float in_max, float out_min, float out_max) {
    return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
  }

  public static Vector2 Snap(Vector2 v, float factor) {
    return new Vector2(Mathf.Round(v.x / factor) * factor, Mathf.Round(v.y / factor) * factor);
  }
}