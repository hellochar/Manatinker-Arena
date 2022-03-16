using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAWhile : MonoBehaviour {
  static LinkedList<GameObject> pool = new LinkedList<GameObject>();
  public static int MAX_OBJECTS = 256;
  // Start is called before the first frame update
  void Start() {
      if (pool.Count >= MAX_OBJECTS) {
          var firstNode = pool.First;
          Destroy(firstNode.Value);
          pool.RemoveFirst();
      }
      pool.AddLast(gameObject);
  }
}
