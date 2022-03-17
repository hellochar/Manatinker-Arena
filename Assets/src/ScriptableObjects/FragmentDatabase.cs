using UnityEngine;

[CreateAssetMenu()]
public class FragmentDatabase : ScriptableObject {
  public GameObject[] Items;
}


// public class MakeFragmentPrefabMapping2 {
//   [MenuItem("Assets/Create/FragmentPrefabMapping2")]
//   public static void CreateFragmentMapping2() {
//     var asset = ScriptableObject.CreateInstance<FragmentPrefabMapping2>();

//     AssetDatabase.CreateAsset(asset, "Assets/")
//   }
// }