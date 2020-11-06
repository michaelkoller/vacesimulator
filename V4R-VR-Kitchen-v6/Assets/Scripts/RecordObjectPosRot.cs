using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine.UI;
using Valve.VR;

//https://support.unity3d.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file-
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
    private JSONDataStructures.PositionAndRotationFrameArr posRotFrameArr;
    private JSONDataStructures.CutRecords jsonCuts;
    private JSONDataStructures.GraspRecords jsonGrasps;
    private JSONDataStructures.PushRecords jsonPushes;
    private JSONDataStructures.InPredicateRecords jsonInPredicates;
    private JSONDataStructures.OnPredicateRecords jsonOnPredicates;
    public Dictionary<string, int> onPredicateStatusDict;
    public new HashSet<string> pushPredicateStatusSet;
    
    public SteamVR_Input_Sources myInputSource;
    public SteamVR_Action_Boolean clickRightAction;
    public SteamVR_Action_Boolean clickLeftAction;
    public SteamVR_Action_Boolean clickUpAction;
    public SteamVR_Action_Boolean clickDownAction;
    public SteamVR_Action_Boolean clickRecordAction;
    public SteamVR_Action_Boolean clickShowInstructions;
    public bool currentlyRecording = false;
    public GameObject recordSignGameObject;
    private Image recordSign;


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
    
    string GetPathCutJSON()
    {
        return  playModeManager.sampleDir +@"\RecordingsFiles\Annotations\Predicates\cuts.json";
    }
    
    string GetPathGraspJSON()
    {
        return  playModeManager.sampleDir +@"\RecordingsFiles\Annotations\Predicates\grasps.json";
    }
    
    string GetPathPushJSON()
    {
        return  playModeManager.sampleDir +@"\RecordingsFiles\Annotations\Predicates\push.json";
    }
    
    string GetPathInJSON()
    {
        return  playModeManager.sampleDir +@"\RecordingsFiles\Annotations\Predicates\in.json";
    }
    
    string GetPathOnJSON()
    {
        return  playModeManager.sampleDir +@"\RecordingsFiles\Annotations\Predicates\on.json";
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
    {   Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        playModeManager = GetComponent<PlayModeManager>();
        playback = playModeManager.playback;
        addGosAfterCut = new List<GameObject>();
        delGosAfterCut = new List<string>();
        onPredicateStatusDict = new Dictionary<string, int>();
        pushPredicateStatusSet = new HashSet<string>();
        
        //TODO maybe next line not needed:
        clickRightAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ClickRight");

        recordSign = recordSignGameObject.GetComponent<Image>();
        Debug.Log("RS " + recordSign);
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
            
            posRotFrameArr = new JSONDataStructures.PositionAndRotationFrameArr();
            jsonCuts = new JSONDataStructures.CutRecords();
            jsonGrasps = new JSONDataStructures.GraspRecords();
            jsonPushes = new JSONDataStructures.PushRecords();
            jsonInPredicates = new JSONDataStructures.InPredicateRecords();
            jsonOnPredicates = new JSONDataStructures.OnPredicateRecords();
            
            allRenderers = FindObjectsOfType<Renderer>();
            for (int i = 0; i < allRenderers.Length; i++)
            {
                allGameObjectsWithRendererDict.Add(allRenderers[i].gameObject.name, allRenderers[i].gameObject);
            }
            
            JSONDataStructures.ColormapJSON cm = new JSONDataStructures.ColormapJSON();

            ObjectId[] objectIds = FindObjectsOfType<ObjectId>();
            foreach(ObjectId objectId in objectIds)
            {
                sbColorMap.AppendLine(objectId.objectName);
                sbColorMap.AppendLine(objectId.c.r.ToString("G17")+", "+objectId.c.g.ToString("G17") 
                                      +", "+objectId.c.b.ToString("G17") +", "+objectId.c.a.ToString("G17"));
                sbColorMap.AppendLine(objectId.id.ToString());
                
                JSONDataStructures.ColormapEntryJSON cme = new JSONDataStructures.ColormapEntryJSON();
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

            SaveIntoJson<JSONDataStructures.ColormapJSON>(GetPathColorMapJSON(), cm);
            
            dictArray = new Dictionary<string, GameObject>[3];
            dictArray[0] = allGameObjectsWithRendererDict;
            dictArray[1] = rightHandDict;
            dictArray[2] = leftHandDict;
            
            particleSystems = FindObjectsOfType<ParticleSystem>();
            particleSystemDict = new Dictionary<string, ParticleSystem>();
            foreach(ParticleSystem ps in particleSystems)
            {
                particleSystemDict.Add(ps.gameObject.name, ps);
            }
            
            //InvokeRepeating("RecordFrame", 0.0f, 0.03333333333f);
        }
    }
    

    public void RecordFrame()
    {
        if(!playback){
            if (rightHandParent == null)
            {
                rightHandParent = GameObject.Find("RightRenderModel Slim(Clone)");
                if (rightHandParent != null)
                {
                    Transform[] rightHandDescendants = rightHandParent.GetComponentsInChildren<Transform>();
                    foreach (Transform t in rightHandDescendants)
                    {    
                        if(t.gameObject.name != "attach")
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
                        if(t.gameObject.name != "attach")
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
                sb.AppendLine(goPair.Value.transform.position.ToString("F3") );
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

    // Update is called once per frame
    void Update()
    {
        if (clickRecordAction.GetStateDown(myInputSource))
        {
            if (!currentlyRecording)
            {
                InvokeRepeating("RecordFrame", 0.0f, 0.03333333333f);
                Debug.Log("RECORDING");
                currentlyRecording = true;
                recordSign.enabled = true;
            }
            else
            {    
                CancelInvoke("RecordFrame");
                FlushRecordings();
                Debug.Log("RECORDING STOPPED");
                currentlyRecording = false;
                recordSign.enabled = false;
            }

        }
        //RecordFrame();
    }

    public void RecordCut(string originalGameObjectName, Vector3 _contactPoint, Vector3 _direction, GameObject leftGO, GameObject rightGO, Material _cutMaterial = null, bool fill = true, bool _addRigidbody = false)
    {   
        if (!currentlyRecording) return;

        sbCut.AppendLine("frame " + currentFrame.ToString());
        sbCut.AppendLine(originalGameObjectName);
        sbCut.AppendLine(_contactPoint.ToString("F3"));
        sbCut.AppendLine(_direction.ToString("F3"));
        File.AppendAllText(pathCut, sbCut.ToString());
        sbCut.Clear();
        delGosAfterCut.Add(originalGameObjectName);
        addGosAfterCut.Add(leftGO);
        addGosAfterCut.Add(rightGO);
        JSONDataStructures.CutRecord jsonCut = new JSONDataStructures.CutRecord();
        jsonCut.frame = currentFrame;
        jsonCut.name = originalGameObjectName;
        jsonCut.contact_point_x = _contactPoint.x;
        jsonCut.contact_point_y = _contactPoint.y;
        jsonCut.contact_point_z = _contactPoint.z;
        jsonCut.cut_direction_x = _direction.x;
        jsonCut.cut_direction_y = _direction.y;
        jsonCut.cut_direction_z = _direction.z;
        jsonCuts.cuts.Add(jsonCut);
    }

    public void RecordGrasp(string gameObjectName, string handName, string graspType)
    {
        if (!currentlyRecording) return;

        JSONDataStructures.GraspRecord jsonGrasp = new JSONDataStructures.GraspRecord();
        jsonGrasp.frame = currentFrame;
        jsonGrasp.grasp_type = graspType;
        jsonGrasp.grasped_object = gameObjectName;
        jsonGrasp.hand = handName;
        jsonGrasps.grasps.Add(jsonGrasp);
    }
    
    public void RecordPush(string gameObjectName, string handName, string pushType)
    {
        string key = currentFrame + " " + gameObjectName + " " + handName;
        if (pushPredicateStatusSet.Contains(key)) return;
        
        pushPredicateStatusSet.Add(key);
        /*bool record = false;
        if (pushType == "start_pushing")
        {
            if (!pushPredicateStatusDict.ContainsKey(key))
            {
                pushPredicateStatusDict.Add(key, 1);
                record = true;
            }else if (pushPredicateStatusDict[key] == 0)
            {
                pushPredicateStatusDict[key] += 1;
                record = true;
            }
            else
            {
                pushPredicateStatusDict[key] += 1;
            }
        }else if (pushType == "end_pushing")
        {
            if (!pushPredicateStatusDict.ContainsKey(key))
            {
                Debug.LogError("PushPredicateDict Counting Error");
            }
            else if(pushPredicateStatusDict[key] == 1)
            {
                pushPredicateStatusDict[key] = 0;
                record = true;
            }
            else
            {
                pushPredicateStatusDict[key] -= 1;
            }
        }
        */
        //if (record)
        //{
        JSONDataStructures.PushRecord jsonPush = new JSONDataStructures.PushRecord();
        jsonPush.frame = currentFrame;
        jsonPush.push_type = pushType;
        jsonPush.pushed_object = gameObjectName;
        jsonPush.hand = handName;
        jsonPushes.pushes.Add(jsonPush);
        //}
    }

    public void RecordInPredicate(string insideObject, string containerObject, string relationType) //relationType: entered or left
    {
        JSONDataStructures.InPredicateRecord jsonInPredicate = new JSONDataStructures.InPredicateRecord();
        jsonInPredicate.frame = currentFrame;
        jsonInPredicate.inside_object = insideObject;
        jsonInPredicate.container_object = containerObject;
        jsonInPredicate.relation_type = relationType;
        jsonInPredicates.inPredicateRecords.Add(jsonInPredicate);
    }
    
    public void RecordOnPredicate(string topObject, string bottomObject, string relationType) //relationType: start_touching or end_touching 
    {
        string key = topObject + " " + bottomObject;
        bool record = false;
        if (relationType == "start_touching")
        {
            if (!onPredicateStatusDict.ContainsKey(key))
            {
                onPredicateStatusDict.Add(key, 1);
                record = true;
            }else if (onPredicateStatusDict[key] == 0)
            {
                onPredicateStatusDict[key] += 1;
                record = true;
            }
            else
            {
                onPredicateStatusDict[key] += 1;
            }
        }else if (relationType == "end_touching")
        {
            if (!onPredicateStatusDict.ContainsKey(key))
            {
                Debug.LogError("OnPredicateDict Counting Error");
            }
            else if(onPredicateStatusDict[key] == 1)
            {
                onPredicateStatusDict[key] = 0;
                record = true;
            }
            else
            {
                onPredicateStatusDict[key] -= 1;
            }
            
        }
        if (record)
        {
            JSONDataStructures.OnPredicateRecord jsonOnPredicate = new JSONDataStructures.OnPredicateRecord();
            jsonOnPredicate.frame = currentFrame;
            jsonOnPredicate.top_object = topObject;
            jsonOnPredicate.bottom_object = bottomObject;
            jsonOnPredicate.relation_type = relationType;
            jsonOnPredicates.onPredicateRecords.Add(jsonOnPredicate);
        }
    }
    private void FlushRecordings()
    {
        File.AppendAllText(path, sb.ToString());
        sb.Clear();
        File.AppendAllText(pathCut, sbCut.ToString());
        sbCut.Clear();
        File.AppendAllText(pathRightHand, sbRightHand.ToString());
        sbRightHand.Clear();
        File.AppendAllText(pathLeftHand, sbLeftHand.ToString());
        sbLeftHand.Clear();
        File.AppendAllText(pathParticles, sbParticleSystems.ToString());
        sbParticleSystems.Clear();    
        
        SaveIntoJson<JSONDataStructures.CutRecords>(GetPathCutJSON(), jsonCuts);
        SaveIntoJson<JSONDataStructures.GraspRecords>(GetPathGraspJSON(), jsonGrasps);
        SaveIntoJson<JSONDataStructures.PushRecords>(GetPathPushJSON(), jsonPushes);
        SaveIntoJson<JSONDataStructures.InPredicateRecords>(GetPathInJSON(), jsonInPredicates);
        SaveIntoJson<JSONDataStructures.OnPredicateRecords>(GetPathOnJSON(), jsonOnPredicates);
        jsonCuts = null;
        jsonGrasps = null;
        jsonPushes = null;
        jsonInPredicates = null;
        jsonOnPredicates = null;
    }
    
    void OnDestroy() //not necessary anymore because of recording stop button
    {
//        if (!playModeManager.playback)
//        {
//            FlushRecordings();
//        }
    }
}
