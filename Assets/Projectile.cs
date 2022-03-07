using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float baseSpeed;
    Rigidbody2D rb2d;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        var angle = this.transform.rotation.eulerAngles.z;
        rb2d.velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * baseSpeed;
        rb2d.SetRotation(angle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // void OnCollisionEnter2D(Collision2D collision) {
    //     Destroy(gameObject);
    // }

    void OnTriggerEnter2D(Collider2D col) {
        Destroy(gameObject);
    }
}
