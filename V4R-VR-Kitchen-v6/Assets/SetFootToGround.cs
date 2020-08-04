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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        leftFootTarget.transform.position = new Vector3(leftUpLeg.transform.position.x, 0.05f, leftUpLeg.transform.position.z);
        rightFootTarget.transform.position = new Vector3(rightUpLeg.transform.position.x, 0.05f, rightUpLeg.transform.position.z);
    }
}
