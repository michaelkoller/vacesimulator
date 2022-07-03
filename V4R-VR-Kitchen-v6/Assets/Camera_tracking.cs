using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_tracking : MonoBehaviour
{
    private PlayModeManager playModeManager;

    public Transform targetObject;
    private bool First_Person;
    public Vector3 cameraLinOffset;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameobject = GameObject.Find("Manager");
        playModeManager = gameobject.GetComponent<PlayModeManager>();


    }

    // Update is called once per frame
    void Update()
    {
        First_Person = playModeManager.FirstPersonPerspective;
        if (playModeManager.FirstPersonPerspective) { 
        Vector3 newPosition = targetObject.transform.position + cameraLinOffset;
        transform.position = newPosition;

        transform.rotation = targetObject.rotation;
       
        }
    }
}
