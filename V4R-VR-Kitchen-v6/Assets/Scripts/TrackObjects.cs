using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class TrackObjects : MonoBehaviour
{
    private Rect bbox;
    private static Camera cam;
    private Canvas vidCapCanvas;
    private GameObject[] trackObjects;
    private GameObject[] boundingBoxes;
    private GameObject[] staticTrackObjects;
    private GameObject[] staticBoundingBoxes;
    public GameObject bbDisp;
    private int trackObjectsAmount;
    private int staticTrackObjectsAmount;
    
    public static Rect GUI3dRectWithObject(GameObject go)
    {

        Vector3 cen = go.GetComponent<Renderer>().bounds.center;
        Vector3 ext = go.GetComponent<Renderer>().bounds.extents;
        Vector2[] extentPoints = new Vector2[8]
        {
            WorldToGUIPoint(new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z - ext.z)),
            WorldToGUIPoint(new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z - ext.z)),
            WorldToGUIPoint(new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z + ext.z)),
            WorldToGUIPoint(new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z + ext.z)),
            WorldToGUIPoint(new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z - ext.z)),
            WorldToGUIPoint(new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z - ext.z)),
            WorldToGUIPoint(new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z + ext.z)),
            WorldToGUIPoint(new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z + ext.z))
        };
        Vector2 min = extentPoints[0];
        Vector2 max = extentPoints[0];
        foreach (Vector2 v in extentPoints)
        {
            min = Vector2.Min(min, v);
            max = Vector2.Max(max, v);
        }

        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }

    public static Rect GUI2dRectWithObject(GameObject go)
    {
        //Vector3[] vertices = go.GetComponent<MeshFilter>().mesh.vertices;
        MeshFilter[] foundMeshFilters = go.GetComponentsInChildren<MeshFilter>();
        int vertAmount = 0;
        for (int i = 0; i < foundMeshFilters.Length; i++)
        {
            vertAmount += foundMeshFilters[i].mesh.vertices.Length;
        }
        Vector3[] vertices = new Vector3[vertAmount];
        int vertIndex = 0;
        for (int i = 0; i < foundMeshFilters.Length; i++)
        {
            Vector3[] tfVerts= new Vector3[foundMeshFilters[i].mesh.vertices.Length];
            Vector3[] oldVerts = foundMeshFilters[i].mesh.vertices;
            for (int j = 0; j < foundMeshFilters[i].mesh.vertices.Length; j++)
            {
                tfVerts[j] = foundMeshFilters[i].transform.TransformPoint(oldVerts[j]);
            }
            tfVerts.CopyTo(vertices, vertIndex);
            vertIndex += foundMeshFilters[i].mesh.vertices.Length;
        } 
            
        float x1 = float.MaxValue, y1 = float.MaxValue, x2 = 0.0f, y2 = 0.0f;

        foreach (Vector3 vert in vertices)
        {
            //Vector2 tmp = WorldToGUIPoint(go.transform.TransformPoint(vert));
            Vector2 tmp = WorldToGUIPoint(vert);

            if (tmp.x < x1) x1 = tmp.x;
            if (tmp.x > x2) x2 = tmp.x;
            if (tmp.y < y1) y1 = tmp.y;
            if (tmp.y > y2) y2 = tmp.y;
        }

        Rect bbox = new Rect(x1, y1, x2 - x1, y2 - y1);
        //Debug.Log(bbox);
        return bbox;
    }

    public static Vector2 WorldToGUIPoint(Vector3 world)
    {
        Vector2 screenPoint = cam.WorldToScreenPoint(world);
        //screenPoint.y = (float) Screen.height - screenPoint.y;
        return screenPoint;
    }

    public void Start()
    {
        cam = GameObject.FindGameObjectWithTag("VideoCaptureCamera").GetComponent<Camera>();
        vidCapCanvas = GameObject.FindGameObjectWithTag("VideoCaptureCanvas").GetComponent<Canvas>();

        trackObjects = GameObject.FindGameObjectsWithTag("TrackObject");
        trackObjectsAmount = trackObjects.Length;
        boundingBoxes = new GameObject[trackObjectsAmount];
        for (int i = 0; i< trackObjectsAmount; i++)
        {
            boundingBoxes[i] = (GameObject) Instantiate(Resources.Load("BBPrefab"));
            boundingBoxes[i].transform.SetParent(vidCapCanvas.transform, false);
        }
        
        staticTrackObjects = GameObject.FindGameObjectsWithTag("StaticTrackObject");
        staticTrackObjectsAmount = staticTrackObjects.Length;
        staticBoundingBoxes = new GameObject[staticTrackObjectsAmount];
        for (int i = 0; i< staticTrackObjectsAmount; i++)
        {
            staticBoundingBoxes[i] = (GameObject) Instantiate(Resources.Load("BBPrefab"));
            staticBoundingBoxes[i].transform.SetParent(vidCapCanvas.transform, false);
        }    
        
        for (int i = 0; i < staticTrackObjectsAmount; i++)
        {
            SetBB(staticTrackObjects, staticBoundingBoxes, i);
        }
    }

    public void SetBB(GameObject[] tempTrackObjects, GameObject[] tempBoundingBoxes, int i)
    {
        bbox = GUI2dRectWithObject(tempTrackObjects[i]);
//        print("BBOXCOORD "+ bbox);

        //bbox.y = -bbox.y;
        RectTransform rt = tempBoundingBoxes[i].GetComponent<RectTransform>();
        rt.anchoredPosition = bbox.position;
        rt.sizeDelta = bbox.size;
    }
    
    public void Update()
    {
        for (int i = 0; i < trackObjectsAmount; i++)
        {
            SetBB(trackObjects, boundingBoxes, i);
        }
//        bbox = GUI2dRectWithObject(GameObject.FindGameObjectsWithTag("TrackObject")[0]);
////        print("BBOXCOORD "+ bbox);
//
//        //bbox.y = -bbox.y;
//        RectTransform rt = bbDisp.GetComponent<RectTransform>();
//        rt.anchoredPosition = bbox.position;
//        rt.sizeDelta = bbox.size;
    }
}