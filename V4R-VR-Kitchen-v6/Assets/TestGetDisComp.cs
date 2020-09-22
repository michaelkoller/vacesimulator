using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGetDisComp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        MeshCollider otherMc = GameObject.Find("Sphere").GetComponent<MeshCollider>();
        MeshCollider thisMc = this.gameObject.AddComponent<MeshCollider>();
        thisMc.sharedMesh = otherMc.sharedMesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
