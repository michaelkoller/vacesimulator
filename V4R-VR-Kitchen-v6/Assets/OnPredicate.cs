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
    
    // Start is called before the first frame update
    void Start()
    {
        thisObjectId = GetComponent<ObjectId>();
        recordObjectPosRot = GameObject.FindGameObjectWithTag("Manager").GetComponent<RecordObjectPosRot>();
    }

    // Update is called once per frame
    void Update()
    {
        // foreach(Vector3 v in rayCastDirections){
        //     RaycastHit hit;
        //     // Does the ray intersect any objects excluding the player layer
        //     if (Physics.Raycast(transform.position, v, out hit, 100))
        //     {
        //         Debug.DrawRay(transform.position, v * hit.distance, Color.yellow);
        //         Debug.Log(v + " did Hit " + hit.collider.gameObject.GetComponent<ObjectId>().objectName);
        //     }
        //     else
        //     {
        //         Debug.DrawRay(transform.position, v * 1000, Color.white);
        //     }
        // }
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     results = new NativeArray<RaycastHit>(rayCastDirections.Length, Allocator.TempJob);
    //     commands = new NativeArray<RaycastCommand>(rayCastDirections.Length, Allocator.TempJob);
    //     GameObject otherGo = collision.gameObject;
    //     ObjectId otherObjectId = otherGo.GetComponentInParent<ObjectId>();
    //     for(int i = 0; i < rayCastDirections.Length; i++)
    //     {
    //         commands[0] = new RaycastCommand(transform.position, rayCastDirections[i], 100, maxHits:1);
    //     }
    //     var handle = RaycastCommand.ScheduleBatch(commands, results, 1);
    //     handle.Complete();
    //
    //     for (int i = 0; i < results.Length; i++)
    //     {
    //         RaycastHit hit = results[i];
    //         if (hit.transform != null)
    //         {
    //             if (otherObjectId != null && otherObjectId == hit.transform.gameObject.GetComponentInParent<ObjectId>() && !currentlyOnList.Contains(otherObjectId))
    //             {
    //                 currentlyOnList.Add(otherObjectId);
    //                 Debug.Log(thisObjectId.objectName + " is on " + otherObjectId.objectName);
    //                 break;
    //             }
    //         }
    //     }
    //     
    //     results.Dispose();
    //     commands.Dispose();
    // }

    //Single Raycast:
    void OnCollisionEnter(Collision collision)
    {
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
                    recordObjectPosRot.RecordOnPredicate(n,otherObjectId.objectName, "start_touching");
                    //Debug.Log(n + " is on " + otherObjectId.objectName);
                    break;
                }
            }
        }
    }
    
    void OnCollisionExit(Collision collision)
    {
        GameObject otherGo = collision.gameObject;
        ObjectId otherObjectId = otherGo.GetComponentInParent<ObjectId>();
        if (currentlyOnList.Contains(otherObjectId))
        {
            currentlyOnList.Remove(otherObjectId);
            //Debug.Log(thisObjectId.objectName + " is not anymore on " + otherObjectId.objectName);
            recordObjectPosRot.RecordOnPredicate(thisObjectId.objectName,otherObjectId.objectName, "end_touching");
        }
    }
}

//https://docs.unity3d.com/2018.1/Documentation/ScriptReference/RaycastCommand.html?_ga=2.107706180.751313826.1600419919-1458406070.1578654659


