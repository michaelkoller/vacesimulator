using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Timeline;
using UnityEngine;

public class RecordPlayerMovement : MonoBehaviour
{
    private GameObject rightHand;
    private GameObject leftHand;
    private GameObject head;
    private int frequency = 30;
    private double timeStep;
    private double timeStepsPassed;
    
    // Start is called before the first frame update
    void Start()
    {
        rightHand = GameObject.FindGameObjectWithTag("RightHand");
        leftHand = GameObject.FindGameObjectWithTag("LeftHand");
        head = GameObject.FindGameObjectWithTag("MainCamera");
        timeStep =  1.0 / frequency;
        timeStepsPassed = 0.0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time > timeStepsPassed)
        {
            timeStepsPassed += timeStep;
            //print("UPDATE " + Time.time);
            //print(head.transform.position.x.ToString() +  " " + head.transform.position.y.ToString() + " " + head.transform.position.z.ToString());
            /*
            head.transform.position.x;
            head.transform.position.y;
            head.transform.position.z;
            head.transform.rotation.w;
            head.transform.rotation.x;
            head.transform.rotation.y;
            head.transform.rotation.z;
            
            rightHand.transform.position.x;
            rightHand.transform.position.y;
            rightHand.transform.position.z;
            rightHand.transform.rotation.w;
            rightHand.transform.rotation.x;
            rightHand.transform.rotation.y;
            rightHand.transform.rotation.z;
            
            leftHand.transform.position.x;
            leftHand.transform.position.y;
            leftHand.transform.position.z;
            leftHand.transform.rotation.w;
            leftHand.transform.rotation.x;
            leftHand.transform.rotation.y;
            leftHand.transform.rotation.z; */
        }
     
    }
}
