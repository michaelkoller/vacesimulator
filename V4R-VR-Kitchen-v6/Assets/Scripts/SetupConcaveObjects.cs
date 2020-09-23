using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SetupConcaveObjects : MonoBehaviour
{    
    List<NonConvexMeshCollider> nonConvexMeshColliders = new List<NonConvexMeshCollider>();
    public void Call()
    {
        nonConvexMeshColliders = GameObject.FindObjectsOfType<NonConvexMeshCollider>().ToList();
        int count = 0;
        foreach (var ncmc in nonConvexMeshColliders)
        {
            GameObject go = ncmc.gameObject;
            List<MeshCollider> mc = go.GetComponents<MeshCollider>().ToList();
            Rigidbody rb = go.GetComponent<Rigidbody>();
            Throwable thr = go.GetComponent<Throwable>();
            Interactable intractble = go.GetComponent<Interactable>();
            InPredicate inp = go.GetComponent<InPredicate>();
            if (!go.GetComponent<Rigidbody>() || mc.Count < 2 || !mc[0] || mc[0].enabled || !mc[0].convex || !mc[1] || !mc[1].enabled || !mc[1].convex || !mc[1].isTrigger || !rb || thr || intractble || !inp)
            {   if(go.name == "bin_1") continue;
                while(mc.Count < 2)
                {
                    mc.Add(go.AddComponent<MeshCollider>());
                }

                mc[0].enabled = false;
                mc[0].convex = true;
                mc[1].enabled = true;
                mc[1].convex = true;
                mc[1].isTrigger = true;
                
                DestroyImmediate(thr);
                DestroyImmediate(intractble);

                go.AddComponent<InPredicate>();
                
                Debug.Log(go);
                count++;
            }
                
        }
        Debug.Log(count + "/" + nonConvexMeshColliders.Count);
    }
}
