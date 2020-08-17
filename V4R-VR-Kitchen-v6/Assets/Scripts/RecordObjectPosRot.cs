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
    //private GameObject[] allGameObjects;
    private Renderer[] allRenderers;
    //private GameObject[] allGameObjectsWithRenderer;
    private Dictionary<string, GameObject> allGameObjectsWithRendererDict = new Dictionary<string, GameObject>();
    private List<string> delGosAfterCut;
    private List<GameObject> addGosAfterCut;
    private StringBuilder sb;
    private PlayModeManager playModeManager;
    private bool playback;
    private StringBuilder sbCut;
    string GetPath()
    {
        return  playModeManager.sampleDir +@"\ReplayFiles\PositionAndOrientation\PO"+ currentFrame.ToString() +".txt";
    }
    string GetPathCut()
    {
        return  playModeManager.sampleDir +@"\ReplayFiles\Cuts\Cuts"+ currentFrame.ToString() +".txt";
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
            //allGameObjects = GameObject.FindObjectsOfType<GameObject>();
            allRenderers = FindObjectsOfType<Renderer>();
            //allGameObjectsWithRenderer = new GameObject[allRenderers.Length];
            for (int i = 0; i < allRenderers.Length; i++)
            {
                //allGameObjectsWithRenderer[i] = allRenderers[i].gameObject;
                allGameObjectsWithRendererDict.Add(allRenderers[i].gameObject.name, allRenderers[i].gameObject);
            }
            path = GetPath();
            sb = new StringBuilder();
            pathCut = GetPathCut();
            sbCut = new StringBuilder();
        }
    }



    // Update is called once per frame
    void Update()
    {   if(!playback){
            sb.AppendLine("frame " + currentFrame.ToString());
            sb.AppendLine(Time.time.ToString());
            sb.AppendLine(Time.deltaTime.ToString());
            foreach(KeyValuePair<string,GameObject> goPair in allGameObjectsWithRendererDict)
            {
                sb.AppendLine(goPair.Key);
                sb.AppendLine(goPair.Value.transform.position.ToString("F3"));
                sb.AppendLine(goPair.Value.transform.eulerAngles.ToString("F3"));
            }

            if (currentFrame % 200 == 0)
            {
                File.WriteAllText(path, sb.ToString());
                sb.Clear();
                path = GetPath();
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
}
