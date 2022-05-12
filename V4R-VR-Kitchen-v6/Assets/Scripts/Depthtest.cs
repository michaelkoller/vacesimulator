using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Depthtest : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
