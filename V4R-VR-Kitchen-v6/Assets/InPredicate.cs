using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InPredicate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ObjectId>())
        {
            Debug.Log(other.name + " ENTERED " + this.name);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ObjectId>())
        {
            Debug.Log(other.name + " LEFT " + this.name);
        }
    }
}
