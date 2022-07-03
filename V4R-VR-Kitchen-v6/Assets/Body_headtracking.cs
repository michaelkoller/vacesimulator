using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[System.Serializable]

public class VRMap
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void Map()
    {
        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}




public class Body_headtracking : MonoBehaviour
{
    
    private PlayModeManager playModeManager;

    public Transform targetObject;
 

    private Vector3 cameraLinOffset;
    public Quaternion rotationOffset;

    

    public float turnSmoothness;

    public VRMap head;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameobject = GameObject.Find("Manager");
        playModeManager = gameobject.GetComponent<PlayModeManager>();

        


        cameraLinOffset = transform.position - targetObject.position;

    // rotationOffset = transform.rotation - targetObject.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (playModeManager.playback == false && !playModeManager.UseSteamVRTracker)
        {

            GameObject.Find("Tracker").GetComponent<SteamVR_TrackedObject>().enabled = false;

            transform.position = targetObject.position + cameraLinOffset;


            transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(targetObject.up, Vector3.up).normalized, Time.deltaTime * turnSmoothness);

            head.Map();
        }
        else {

            GameObject.Find("Tracker").GetComponent<SteamVR_TrackedObject>().enabled = true;        
        }

    }
}

