using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTickController : MonoBehaviour
{
    public float deathTime = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, deathTime);   
    }
}
