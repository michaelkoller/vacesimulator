using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


public class ObjectId : MonoBehaviour
{
    
    private static int classId = 1;
    public int id;
    public string objectName;
    public string objectType;
    public Color c;
    public int xMax;
    public int xMin;
    public int yMax;
    public int yMin;
    public bool makeGhost = true;

    public bool cuttable;

    public bool cuttingTool;

    public Vector3 cuttingPlaneNormal = Vector3.zero;
    
    //ghost for grabbing
    private List<GameObject> ghosts;

    private float cutCooldownAmount = 0.5f;
    private float lastCutTime = 0.0f;
    
    private void Awake()
    {
        id = classId;
        classId++;
        objectName = gameObject.name;
        //objectType = PrefabUtility.GetCorrespondingObjectFromSource(this.gameObject).name;
        c = new Color();
        c.r = id%10 *0.1f;
        c.g = id%100 * 0.01f;
        c.b = id*0.00390625f;
        c.a = 1.0f;
        xMax = int.MinValue;
        xMin = int.MaxValue;
        yMax = int.MinValue;
        yMin = int.MaxValue;
    }
    private void Start()
    {   
        this.MakeGhost();
        
        /*if (!makeGhost)
        {
            return;
        }
        ghosts = new List<GameObject>();
        MeshCollider[] mcs = GetComponentsInChildren<MeshCollider>();
        for (int i = 0; i < mcs.Length; i++)
        {
            GameObject ghost = (GameObject) Instantiate(Resources.Load("GhostPrefab"));
            ghost.transform.position = mcs[i].transform.position;
            ghost.transform.localScale = mcs[i].transform.localScale;
            ghost.transform.rotation = mcs[i].transform.rotation;
            ghost.transform.SetParent(mcs[i].gameObject.transform);
            ghost.name = mcs[i].gameObject.name + "Ghost";
            ConfigurableJoint cfgJoing = ghost.GetComponent<ConfigurableJoint>();
            //cfgJoing.connectedBody = mcs[i].transform.GetComponent<Rigidbody>();
            cfgJoing.connectedBody = transform.GetComponent<Rigidbody>();
            MeshCollider mC = ghost.GetComponent<MeshCollider>();
            mC.sharedMesh = mcs[i].sharedMesh;
        }*/
    }

    private void MakeGhost()
    {
        if (!this.makeGhost)
        {
            return;
        }
        ghosts = new List<GameObject>();
        MeshCollider[] mcs = GetComponentsInChildren<MeshCollider>();
        for (int i = 0; i < mcs.Length; i++)
        {
            GameObject ghost = (GameObject) Instantiate(Resources.Load("GhostPrefab"));
            ghost.transform.position = mcs[i].transform.position;
            ghost.transform.localScale = mcs[i].transform.localScale;
            ghost.transform.rotation = mcs[i].transform.rotation;
            ghost.transform.SetParent(mcs[i].gameObject.transform);
            ghost.name = mcs[i].gameObject.name + "Ghost";
            ConfigurableJoint cfgJoing = ghost.GetComponent<ConfigurableJoint>();
            //cfgJoing.connectedBody = mcs[i].transform.GetComponent<Rigidbody>();
            cfgJoing.connectedBody = transform.GetComponent<Rigidbody>();
            MeshCollider mC = ghost.GetComponent<MeshCollider>();
            mC.sharedMesh = mcs[i].sharedMesh;
        }
    }

    public void Update()
    {
        // if (gameObject.name == "Can")
        // {
        //     //Debug.Log(gameObject.transform.position);
        //     Component c = GetComponent<Transform>();
        //     Debug.Log(c.GetType());
        //     FieldInfo[] f = c.GetType().GetFields();
        //     Debug.Log(f[0]);
        //     //f[0].SetValue(c, new Vector3(1.0f, 1.0f, 1.0f));
        // }
    }
    
    void OnDrawGizmosSelected()
    {
        if (cuttingPlaneNormal != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, this.transform.position + cuttingPlaneNormal);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        ObjectId collisionObjectID = collision.collider.gameObject.GetComponent<ObjectId>();
        if (Time.time - lastCutTime > cutCooldownAmount && this.cuttingTool && collisionObjectID != null && collisionObjectID.cuttable)
        {   lastCutTime = Time.time;

            foreach (ContactPoint contact in collision.contacts)
            {
                GameObject newGO = Cutter.Cut(collision.collider.gameObject, contact.point, this.transform.forward, null, true, true);
                
                collision.collider.transform.GetChild(0).transform.GetComponent<MeshCollider>().sharedMesh = collision.collider.gameObject.GetComponent<MeshCollider>().sharedMesh;
                Debug.Log(collision.collider.gameObject.name);
                Debug.Log("child " +collision.collider.gameObject.transform.GetChild(0).gameObject.name);
                
                ObjectId newObjectId = newGO.AddComponent<ObjectId>();
                newObjectId.makeGhost = true;
                newObjectId.cuttable = true;
                lastCutTime = Time.time;
                
                //ONLY take first contact point per cut
                break;
            }
        }
    }
}
