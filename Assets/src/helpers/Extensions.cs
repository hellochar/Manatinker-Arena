using UnityEngine;

public static class MyVector3Extensions {
    public static Vector2 xy(this Vector3 v) {
        return new Vector2(v.x, v.y);
    }

}