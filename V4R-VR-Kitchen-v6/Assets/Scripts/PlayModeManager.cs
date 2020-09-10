using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.iOS;
using Valve.VR;
using Object = System.Object;




public class PlaybackState
{   
  
    public string replayDir;
    public string pathPO;
    public string pathCut;
    public string pathRightHand;
    public string pathLeftHand;
    public string pathParticles;
    public int frameNumber;
    public int nextFrameForCutting = -1;
    public float time;
    public float deltaTime;
    private DirectoryInfo di;
    private DirectoryInfo diRightHand;
    private DirectoryInfo diLeftHand;
    private DirectoryInfo diParticles;
    private FileInfo[] fileInfosPO;
    private FileInfo[] fileInfosCuts;
    private FileInfo[] fileInfosRightHand;
    private FileInfo[] fileInfosLeftHand;
    private FileInfo[] fileInfosParticles;
    private StreamReader reader;
    private int currPathPOfileNumber;
    private int poFileCount;
    private StreamReader cutReader;
    private StreamReader readerRightHand;
    private StreamReader readerLeftHand;
    private StreamReader readerParticles;
    private Dictionary<string, GameObject> rightHandDict = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> leftHandDict = new Dictionary<string, GameObject>();
    private Dictionary<string, ParticleSystem> particleSystemDict = new Dictionary<string, ParticleSystem>();
    private GameObject leftHandParent;
    private GameObject rightHandParent;
    public bool playbackFinished;
    
    private Renderer[] allRenderers;
    private GameObject[] allGameObjectsWithRenderer;
    private Dictionary<string, GameObject> gameObjectDict = new Dictionary<string, GameObject>();
    
    public PlaybackState(string replayDir, string[] excludeFromPOFileGOs, GameObject rightHandParent, GameObject leftHandParent)
    { allRenderers = GameObject.FindObjectsOfType<Renderer>();
        allGameObjectsWithRenderer = new GameObject[allRenderers.Length];
        for (int i = 0; i < allRenderers.Length; i++)
        {
            if (!excludeFromPOFileGOs.Contains(allRenderers[i].gameObject.name))
            {
                allGameObjectsWithRenderer[i] = allRenderers[i].gameObject;
                gameObjectDict.Add(allRenderers[i].gameObject.name, allRenderers[i].gameObject);
            }
        }        
        this.replayDir =  replayDir;
        this.pathPO = replayDir + @"\PositionAndOrientation";
        this.pathCut = replayDir + @"\Cuts";
        pathRightHand = replayDir + @"\RightHand";
        pathLeftHand = replayDir + @"\LeftHand";
        pathParticles = replayDir + @"\Particles";
        
        di = new DirectoryInfo(pathPO);
        diRightHand = new DirectoryInfo(pathRightHand);
        diLeftHand = new DirectoryInfo(pathLeftHand);
        fileInfosPO = di.GetFiles().OrderBy(p => p.CreationTime).ToArray();
        fileInfosRightHand = diRightHand.GetFiles().OrderBy(p => p.CreationTime).ToArray();
        fileInfosLeftHand = diLeftHand.GetFiles().OrderBy(p => p.CreationTime).ToArray();
        currPathPOfileNumber = 0;
        poFileCount = fileInfosPO.Length;
        di = new DirectoryInfo(pathCut);
        fileInfosCuts = di.GetFiles();
        diParticles = new DirectoryInfo(pathParticles);
        fileInfosParticles = diParticles.GetFiles().OrderBy(p => p.CreationTime).ToArray();;

        GetNextPOFile();
        cutReader = new StreamReader(fileInfosCuts[0].FullName); //there is only one cut file per sample
        //get frame number of first cut
        if (!cutReader.EndOfStream)
        {
            nextFrameForCutting = int.Parse(cutReader.ReadLine().Trim().Split(' ')[1]);
        }
        else
        {
            nextFrameForCutting = -1;
        }

        this.rightHandParent = rightHandParent;
        this.leftHandParent = leftHandParent;
        
        Transform[] rightHandDescendants = rightHandParent.GetComponentsInChildren<Transform>();
        foreach (Transform t in rightHandDescendants)
        {    
            rightHandDict.Add(t.gameObject.name, t.gameObject);
        }
        
        Transform[] leftHandDescendants = leftHandParent.GetComponentsInChildren<Transform>();
        foreach (Transform t in leftHandDescendants)
        {
            leftHandDict.Add(t.gameObject.name, t.gameObject);
        }
        
        ParticleSystem[] particleSystems = GameObject.FindObjectsOfType<ParticleSystem>();
        foreach(ParticleSystem ps in particleSystems)
        {
            particleSystemDict.Add(ps.gameObject.name, ps);
        }

        playbackFinished = false;
    }

    private void GetNextPOFile()
    {
        this.reader = new StreamReader(fileInfosPO[currPathPOfileNumber].FullName);
        readerRightHand = new StreamReader(fileInfosRightHand[currPathPOfileNumber].FullName);
        readerLeftHand = new StreamReader(fileInfosLeftHand[currPathPOfileNumber].FullName);
        readerParticles = new StreamReader(fileInfosParticles[currPathPOfileNumber].FullName);
        currPathPOfileNumber++;
    }

    public void GetStateOfFrame()
    {
        if (this.reader.EndOfStream && currPathPOfileNumber < poFileCount)
        {
            GetNextPOFile();
        }
        else if(this.reader.EndOfStream)
        {
            playbackFinished = true;
            return;
        }
        
        //Regular Objects
        string frameNumberString = reader.ReadLine().Trim().Split(' ')[1];
        this.frameNumber = int.Parse(frameNumberString);
        this.time = float.Parse(reader.ReadLine().Trim());
        this.deltaTime = float.Parse(reader.ReadLine().Trim());
        for (int i = 0; i < gameObjectDict.Count; i++)
        {
            string name = reader.ReadLine().Trim().Split((' '))[0];
            string[] pos = reader.ReadLine().Trim('(').Trim(')').Split(',');
            string[] rot = reader.ReadLine().Trim('(').Trim(')').Split(',');
            //Debug.Log(i);
            //Debug.Log(pos);
            //Debug.Log(rot);
            float[] posFloat = new float[3];
            float[] rotFloat = new float[4];
            for (int j = 0; j < 3; j++)
            {
                posFloat[j] = float.Parse(pos[j]);
            }
            for (int j = 0; j < 3; j++)
            {
                rotFloat[j] = float.Parse(rot[j]);
            }
            this.gameObjectDict[name].transform.position = new Vector3(posFloat[0],posFloat[1],posFloat[2]);
            this.gameObjectDict[name].transform.eulerAngles = new Vector3(rotFloat[0],rotFloat[1],rotFloat[2]);
        }

        //Right Hand
        string r = readerRightHand.ReadLine();
        frameNumberString = r.Trim().Split(' ')[1];
        this.frameNumber = int.Parse(frameNumberString); //Watch out for problems if these numbers don't match in the regular obj, righthand, lefthand
        this.time = float.Parse(readerRightHand.ReadLine().Trim());
        this.deltaTime = float.Parse(readerRightHand.ReadLine().Trim());
        for (int i = 0; i < rightHandDict.Count; i++)
        {
            string name = readerRightHand.ReadLine().Trim();
            if (name == "--stop--")
            { break;
            }
            string[] pos = readerRightHand.ReadLine().Trim('(').Trim(')').Split(',');
            string[] rot = readerRightHand.ReadLine().Trim('(').Trim(')').Split(',');
            float[] posFloat = new float[3];
            float[] rotFloat = new float[4];
            for (int j = 0; j < 3; j++)
            {
                posFloat[j] = float.Parse(pos[j]);
            }
            for (int j = 0; j < 3; j++)
            {
                rotFloat[j] = float.Parse(rot[j]);
            }
            this.rightHandDict[name].transform.position = new Vector3(posFloat[0],posFloat[1],posFloat[2]);
            this.rightHandDict[name].transform.eulerAngles = new Vector3(rotFloat[0],rotFloat[1],rotFloat[2]);

            if (i == rightHandDict.Count - 1)
            {
                readerRightHand.ReadLine();
            }
        }

        //Left Hand
        frameNumberString = readerLeftHand.ReadLine().Trim().Split(' ')[1];
        this.frameNumber = int.Parse(frameNumberString); //Watch out for problems if these numbers don't match in the regular obj, righthand, lefthand
        this.time = float.Parse(readerLeftHand.ReadLine().Trim());
        this.deltaTime = float.Parse(readerLeftHand.ReadLine().Trim());
        for (int i = 0; i < leftHandDict.Count; i++)
        {
            string name = readerLeftHand.ReadLine().Trim();
            if (name == "--stop--")
            {
                break;
            }
            string[] pos = readerLeftHand.ReadLine().Trim('(').Trim(')').Split(',');
            string[] rot = readerLeftHand.ReadLine().Trim('(').Trim(')').Split(',');
            float[] posFloat = new float[3];
            float[] rotFloat = new float[4];
            for (int j = 0; j < 3; j++)
            {
                posFloat[j] = float.Parse(pos[j]);
            }
            for (int j = 0; j < 3; j++)
            {
                rotFloat[j] = float.Parse(rot[j]);
            }
            this.leftHandDict[name].transform.position = new Vector3(posFloat[0],posFloat[1],posFloat[2]);
            this.leftHandDict[name].transform.eulerAngles = new Vector3(rotFloat[0],rotFloat[1],rotFloat[2]);
            
            if (i == leftHandDict.Count - 1)
            {
                readerLeftHand.ReadLine();
            }
        }
        
        //Particles
        //TODO this solution currently doesn't handle slower replay times. Look if this is ok, when you stop time when you render videos.
        frameNumberString = readerParticles.ReadLine().Trim().Split(' ')[1];
        this.frameNumber = int.Parse(frameNumberString); //Watch out for problems if these numbers don't match in the regular obj, righthand, lefthand
        this.time = float.Parse(readerParticles.ReadLine().Trim());
        this.deltaTime = float.Parse(readerParticles.ReadLine().Trim());
        for (int i = 0; i < particleSystemDict.Count; i++)
        {
            string name = readerParticles.ReadLine().Trim();
            float emissionRate = float.Parse(readerParticles.ReadLine().Trim());
            ParticleSystem.EmissionModule em = particleSystemDict[name].emission;
            em.rateOverTime = new ParticleSystem.MinMaxCurve(emissionRate);
        }
        
        //Cuts
        while ((nextFrameForCutting == frameNumber) && !cutReader.EndOfStream)
        {
            string nameCut = cutReader.ReadLine().Trim().Split((' '))[0];
            string[] contactPoint = cutReader.ReadLine().Trim('(').Trim(')').Split(',');
            string[] direction = cutReader.ReadLine().Trim('(').Trim(')').Split(',');
            float[] contactPointFloat = new float[3];
            float[] directionFloat = new float[3];
            for (int j = 0; j < 3; j++)
            {
                contactPointFloat[j] = float.Parse(contactPoint[j]);
            }
            for (int j = 0; j < 3; j++)
            {
                directionFloat[j] = float.Parse(direction[j]);
            }
            Vector3 contactPointVec3 = new Vector3(contactPointFloat[0],contactPointFloat[1],contactPointFloat[2]);
            Vector3 directionPointVec3 = new Vector3(directionFloat[0],directionFloat[1],directionFloat[2]);
            Cutter.Cut(gameObjectDict[nameCut], contactPointVec3, directionPointVec3);
            gameObjectDict.Remove(nameCut);
            gameObjectDict.Add(nameCut+"0", GameObject.Find(nameCut+"0") );
            gameObjectDict.Add(nameCut+"1", GameObject.Find(nameCut+"1") );
            
            //read in next frame number
            if (!cutReader.EndOfStream)
            {
                nextFrameForCutting = int.Parse(cutReader.ReadLine().Trim().Split(' ')[1]);
            }
        }
    }
}


public class PlayModeManager : MonoBehaviour
{
    public GameObject recording;
    public GameObject recordingBB;
    public GameObject recordingDepth;
    public string directory = @"C:\Users\v4rmini\Documents\RecordingsForRender";
    [HideInInspector]
    public string sampleDir;
    private string replayDir;
    public bool playback;
    public bool useMostRecentRecording;
    private string colorMapPath;
    private ColorByNumber colorByNumber;

    private PlaybackState ps;
    
    public GameObject leftRenderModelPrefab;
    private GameObject leftRenderModelInstance;
    public GameObject rightRenderModelPrefab;
    private GameObject rightRenderModelInstance;
    public GameObject steamVRrightHand;
    public GameObject steamVRleftHand;
    private SkinnedMeshRenderer skinnedMeshRendererRightHand;
    private SkinnedMeshRenderer skinnedMeshRendererLeftHand;
    private bool steamHandsInvisible = false;
    
    private Dictionary<string, string> colorToNameDict = new Dictionary<string, string>();
    public bool initialized = false;

    private BbFrameArray bbFrameArray;
    private string bbSavePath;
    
    // Start is called before the first frame update
    void Awake()
    {
        colorByNumber = FindObjectOfType<ColorByNumber>();
        if (!playback)
        {   recordingBB.SetActive(false);
            recording.SetActive(false);
            recordingDepth.SetActive(false);
            //Leave everything as it is in the project settings
            
            //Create Folder Structures
            string now = DateTime.Now.ToString("yyyy-MM-dd-HH_mm_ss");
            Debug.Log("DATE "+now); //in this folder everything is saved
            sampleDir = directory + @"\Sample" + now;
            Directory.CreateDirectory(sampleDir);
            replayDir = sampleDir + @"\ReplayFiles";
            Directory.CreateDirectory(replayDir);
            string recordingsDir = sampleDir + @"\RecordingsFiles";
            Directory.CreateDirectory(recordingsDir);

            Directory.CreateDirectory(replayDir + @"\PositionAndOrientation");
            Directory.CreateDirectory(replayDir + @"\Cuts");
            Directory.CreateDirectory(replayDir + @"\Particles");
            Directory.CreateDirectory(replayDir + @"\RightHand");
            Directory.CreateDirectory(replayDir + @"\LeftHand");


            string videoDir = recordingsDir + @"\Videos";
            Directory.CreateDirectory(videoDir);
            string annotationDir = recordingsDir + @"\Annotations";
            Directory.CreateDirectory(annotationDir);
            
            string[] cameras = new string[2]{"Ego", "Cam1"}; //Add further cameras here
            foreach (string c in cameras)
            {
                Directory.CreateDirectory(videoDir + @"\" + c);
            }

            Directory.CreateDirectory(annotationDir + @"\PoseAndOrientation");
            Directory.CreateDirectory(annotationDir + @"\BoundingBox");
            Directory.CreateDirectory(annotationDir + @"\Colormap");
            Directory.CreateDirectory(annotationDir + @"\Predicates");
        }
        else // if playback
        {
            //Turn off everything for creating bounding boxes and other frame rendering recording
            //recording = GameObject.Find("Recording");
            //recordingBB = GameObject.Find("RecordingBB");
            //recording.SetActive(false);
            //recordingBB.SetActive(false);
            
            UnityEngine.XR.InputTracking.disablePositionalTracking = true;
            
            //Adjust physics settings to prepare for playback mode
            Physics.autoSimulation = false;

            DirectoryInfo sampleFolderDirInfo;
            //get most recent replay
            if (useMostRecentRecording)
            {
                sampleDir = new DirectoryInfo(directory).GetDirectories().OrderByDescending(d=>d.LastWriteTimeUtc).First().ToString();
                replayDir = sampleDir + @"\ReplayFiles";
                colorMapPath = sampleDir + @"\RecordingsFiles\Annotations\Colormap\colormap1.txt";
            }
            
            //This is the hand model without the vr input script and animator
            leftRenderModelInstance = GameObject.Instantiate(leftRenderModelPrefab);
            rightRenderModelInstance = GameObject.Instantiate(rightRenderModelPrefab);
            //leftRenderModelInstance.gameObject.name = "LeftHand";
            //rightRenderModelInstance.gameObject.name = "RightHand";
            Destroy(leftRenderModelInstance.GetComponentInChildren<SteamVR_Behaviour_Skeleton>());
            Destroy(rightRenderModelInstance.GetComponentInChildren<SteamVR_Behaviour_Skeleton>());
            Destroy(rightRenderModelInstance.GetComponentInChildren<Animator>());
            Destroy(leftRenderModelInstance.GetComponentInChildren<Animator>());

            
            recordingBB.SetActive(false);
            recording.SetActive(false);
            recordingDepth.SetActive(false);
            
            string[] excludes = new String[]{"vr_glove_right_slim", "vr_glove_left_slim"};
            ps = new PlaybackState(replayDir, excludes, rightRenderModelInstance, leftRenderModelInstance);

            recordingBB.SetActive(true);
            recording.SetActive(true);
            recordingDepth.SetActive(true);
            
            bbFrameArray = new BbFrameArray();
            bbSavePath = sampleDir + @"\RecordingsFiles\Annotations\BoundingBox\bounding_box_1.json";
            initialized = true;
        }
    }

    private void Start()
    {
        if (playback)
        {    
            //set all object ids in replay mode to the recorded ones
            StreamReader colorMapReader = new StreamReader(colorMapPath);
            while (!colorMapReader.EndOfStream)
            {
                string objName = colorMapReader.ReadLine();
                char[] arr = {'R', 'G', 'B', 'A', '('};
                string color = colorMapReader.ReadLine().Trim(arr).Trim(')');
                colorToNameDict.Add(color, objName);
                string[] colors = color.Split(',');
                float[] colorsf = new float[4];
                for (int i = 0; i < 4; i++)
                {
                    colorsf[i] = float.Parse(colors[i]);
                }

                int intid = int.Parse(colorMapReader.ReadLine().Trim());
                //Debug.Log("OBJNAME " + objName);
                GameObject g = GameObject.Find(objName);
                ObjectId oid = g.GetComponent<ObjectId>();
                oid.c = new Color(colorsf[0], colorsf[1], colorsf[2], colorsf[3]);
                oid.objectName = objName;
                oid.id = intid;
            }
            
            CopyComponent<ObjectId>(steamVRleftHand.GetComponent<ObjectId>(), leftRenderModelInstance);
            CopyComponent<ObjectId>(steamVRrightHand.GetComponent<ObjectId>(), rightRenderModelInstance);

            leftRenderModelInstance.tag = "LeftHandReplay";
            rightRenderModelInstance.tag = "RightHandReplay";

            initialized = true;
        }
    }

    private void MakeOrigHandsInvisible()
    {
        if (playback && !ps.playbackFinished)
        {
            if (steamHandsInvisible == false)
            {
                if (skinnedMeshRendererRightHand == null)
                {
                    skinnedMeshRendererRightHand = steamVRrightHand.GetComponentInChildren<SkinnedMeshRenderer>();
                    if (skinnedMeshRendererRightHand != null)
                    {
                        skinnedMeshRendererRightHand.enabled = false;
                    }
                }

                if (skinnedMeshRendererLeftHand == null)
                {
                    skinnedMeshRendererLeftHand = steamVRleftHand.GetComponentInChildren<SkinnedMeshRenderer>();
                    if (skinnedMeshRendererLeftHand != null)
                    {
                        skinnedMeshRendererLeftHand.enabled = false;
                    }
                }

                if (skinnedMeshRendererRightHand != null && skinnedMeshRendererLeftHand != null &&
                    skinnedMeshRendererRightHand.enabled == false && skinnedMeshRendererLeftHand.enabled == false)
                {
                    steamHandsInvisible = true;
                }
            }
        }
    }
    
    //https://answers.unity.com/questions/458207/copy-a-component-at-runtime.html
    T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }

    // Update is called once per frame
    void Update()
    {
        if (playback)
        {
            MakeOrigHandsInvisible();
            ps.GetStateOfFrame();

            //Write Video Files
            colorByNumber.StoreAllAs(sampleDir + @"\RecordingsFiles\Videos\Cam1\", ps.frameNumber);

            //save bounding boxes for each frame
            BbFrame bbf = new BbFrame();
            bbFrameArray.bb_frame_arr.Add(bbf);
            for (int i = 0; i < colorByNumber.objectIds.Length; i++)
            {
                if (colorByNumber.objectIds[i].xMax != int.MinValue &&
                    colorByNumber.objectIds[i].xMin != int.MaxValue &&
                    colorByNumber.objectIds[i].yMax != int.MinValue && 
                    colorByNumber.objectIds[i].yMin != int.MaxValue)
                {
                    BbObject bbo = new BbObject();
                    bbo.name = colorByNumber.objectIds[i].objectName;
                    bbo.id_no = colorByNumber.objectIds[i].id;
                    bbo.x_max = colorByNumber.objectIds[i].xMax;
                    bbo.x_min = colorByNumber.objectIds[i].xMin;
                    bbo.y_max = colorByNumber.objectIds[i].yMax;
                    bbo.y_min = colorByNumber.objectIds[i].yMin;
                    bbf.bb_obect_arr.Add(bbo);
                }
            }

            if (ps.frameNumber % 200 == 0)
            {

                RecordObjectPosRot.SaveIntoJson(bbSavePath, bbFrameArray);
                bbSavePath = sampleDir + @"\RecordingsFiles\Annotations\BoundingBox\bounding_box_" +
                             ps.frameNumber.ToString() + ".json";
            }
        }
    }
    
    void OnDestroy()
    {
        if (playback)
        {
            RecordObjectPosRot.SaveIntoJson(bbSavePath, bbFrameArray);
        }
    }
}

