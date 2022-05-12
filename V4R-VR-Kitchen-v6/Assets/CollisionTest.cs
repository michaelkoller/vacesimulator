using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(gameObject.name + " collides with " + other.gameObject.name);
    }
    
    private void OnCollisionStay(Collision other)
    {
        Debug.Log(gameObject.name + " stays with " + other.gameObject.name);
    }
    
    private void OnCollisionExit(Collision other)
    {
        Debug.Log(gameObject.name + " exits " + other.gameObject.name);
    }
}
