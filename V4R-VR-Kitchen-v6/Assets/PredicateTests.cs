using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;


public class PredicateTests : MonoBehaviour
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
    private List<ObjectId> enteredIds;
    private List<ObjectId> exitedIds;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
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
                    Debug.Log("start_touching");
                    return;
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
            Debug.Log("end_touching");
        }
    }
}
