using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class OnPredicate : MonoBehaviour
{
    private ObjectId thisObjectId;
    private Vector3[] rayCastDirections = new Vector3[]{new Vector3(0f, -1f, 0f), 
                                                        new Vector3(-1f, -1f, 0f),
                                                        new Vector3(1f, -1f, 0f), 
                                                        new Vector3(0f, -1f, -1f), 
                                                        new Vector3(0f, -1f, 1f), 
                                                        new Vector3(-1f, -1f, -1f),
                                                        new Vector3(-1f, -1f, 1f),
                                                        new Vector3(1f, -1f, -1f),
                                                        new Vector3(1f, -1f, 1f),
                                                        new Vector3(-0.5f, -1f, 0f),
                                                        new Vector3(0.5f, -1f, 0f), 
                                                        new Vector3(0f, -1f, -0.5f), 
                                                        new Vector3(0f, -1f, 0.5f), 
                                                        new Vector3(-0.5f, -1f, -0.5f),
                                                        new Vector3(-0.5f, -1f, 0.5f),
                                                        new Vector3(0.5f, -1f, -0.5f),
                                                        new Vector3(0.5f, -1f, 0.5f) 
                                                        }; 
    
    private List<ObjectId> currentlyOnList = new List<ObjectId>();

    private NativeArray<RaycastHit> results;
    private NativeArray<RaycastCommand> commands;
    private RecordObjectPosRot recordObjectPosRot;
    private List<ObjectId> enteredIds;
    private List<ObjectId> exitedIds;
    
    // Start is called before the first frame update
    void Start()
    {
        thisObjectId = GetComponent<ObjectId>();
        recordObjectPosRot = GameObject.FindGameObjectWithTag("Manager").GetComponent<RecordObjectPosRot>();
        enteredIds = new List<ObjectId>();
        exitedIds = new List<ObjectId>();
    }

    private void Update()
    {
        if(enteredIds.Count > 0) enteredIds = new List<ObjectId>();
        if(exitedIds.Count > 0) exitedIds = new List<ObjectId>();
    }

    void OnCollisionEnter(Collision collision)
    {
        //Need to know the on properties already before starting to record.
        //Therefore no check for currentlyRecording
        //if (!recordObjectPosRot.currentlyRecording) return;

        GameObject otherGo = collision.gameObject;
        ObjectId otherObjectId = otherGo.GetComponentInParent<ObjectId>();
        
        foreach(Vector3 v in rayCastDirections){
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, v, out hit, 100))
            {
                // Debug.DrawRay(transform.position, v * hit.distance, Color.yellow);
                if (otherObjectId != null && otherObjectId == hit.transform.gameObject.GetComponentInParent<ObjectId>() && !currentlyOnList.Contains(otherObjectId))
                {
                    currentlyOnList.Add(otherObjectId);
                    string n = thisObjectId != null? thisObjectId. objectName : this.gameObject.name;
                    if (recordObjectPosRot == null)
                    {
                        recordObjectPosRot = GameObject.FindGameObjectWithTag("Manager").GetComponent<RecordObjectPosRot>();
                    }
                    recordObjectPosRot.RecordOnPredicate(n,otherObjectId.objectName, "start_touching");
                    //Debug.Log(n + " is on " + otherObjectId.objectName + " " + Time.time + " " + recordObjectPosRot.onPredicateStatusDict[n + " " + otherObjectId.objectName]);
                    
                    return;
                }
            }
        }
    }
    
    
    
    void OnCollisionExit(Collision collision)
    {
        //Need to know the on properties already before starting to record.
        //Therefore no check for currentlyRecording
        //if (!recordObjectPosRot.currentlyRecording) return;
        
        GameObject otherGo = collision.gameObject;
        ObjectId otherObjectId = otherGo.GetComponentInParent<ObjectId>();
        if (currentlyOnList.Contains(otherObjectId))
        {
            currentlyOnList.Remove(otherObjectId);
            //Debug.Log(thisObjectId.objectName + " is not anymore on " + otherObjectId.objectName);
            recordObjectPosRot.RecordOnPredicate(thisObjectId.objectName,otherObjectId.objectName, "end_touching");
            //Debug.Log(gameObject.name + " is NOT " + otherObjectId.objectName + " " + Time.time + " " + recordObjectPosRot.onPredicateStatusDict[thisObjectId.objectName + " " +otherObjectId.objectName]);
        }
    }
}

//https://docs.unity3d.com/2018.1/Documentation/ScriptReference/RaycastCommand.html?_ga=2.107706180.751313826.1600419919-1458406070.1578654659


