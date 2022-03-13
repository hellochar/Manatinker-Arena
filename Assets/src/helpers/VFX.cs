using System.Collections.Generic;
using UnityEngine;

public static class VFX {
  public static Dictionary<string, GameObject> prefabCache = new Dictionary<string, GameObject>();
  public static GameObject Get(string name) {
    if (!prefabCache.ContainsKey(name)) {
      prefabCache.Add(name, Resources.Load<GameObject>($"VFX/{name}/{name}"));
    }
    return prefabCache[name];
  }
}