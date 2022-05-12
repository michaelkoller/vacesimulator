using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasStoveSwitch : MonoBehaviour
{
    private ParticleSystem stoveFlame;
    // Start is called before the first frame update
    void Start()
    {
        stoveFlame = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
