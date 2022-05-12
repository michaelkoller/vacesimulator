using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class TapWater : MonoBehaviour
{
    public int emmisionRate; 
    public ParticleSystem tapWaterParticles;
    private int noCurrentlyTouching = 0;
   
    // Start is called before the first frame update
    void Start()
    {
        tapWaterParticles = GetComponentInChildren<ParticleSystem>();
        //print(tapWaterParticles.name);
        var emission = tapWaterParticles.emission;
        emission.rateOverTime = 0;
    }


    private void OnTriggerEnter(Collider other)
    {   print("wasserhahn: "+other.name);
        if (other.name.Contains("finger") || other.name.Contains("Sphere") || other.name.Contains("thumb"))
        {
            if (noCurrentlyTouching == 0)
            {
                var emission = tapWaterParticles.emission;
                if (emission.rateOverTime.constant == 0)
                {
                    emission.rateOverTime = emmisionRate;
                }
                else if (emission.rateOverTime.constant == emmisionRate)
                {
                    emission.rateOverTime = 0;
                }
            }
            noCurrentlyTouching++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("finger") || other.name.Contains("Sphere") || other.name.Contains("thumb"))
        {
            noCurrentlyTouching--;
        }    
    }
}
