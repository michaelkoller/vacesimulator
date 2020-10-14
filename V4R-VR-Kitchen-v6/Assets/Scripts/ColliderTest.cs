using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Entered Coll");
    }
    
    private void OnCollisionExit(Collision other)
    {
        Debug.Log("Exited Coll");
    }
}
