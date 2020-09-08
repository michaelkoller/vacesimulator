using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;

//https://support.unity3d.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file-


[System.Serializable]
public class PositionAndRotationFrameArr
{
    public string type = "position_and_rotation_frame_arr";
    public string recipe = "";
    public string creation_time = "";
    public List<PositionAndRotationFrame> positionAndRotationFrameArr = new List<PositionAndRotationFrame>();
}

[System.Serializable]
public class PositionAndRotationFrame
{
    public string type = "position_and_rotation_frame";
    public int frame_number = 0;
    public float time = 0.0f;
    public float delta_time = 0.0f;
    public List<ObjectPosition> objectPositionAndRotationArr = new List<ObjectPosition>();
}

[System.Serializable]
public class ObjectPosition
{
    public string type = "object_pos";
    public string name = "";
    public float posX = 0.0f;
    public float posY = 0.0f;
    public float posZ = 0.0f;
    public float angX = 0.0f;
    public float angY = 0.0f;
    public float angZ = 0.0f;
}


[System.Serializable]
public class ColormapJSON
{
    public List<ColormapEntryJSON> object_colors = new List<ColormapEntryJSON>();
}

[System.Serializable]
public class ColormapEntryJSON
{
    public string name = "";
    public byte r = 0;
    public byte g = 0;
    public byte b = 0;
    public byte a = 0;
    public int id_no = -1;
}


[System.Serializable]
public class BbFrameArray
{
    public List<BbFrame> bb_frame_arr = new List<BbFrame>();
}

[System.Serializable]
public class BbFrame
{
    public int frame_number = -1;
    public List<BbObject> bb_obect_arr = new List<BbObject>();
}

[System.Serializable]
public class BbObject
{
    public string name = "";
    public int id_no = -1;
    public int x_max = -1;
    public int x_min = -1;
    public int y_max = -1;
    public int y_min = -1;
}

public class RecordObjectPosRot : MonoBehaviour
{
    int currentFrame = 1;
    private string path;
    private string pathCut;
    private string pathRightHand;
    private string pathLeftHand;
    private string pathColorMap;
    private string pathPosRotJSON;
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
    private StringBuilder sbParticleSystems;
    private ParticleSystem [] particleSystems;
    private Dictionary<string, ParticleSystem> particleSystemDict;
    private string pathParticles;
    private PositionAndRotationFrameArr posRotFrameArr;

    public static void SaveIntoJson<T>(string path, T jsonstruct){
        string jsonstructstring = JsonUtility.ToJson(jsonstruct);
        System.IO.File.WriteAllText(path, jsonstructstring);
    }
    
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
    string GetPathColorMapJSON()
    {
        return  playModeManager.sampleDir +@"\RecordingsFiles\Annotations\Colormap\colormap.json";
    }
    
    string GetPathParticles()
    {
        return  playModeManager.sampleDir +@"\ReplayFiles\Particles\particles"+ currentFrame.ToString() +".txt";
    }
    
    string GetPosRotJSONPath()
    {
        return  playModeManager.sampleDir +@"\RecordingsFiles\Annotations\PoseAndOrientation\position_orientation_"+ currentFrame.ToString() + ".json";
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
            pathParticles = GetPathParticles();
            sbParticleSystems = new StringBuilder();
            pathPosRotJSON = GetPosRotJSONPath();
            
            posRotFrameArr = new PositionAndRotationFrameArr();

            //allGameObjects = GameObject.FindObjectsOfType<GameObject>();
            allRenderers = FindObjectsOfType<Renderer>();
            //allGameObjectsWithRenderer = new GameObject[allRenderers.Length];
            for (int i = 0; i < allRenderers.Length; i++)
            {
                //allGameObjectsWithRenderer[i] = allRenderers[i].gameObject;
                allGameObjectsWithRendererDict.Add(allRenderers[i].gameObject.name, allRenderers[i].gameObject);
            }
            
            ColormapJSON cm = new ColormapJSON();

            ObjectId[] objectIds = FindObjectsOfType<ObjectId>();
            foreach(ObjectId objectId in objectIds)
            {
                sbColorMap.AppendLine(objectId.objectName);
                sbColorMap.AppendLine(objectId.c.ToString());
                sbColorMap.AppendLine(objectId.id.ToString());
                
                ColormapEntryJSON cme = new ColormapEntryJSON();
                cme.name = objectId.objectName;
                cme.r = objectId.c.r;
                cme.g = objectId.c.g;
                cme.b = objectId.c.b;
                cme.a = objectId.c.a;
                cme.id_no = objectId.id;
                cm.object_colors.Add(cme);
            }
            File.WriteAllText(pathColorMap, sbColorMap.ToString());
            sbColorMap.Clear();

            SaveIntoJson<ColormapJSON>(GetPathColorMapJSON(), cm);
            
            dictArray = new Dictionary<string, GameObject>[3];
            dictArray[0] = allGameObjectsWithRendererDict;
            dictArray[1] = rightHandDict;
            dictArray[2] = leftHandDict;
            
            particleSystems = FindObjectsOfType<ParticleSystem>();
            particleSystemDict = new Dictionary<string, ParticleSystem>();
            foreach(ParticleSystem ps in particleSystems)
            {
                particleSystemDict.Add(ps.gameObject.name, ps);
                Debug.Log("PARTICLE " + ps.gameObject.name);
            }
            
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
            
            //JSON
            PositionAndRotationFrame posRotFrame = new PositionAndRotationFrame();
            posRotFrameArr.positionAndRotationFrameArr.Add(posRotFrame);
            posRotFrame.frame_number = currentFrame;
            posRotFrame.time = Time.time;
            posRotFrame.delta_time = Time.deltaTime;
            
            foreach(KeyValuePair<string,GameObject> goPair in allGameObjectsWithRendererDict)
            {
                ObjectPosition op = new ObjectPosition();
                op.name = goPair.Key;
                op.posX = goPair.Value.transform.position.x;
                op.posY = goPair.Value.transform.position.y;
                op.posZ = goPair.Value.transform.position.z;
                op.angX = goPair.Value.transform.eulerAngles.x;
                op.angY = goPair.Value.transform.eulerAngles.y;
                op.angZ = goPair.Value.transform.eulerAngles.z;
                posRotFrame.objectPositionAndRotationArr.Add(op);
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
            
            sbParticleSystems.AppendLine("frame " + currentFrame.ToString());
            sbParticleSystems.AppendLine(Time.time.ToString());
            sbParticleSystems.AppendLine(Time.deltaTime.ToString());
            
            foreach(KeyValuePair<string,ParticleSystem> psPair in particleSystemDict)
            {
                sbParticleSystems.AppendLine(psPair.Key);
                sbParticleSystems.AppendLine(psPair.Value.emission.rateOverTime.constant.ToString());
            }

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
                
                File.WriteAllText(pathParticles, sbParticleSystems.ToString());
                sbParticleSystems.Clear();
                pathParticles = GetPathParticles();
                
                SaveIntoJson<PositionAndRotationFrameArr>(pathPosRotJSON, posRotFrameArr);
                posRotFrameArr = new PositionAndRotationFrameArr();
                pathPosRotJSON = GetPosRotJSONPath();

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
        File.WriteAllText(pathParticles, sbParticleSystems.ToString());
        sbParticleSystems.Clear();    
        
        SaveIntoJson<PositionAndRotationFrameArr>(pathPosRotJSON, posRotFrameArr);
    }
    
    void OnDestroy()
    {
        if (!playModeManager.playback)
        {
            FlushRecordings();
        }
    }
}
