using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundBoxByColor : MonoBehaviour
{
    public ObjectId[] objectIds;

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
        
   
    
    // Start is called before the first frame update
    void Start()
    {
        objectIds = FindObjectOfType<ColorByNumber>().objectIds;
        cam = GameObject.FindGameObjectWithTag("VideoCaptureCamera").GetComponent<Camera>();
        vidCapCanvas = GameObject.FindGameObjectWithTag("VideoCaptureCanvas").GetComponent<Canvas>();
        boundingBoxes = new GameObject[objectIds.Length];
        for (int i = 0; i< objectIds.Length; i++)
        {
            boundingBoxes[i] = (GameObject) Instantiate(Resources.Load("BBPrefab"));
            boundingBoxes[i].transform.SetParent(vidCapCanvas.transform, false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < objectIds.Length; i++)
        {
            Rect bbox = new Rect(objectIds[i].xMin, objectIds[i].yMax, objectIds[i].xMax - objectIds[i].xMin, objectIds[i].yMax - objectIds[i].yMin);
            RectTransform rt = boundingBoxes[i].GetComponent<RectTransform>();
            rt.anchoredPosition = bbox.position;
            rt.sizeDelta = bbox.size;
        }
    }
}
