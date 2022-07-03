using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHeadPosition : MonoBehaviour
{
    private PlayModeManager playModeManager;
    public Transform cameraTransform;
    public Camera_tracking script;
    // Start is called before the first frame update
    void Start()
    {
        GameObject gameobject = GameObject.Find("Manager");
        playModeManager = gameobject.GetComponent<PlayModeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.position = cameraTransform.position;
        if (!playModeManager.FirstPersonPerspective) { 
        transform.rotation = cameraTransform.rotation;
        }
    }
}
