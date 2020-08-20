using System.Collections;
using System.Collections.Generic;
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
    private bool currentlyStepping;
    private Vector3 leftFootProjection;
    private bool stepWithLeftFoot;
    private Vector3 targetFootPosition;
    void Start()
    {
        anchorLeft = leftFootTarget.transform.position;
        stepThreshold = 0.3f;
        stopThreshold = 0.05f;
        currentlyStepping = false;
        stepWithLeftFoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        leftFootProjection = new Vector3(leftUpLeg.transform.position.x, 0.05f, leftUpLeg.transform.position.z);
        rightFootTarget.transform.position = new Vector3(rightUpLeg.transform.position.x, 0.05f, rightUpLeg.transform.position.z);


        if (!stepWithLeftFoot) //hind leg
        {
            if (Vector3.Distance(anchorLeft, leftFootProjection) > stepThreshold)
            {
                currentlyStepping = true;
            }

            if (Vector3.Distance(leftFootProjection, leftFootTarget.transform.position) < stopThreshold)
            {
                if (currentlyStepping)
                {
                    anchorLeft = leftFootTarget.transform.position;
                }

                currentlyStepping = false;
            }

            if (currentlyStepping)
            {
                leftFootTarget.transform.position =
                    Vector3.Lerp(leftFootTarget.transform.position, leftFootProjection, 0.1f);
            }
            else
            {
                leftFootTarget.transform.position = anchorLeft;
            }
        }
        else //front leg
        {
            if (Vector3.Distance(leftFootProjection, leftFootTarget.transform.position) > stopThreshold)
            {
                currentlyStepping = true;
                targetFootPosition = anchorLeft + ((0.3f * (leftFootProjection - anchorLeft))/ Vector3.Distance(leftFootProjection, anchorLeft));
            }
            if (Vector3.Distance(anchorLeft, leftFootProjection) > stepThreshold)
            {
                if (currentlyStepping)
                {
                    anchorLeft = targetFootPosition;
                }
                currentlyStepping = false;
            }
            
            if (currentlyStepping)
            {
                leftFootTarget.transform.position =
                    Vector3.Lerp(leftFootTarget.transform.position, leftFootProjection, 0.1f);
            }
            else
            {
                leftFootTarget.transform.position = anchorLeft;
            }
        }
    }
}
