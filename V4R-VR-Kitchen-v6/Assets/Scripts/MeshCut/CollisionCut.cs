using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CollisionCut : MonoBehaviour
{    
    //List<Vector3> v3orig = new List<Vector3>();
    //List<Vector3> v3dir = new List<Vector3>();

    private float cutCooldownAmount = 0.5f;
    private float cutCooldownTimer = 2.0f;

    private void Update()
    {
        if (cutCooldownTimer >= 0f)
        {
            cutCooldownTimer -= Time.deltaTime;
        }

    }

    /*private void Update()
    {
        for (int i = 0; i < v3orig.Count; i++)
        {
            Debug.DrawRay(v3orig[i], v3dir[i], Color.red);
        }

        if (cutCooldownTimer >= 0f)
        {
            cutCooldownTimer -= Time.deltaTime;
        }
        
        v3orig.Add(this.transform.position);
        v3dir.Add(this.transform.forward);
        //print(this.transform.forward);
    }*/

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            //v3orig.Add(contact.point);
            //v3dir.Add(contact.normal);
            
            ////Cutter.Cut(obj, centre, up,null,true,true);
            if (cutCooldownTimer <= 0)
            {
                //Debug.Log("other " + collision.collider.gameObject.name);
                GameObject newGO = Cutter.Cut(collision.collider.gameObject, contact.point, this.transform.forward, null, true, true);
                cutCooldownTimer = cutCooldownAmount;
            }
        }
    }
  
}
