using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPredicate : MonoBehaviour
{
    private RecordObjectPosRot recordObjectPosRot;
    //private List<ObjectId> currentPushList = new List<ObjectId>();
    
    void Start()
    {
        recordObjectPosRot = GameObject.FindGameObjectWithTag("Manager").GetComponent<RecordObjectPosRot>();
    }

    
    private void OnCollisionStay(Collision other)
    {
        if (!recordObjectPosRot.currentlyRecording) return;
        
        GameObject otherGo = other.gameObject;
        ObjectId otherObjectId = otherGo.GetComponentInParent<ObjectId>();
        if (otherObjectId != null)
        {
            recordObjectPosRot.RecordPush(otherObjectId.objectName, this.gameObject.name, "pushing");
        }
    }
    /*private void OnCollisionEnter(Collision other)
    {
        if (!recordObjectPosRot.currentlyRecording) return;
        
        GameObject otherGo = other.gameObject;
        ObjectId otherObjectId = otherGo.GetComponentInParent<ObjectId>();
        if (otherObjectId != null && !currentPushList.Contains(otherObjectId))
        {
            recordObjectPosRot.RecordPush(otherObjectId.objectName, this.gameObject.name, "start_pushing");
            currentPushList.Add(otherObjectId);
        }
   }

    private void OnCollisionExit(Collision other)
    {
        if (!recordObjectPosRot.currentlyRecording) return;

        GameObject otherGo = other.gameObject;
        ObjectId otherObjectId = otherGo.GetComponentInParent<ObjectId>();
        if (otherObjectId != null && currentPushList.Contains(otherObjectId))
        {
            recordObjectPosRot.RecordPush(otherObjectId.objectName, this.gameObject.name, "end_pushing");
            currentPushList.Remove(otherObjectId);
        }    
    }*/
}
