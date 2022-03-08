using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolController : MonoBehaviour
{
    // Start is called before the first frame update
    public Fragment fragment;
    public GameObject projectilePrefab;
    public Transform spawnPoint;
    void Start()
    {
        fragment = GetComponent<Fragment>();
        fragment.Update = MyUpdate;
    }

    void MyUpdate() {
        if (Input.GetMouseButtonDown(0) && fragment.Mana > 10) {
            fragment.ChangeMana(-10);
            var p = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
            p.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
