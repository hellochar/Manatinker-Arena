using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolController : MonoBehaviour
{
    // Start is called before the first frame update
    public Fragment fragment;
    public GameObject projectilePrefab;
    void Start()
    {
        fragment = GetComponent<Fragment>();
        fragment.Update = MyUpdate;
    }

    void MyUpdate() {
        if (Input.GetMouseButtonDown(0) && fragment.mana > 10) {
            fragment.mana -= 10;
            var p = Instantiate(projectilePrefab, this.transform.position, this.transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
