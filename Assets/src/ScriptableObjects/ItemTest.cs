using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemTest : ScriptableObject
{
    public string Name;
    public string[] tags;
    public int damage;
    public GameObject prefab;
}
