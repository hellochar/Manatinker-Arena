using UnityEngine;

public static class MyVector3Extensions {
    public static Vector2 xy(this Vector3 v) {
        return new Vector2(v.x, v.y);
    }
    public static float angleDeg(this Vector2 v) {
        return Vector2.SignedAngle(Vector2.right, v);
    }

    public static Vector3 z(this Vector2 v, float z = 0) {
        return new Vector3(v.x, v.y, z);
    }
}

public static class StringExtensions
{
    public static string AddColor(this string text, Color col) => $"<color={ColorHexFromUnityColor(col)}>{text}</color>";
    public static string ColorHexFromUnityColor(this Color unityColor) => $"#{ColorUtility.ToHtmlStringRGBA(unityColor)}";
}