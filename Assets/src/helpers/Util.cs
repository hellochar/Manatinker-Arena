using UnityEngine;

public static class Util {
  public static float MapLinear(float x, float in_min, float in_max, float out_min, float out_max) {
    return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
  }

  public static float Snap(float v, float factor) {
    return Mathf.Round(v / factor) * factor;
  }

  public static Vector2 Snap(Vector2 v, float factor) {
    return new Vector2(Snap(v.x, factor), Snap(v.y, factor));
  }

  public static Vector2 fromDeg(float deg) {
    return new Vector2(Mathf.Cos(deg * Mathf.Deg2Rad), Mathf.Sin(deg * Mathf.Deg2Rad));
  }

  public static int Temporal(float value) {
    float rand = value % 1;
    return Random.value < rand ? Mathf.FloorToInt(value) : Mathf.CeilToInt(value);
  }
}
