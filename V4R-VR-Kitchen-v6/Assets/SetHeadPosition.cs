using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHeadPosition : MonoBehaviour
{   
    public Transform cameraTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.position = cameraTransform.position;
        transform.rotation = cameraTransform.rotation;
    }
}
