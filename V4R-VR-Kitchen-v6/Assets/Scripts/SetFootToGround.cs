using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SetFootToGround : MonoBehaviour
{   
    public Transform leftFootTarget;
    public Transform rightFootTarget;

    public Transform leftUpLeg;
    public Transform rightUpLeg;
    // Start is called before the first frame update

    private Vector3 anchorLeft;
    private float stepThreshold;
    private float stopThreshold;
    private float leadStepSize;
    private bool currentlySteppingTrail;
    private bool currentlySteppingLead;
    private Vector3 leftFootProjection;
    private bool stepWithLeftFoot;
    private Vector3 targetRightFootPosition;
    private Vector3 anchorRight;
    private Vector3 rightFootProjection;
    
    void Start()
    {
        anchorLeft = leftFootTarget.transform.position;
        anchorRight = rightFootTarget.transform.position;
        stepThreshold = 0.3f;
        stopThreshold = 0.05f;
        leadStepSize = 0.3f;
        currentlySteppingTrail = false;
        currentlySteppingLead = false;
        stepWithLeftFoot = false;
    }

    // Update is called once per frame
    void Update()
    {
        //leftFootProjection = new Vector3(leftUpLeg.transform.position.x, 0.05f, leftUpLeg.transform.position.z);
        //rightFootProjection = new Vector3(rightUpLeg.transform.position.x, 0.05f, rightUpLeg.transform.position.z);
        
        //Old way, just set feet to ground level
        rightFootTarget.transform.position = new Vector3(rightUpLeg.transform.position.x, 0.05f, rightUpLeg.transform.position.z);
        leftFootTarget.transform.position = new Vector3(leftUpLeg.transform.position.x, 0.05f, leftUpLeg.transform.position.z);

        
/*
        if (!stepWithLeftFoot) //hind leg
        {    
            //trailing leg
            if (Vector3.Distance(anchorLeft, leftFootProjection) > stepThreshold)
            {
                currentlySteppingTrail = true;
            }

            if (Vector3.Distance(leftFootProjection, leftFootTarget.transform.position) < stopThreshold)
            {
                if (currentlySteppingTrail)
                {
                    anchorLeft = leftFootTarget.transform.position;
                }

                currentlySteppingTrail = false;
            }

            if (currentlySteppingTrail)
            {
                leftFootTarget.transform.position =
                    Vector3.Lerp(leftFootTarget.transform.position, leftFootProjection, 0.1f);
            }
            else
            {
                leftFootTarget.transform.position = anchorLeft;
            }
            
            //leading leg
            
            //reset anchor in beginning of scene
            if (Vector3.Distance(anchorRight, rightFootTarget.transform.position) > 1.0f)
            {
                anchorRight = rightFootProjection;
            }            
            
            if (Vector3.Distance(anchorRight, rightFootProjection) > stopThreshold)
            {
                currentlySteppingLead = true;
            }

            if (Vector3.Distance(targetRightFootPosition, rightFootTarget.transform.position) <= stopThreshold)
            {
                if (currentlySteppingLead)
                {
                    anchorRight = rightFootTarget.transform.position;
                }
                currentlySteppingLead = false;
            }

            if (currentlySteppingLead)
            {
                targetRightFootPosition =  (leadStepSize * (rightFootProjection - anchorRight))/Vector3.Distance(rightFootProjection, anchorRight);
                targetRightFootPosition = anchorRight + new Vector3(targetRightFootPosition.x, 0.0f, targetRightFootPosition.z);
                rightFootTarget.transform.position =
                    Vector3.Lerp(rightFootTarget.transform.position, targetRightFootPosition, 0.1f);
            }
        }*/
    }
}
