using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[System.Serializable]

//https://www.youtube.com/watch?v=tBYl-aSxUe0

public class VRMap
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;
    Quaternion rotation = Quaternion.Euler(0, 18, 0);
    public void Map()
    {
        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        //rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
        rigTarget.forward =(new Vector3(0,0.1f,0) + Vector3.Lerp(rigTarget.forward, Vector3.ProjectOnPlane(vrTarget.forward, Vector3.up).normalized, Time.deltaTime * 15f)).normalized;  
    }
}




public class Body_headtracking : MonoBehaviour
{
    
    private PlayModeManager playModeManager;

    public Transform targetObject;
    private Transform targetOffset;

    private Vector3 cameraLinOffset;

    public float turnSmoothness;
    private Vector3 test;
    public VRMap head;

    // Start is called before the first frame update
    void Start()
    {
        //find manager to find out if we wanna use a tracker or no tracker
        GameObject gameobject = GameObject.Find("Manager");
        playModeManager = gameobject.GetComponent<PlayModeManager>();

        cameraLinOffset = transform.position - targetObject.position;

    // rotationOffset = transform.rotation - targetObject.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //if we dont use tracker and we are not currently rendering use body_headtracker
        if (playModeManager.playback == false && !playModeManager.UseSteamVRTracker)
        {

            GameObject.Find("Tracker").GetComponent<SteamVR_TrackedObject>().enabled = false;

            //transform.position = targetObject.position + cameraLinOffset;

            //transform.forward = Vector3.ProjectOnPlane(targetObject.up, Vector3.up).normalized;
            //Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(targetObject.up, Vector3.up).normalized, Time.deltaTime * turnSmoothness);

            head.Map();
        }
        else {

            GameObject.Find("Tracker").GetComponent<SteamVR_TrackedObject>().enabled = true;        
        }

    }
}

