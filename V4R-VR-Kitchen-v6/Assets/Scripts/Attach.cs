using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attach : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Colliders")
        {
            other.transform.parent.transform.parent = transform;
        }
        else if (other.tag != "ChildDontReattach" && !other.name.Contains("Ghost") && !other.name.Contains("Sphere") 
                 && !other.name.Contains("finger") && !other.name.Contains("thumb"))       {
            other.gameObject.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Colliders")
        {
            other.transform.parent.transform.parent = null;
        }
        else if (other.tag != "ChildDontReattach" && !other.name.Contains("Ghost") && !other.name.Contains("Sphere") 
                 && !other.name.Contains("finger") && !other.name.Contains("thumb"))       {
            other.gameObject.transform.parent = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Colliders")
        {
            other.transform.parent.transform.parent = transform;
        }
        else if (other.tag != "ChildDontReattach" && !other.name.Contains("Ghost") && !other.name.Contains("Sphere") 
                 && !other.name.Contains("finger") && !other.name.Contains("thumb"))       {
            other.gameObject.transform.parent = transform;
        }
    }
}
