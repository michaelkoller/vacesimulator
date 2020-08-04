using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowVRCam : MonoBehaviour
{
    private GameObject VRCam;
    
    // Start is called before the first frame update
    void Start()
    {
        VRCam = GameObject.FindWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = VRCam.transform.position;
        this.transform.rotation = VRCam.transform.rotation;
    }
}
