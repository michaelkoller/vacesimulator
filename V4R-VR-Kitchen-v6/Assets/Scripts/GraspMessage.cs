using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class GraspMessage : MonoBehaviour
{
    private RecordObjectPosRot recordScript;
    // Start is called before the first frame update
    void Start()
    {
        recordScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<RecordObjectPosRot>();
    }

    void OnAttachedToCircularDrive(Valve.VR.InteractionSystem.Hand hand)
    {
        //Debug.Log("ATTACHED CIRC "+ gameObject.transform.parent.name +" "+ gameObject.name + " " + hand.handType);
        recordScript.RecordGrasp(gameObject.transform.parent.name +"_"+ gameObject.name, hand.handType.ToString(), "Grasp");
    }
    
    void OnDetachedFromCircularDrive(Valve.VR.InteractionSystem.Hand hand)
    {
        //Debug.Log("DETACHED CIRC "+ gameObject.transform.parent.name +" "+ gameObject.name + " " + hand.handType);
        recordScript.RecordGrasp(gameObject.transform.parent.name +"_"+ gameObject.name, hand.handType.ToString(), "Release");
    }
    
    void OnAttachedToHand(Valve.VR.InteractionSystem.Hand hand)
    {
        //Debug.Log("ATTACHED "+ hand.AttachedObjects[0].attachedObject.name + " " + hand.handType);
        recordScript.RecordGrasp(hand.AttachedObjects[0].attachedObject.name, hand.handType.ToString(), "Grasp");
    }
    
    void OnDetachedFromHand(Valve.VR.InteractionSystem.Hand hand)
    {
        //Debug.Log("DETACHED "+ hand.AttachedObjects[0].attachedObject.name + " " + hand.handType);
        recordScript.RecordGrasp(hand.AttachedObjects[0].attachedObject.name, hand.handType.ToString(), "Release");
    }
}
