using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InPredicate : MonoBehaviour
{
    private RecordObjectPosRot recordObjectPosRot;

    private List<ObjectId> enteredObjectIds;

    private List<ObjectId> exitedObjectIds;
    
    // Start is called before the first frame update
    void Start()
    {
        recordObjectPosRot = GameObject.FindGameObjectWithTag("Manager").GetComponent<RecordObjectPosRot>();
        enteredObjectIds = new List<ObjectId>();
        exitedObjectIds = new List<ObjectId>();
    }

    // Update is called once per frame
    void Update()
    {   if(enteredObjectIds.Count > 0) enteredObjectIds = new List<ObjectId>();
        if(exitedObjectIds.Count > 0) exitedObjectIds = new List<ObjectId>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Need to know the on properties already before starting to record.
        //Therefore no check for currentlyRecording
        //if (!recordObjectPosRot.currentlyRecording) return;
        
        ObjectId otherId = other.GetComponent<ObjectId>();
        if (otherId && !enteredObjectIds.Contains(otherId))
        {
            recordObjectPosRot.RecordInPredicate(other.name, this.name, "entered");
            enteredObjectIds.Add(otherId);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        //Need to know the on properties already before starting to record.
        //Therefore no check for currentlyRecording
        //if (!recordObjectPosRot.currentlyRecording) return;
        
        ObjectId otherId = other.GetComponent<ObjectId>();
        if (otherId &&!exitedObjectIds.Contains(otherId))
        {
            recordObjectPosRot.RecordInPredicate(other.name, this.name, "left");
            exitedObjectIds.Add(otherId);
        }
    }
}
