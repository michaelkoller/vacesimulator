using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InPredicate : MonoBehaviour
{
    private RecordObjectPosRot recordObjectPosRot; 
    // Start is called before the first frame update
    void Start()
    {
        recordObjectPosRot = GameObject.FindGameObjectWithTag("Manager").GetComponent<RecordObjectPosRot>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ObjectId>())
        {
            recordObjectPosRot.RecordInPredicate(other.name, this.name, "entered");
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ObjectId>())
        {
            recordObjectPosRot.RecordInPredicate(other.name, this.name, "left");
        }
    }
}
