using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;


//https://support.unity3d.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file-

public class RecordObjectPosRot : MonoBehaviour
{
    int currentFrame = 1;
    private string path;
    private string pathCut;
    private string pathRightHand;
    private string pathLeftHand;
    private string pathColorMap;
    //private GameObject[] allGameObjects;
    private Renderer[] allRenderers;
    //private GameObject[] allGameObjectsWithRenderer;
    private Dictionary<string, GameObject> allGameObjectsWithRendererDict = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> rightHandDict = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> leftHandDict = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject>[] dictArray;
    private GameObject leftHandParent;
    private GameObject rightHandParent;
    private List<string> delGosAfterCut;
    private List<GameObject> addGosAfterCut;
    private StringBuilder sb;
    private PlayModeManager playModeManager;
    private bool playback;
    private StringBuilder sbCut;
    private StringBuilder sbRightHand;
    private StringBuilder sbLeftHand;
    private StringBuilder sbColorMap;
    
    
    string GetPath()
    {
        return  playModeManager.sampleDir +@"\ReplayFiles\PositionAndOrientation\PO"+ currentFrame.ToString() +".txt";
    }
    string GetPathCut()
    {
        return  playModeManager.sampleDir +@"\ReplayFiles\Cuts\Cuts"+ currentFrame.ToString() +".txt";
    }
    
    string GetPathRightHand()
    {
        return  playModeManager.sampleDir +@"\ReplayFiles\RightHand\rhPO"+ currentFrame.ToString() +".txt";
    }
  
    string GetPathLeftHand()
    {
        return  playModeManager.sampleDir +@"\ReplayFiles\LeftHand\lhPO"+ currentFrame.ToString() +".txt";
    }
    
    string GetPathColorMap()
    {
        return  playModeManager.sampleDir +@"\RecordingsFiles\Annotations\Colormap\colormap"+ currentFrame.ToString() +".txt";
    }
    
    // Start is called before the first frame update
    void Start()
    {
        playModeManager = GetComponent<PlayModeManager>();
        playback = playModeManager.playback;
        addGosAfterCut = new List<GameObject>();
        delGosAfterCut = new List<string>();
        if (!playback)
        {
            path = GetPath();
            sb = new StringBuilder();
            pathCut = GetPathCut();
            sbCut = new StringBuilder();
            pathRightHand = GetPathRightHand();
            sbRightHand = new StringBuilder();
            pathLeftHand = GetPathLeftHand();
            sbLeftHand = new StringBuilder();
            pathColorMap = GetPathColorMap();
            sbColorMap = new StringBuilder();
            
            //allGameObjects = GameObject.FindObjectsOfType<GameObject>();
            allRenderers = FindObjectsOfType<Renderer>();
            //allGameObjectsWithRenderer = new GameObject[allRenderers.Length];
            for (int i = 0; i < allRenderers.Length; i++)
            {
                //allGameObjectsWithRenderer[i] = allRenderers[i].gameObject;
                allGameObjectsWithRendererDict.Add(allRenderers[i].gameObject.name, allRenderers[i].gameObject);
            }

            ObjectId[] objectIds = FindObjectsOfType<ObjectId>();
            foreach(ObjectId objectId in objectIds)
            {
                sbColorMap.AppendLine(objectId.objectName);
                sbColorMap.AppendLine(objectId.c.ToString());
            }
            File.WriteAllText(pathColorMap, sbColorMap.ToString());
            sbColorMap.Clear();
            
            dictArray = new Dictionary<string, GameObject>[3];
            dictArray[0] = allGameObjectsWithRendererDict;
            dictArray[1] = rightHandDict;
            dictArray[2] = leftHandDict;
            
        }
    }
    


    // Update is called once per frame
    void Update()
    {   if(!playback){
            if (rightHandParent == null)
            {
                rightHandParent = GameObject.Find("RightRenderModel Slim(Clone)");
                if (rightHandParent != null)
                {
                    Transform[] rightHandDescendants = rightHandParent.GetComponentsInChildren<Transform>();
                    foreach (Transform t in rightHandDescendants)
                    {    
                        rightHandDict.Add(t.gameObject.name, t.gameObject);
                    }
                }
            }

            if (leftHandParent == null)
            {
                leftHandParent = GameObject.Find("LeftRenderModel Slim(Clone)");
                if (leftHandParent != null)
                {
                    Transform[] leftHandDescendants = leftHandParent.GetComponentsInChildren<Transform>();
                    foreach (Transform t in leftHandDescendants)
                    {
                        leftHandDict.Add(t.gameObject.name, t.gameObject);
                    }
                }
            }


            //regular GOs
            sb.AppendLine("frame " + currentFrame.ToString());
            sb.AppendLine(Time.time.ToString());
            sb.AppendLine(Time.deltaTime.ToString());
            
            foreach(KeyValuePair<string,GameObject> goPair in allGameObjectsWithRendererDict)
            {
                sb.AppendLine(goPair.Key);
                sb.AppendLine(goPair.Value.transform.position.ToString("F3"));
                sb.AppendLine(goPair.Value.transform.eulerAngles.ToString("F3"));
            }
            
            
            //right hand
            sbRightHand.AppendLine("frame " + currentFrame.ToString());
            sbRightHand.AppendLine(Time.time.ToString());
            sbRightHand.AppendLine(Time.deltaTime.ToString());
            foreach(KeyValuePair<string,GameObject> goPair in rightHandDict)
            {
                sbRightHand.AppendLine(goPair.Key);
                sbRightHand.AppendLine(goPair.Value.transform.position.ToString("F3"));
                sbRightHand.AppendLine(goPair.Value.transform.eulerAngles.ToString("F3"));
            }
            sbRightHand.AppendLine("--stop--");
            
            //left hand
            sbLeftHand.AppendLine("frame " + currentFrame.ToString());
            sbLeftHand.AppendLine(Time.time.ToString());
            sbLeftHand.AppendLine(Time.deltaTime.ToString());
            foreach(KeyValuePair<string,GameObject> goPair in leftHandDict)
            {
                sbLeftHand.AppendLine(goPair.Key);
                sbLeftHand.AppendLine(goPair.Value.transform.position.ToString("F3"));
                sbLeftHand.AppendLine(goPair.Value.transform.eulerAngles.ToString("F3"));
            }
            sbLeftHand.AppendLine("--stop--");

            if (currentFrame % 200 == 0)
            {
                File.WriteAllText(path, sb.ToString());
                sb.Clear();
                path = GetPath();
                
                File.WriteAllText(pathRightHand, sbRightHand.ToString());
                sbRightHand.Clear();
                pathRightHand = GetPathRightHand();
                
                File.WriteAllText(pathLeftHand, sbLeftHand.ToString());
                sbLeftHand.Clear();
                pathLeftHand = GetPathLeftHand();
            }

            foreach (string s in delGosAfterCut)
            {
                allGameObjectsWithRendererDict.Remove(s);
            }
            delGosAfterCut.Clear();
            
            foreach (GameObject go in addGosAfterCut)
            {
                allGameObjectsWithRendererDict.Add(go.name, go);
            }
            addGosAfterCut.Clear();
            
            currentFrame++;
        }
    }

    public void RecordCut(string originalGameObjectName, Vector3 _contactPoint, Vector3 _direction, GameObject leftGO, GameObject rightGO, Material _cutMaterial = null, bool fill = true, bool _addRigidbody = false)
    {   
        sbCut.AppendLine("frame " + currentFrame.ToString());
        sbCut.AppendLine(originalGameObjectName);
        sbCut.AppendLine(_contactPoint.ToString("F3"));
        sbCut.AppendLine(_direction.ToString("F3"));
        File.AppendAllText(pathCut, sbCut.ToString());
        sbCut.Clear();
        delGosAfterCut.Add(originalGameObjectName);
        addGosAfterCut.Add(leftGO);
        addGosAfterCut.Add(rightGO);
        //allGameObjectsWithRendererDict.Remove(originalGameObjectName);
        //allGameObjectsWithRendererDict.Add(leftGO.name, leftGO);
        //allGameObjectsWithRendererDict.Add(rightGO.name, rightGO);
    }

    private void FlushRecordings()
    {
        File.WriteAllText(path, sb.ToString());
        sb.Clear();
        File.AppendAllText(pathCut, sbCut.ToString());
        sbCut.Clear();
        File.WriteAllText(pathRightHand, sbRightHand.ToString());
        sbRightHand.Clear();
        File.WriteAllText(pathLeftHand, sbLeftHand.ToString());
        sbLeftHand.Clear();
    }
    
    void OnDestroy()
    {
       FlushRecordings();
    }
}
