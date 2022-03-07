using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitManager : MonoBehaviour
{
    // public List<Fragment> fragments;

    public Circuit circuit;

    // Start is called before the first frame update
    void Start()
    {
        circuit = new Circuit(GameObject.FindObjectsOfType<Fragment>());
    }

    // Update is called once per frame
    void Update()
    {
        circuit.simulate(Time.deltaTime);
    }
}
