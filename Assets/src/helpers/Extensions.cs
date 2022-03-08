using UnityEngine;

public static class MyVector3Extensions {
    public static Vector2 xy(this Vector3 v) {
        return new Vector2(v.x, v.y);
    }
    public static Vector3 z(this Vector2 v, float z = 0) {
        return new Vector3(v.x, v.y, z);
    }
}