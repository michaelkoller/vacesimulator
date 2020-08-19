using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;


public class PlaybackState
{   
  
    public string replayDir;
    public string pathPO;
    public string pathCut;
    public int frameNumber;
    public int nextFrameForCutting = -1;
    public float time;
    public float deltaTime;
    private DirectoryInfo di;
    private FileInfo[] fileInfosPO;
    private FileInfo[] fileInfosCuts;
    private StreamReader reader;
    private int currPathPOfileNumber;
    private int poFileCount;
    private StreamReader cutReader;
    public bool playbackFinished;
    
    private Renderer[] allRenderers;
    private GameObject[] allGameObjectsWithRenderer;
    private Dictionary<string, GameObject> gameObjectDict = new Dictionary<string, GameObject>();

    public PlaybackState(string replayDir)
    {   
        allRenderers = GameObject.FindObjectsOfType<Renderer>();
        allGameObjectsWithRenderer = new GameObject[allRenderers.Length];
        for (int i = 0; i < allRenderers.Length; i++)
        {
            allGameObjectsWithRenderer[i] = allRenderers[i].gameObject;
            gameObjectDict.Add(allRenderers[i].gameObject.name, allRenderers[i].gameObject);
        }        
        this.replayDir =  replayDir;
        this.pathPO = replayDir + @"\PositionAndOrientation";
        this.pathCut = replayDir + @"\Cuts";
        
        di = new DirectoryInfo(pathPO);
        fileInfosPO = di.GetFiles().OrderBy(p => p.CreationTime).ToArray();;
        currPathPOfileNumber = 0;
        poFileCount = fileInfosPO.Length;
        di = new DirectoryInfo(pathCut);
        fileInfosCuts = di.GetFiles();
        
        GetNextPOFile();
        //this.reader = new StreamReader(fileInfosPO[currPathPOfileNumber].FullName); // TODO iterate over all the files in dir
        this.cutReader = new StreamReader(fileInfosCuts[0].FullName);
        //get frame number of first cut
        if (!cutReader.EndOfStream)
        {
            nextFrameForCutting = int.Parse(cutReader.ReadLine().Trim().Split(' ')[1]);
        }
        else
        {
            nextFrameForCutting = -1;
        }
        
        playbackFinished = false;
    }

    private void GetNextPOFile()
    {
        this.reader = new StreamReader(fileInfosPO[currPathPOfileNumber].FullName); // TODO iterate over all the files in dir
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
        string frameNumberString = reader.ReadLine().Trim().Split(' ')[1];
        this.frameNumber = int.Parse(frameNumberString);
        this.time = float.Parse(reader.ReadLine().Trim());
        this.deltaTime = float.Parse(reader.ReadLine().Trim());
        for (int i = 0; i < gameObjectDict.Count; i++)
        {
            string name = reader.ReadLine().Trim().Split((' '))[0];
            string[] pos = reader.ReadLine().Trim('(').Trim(')').Split(',');
            string[] rot = reader.ReadLine().Trim('(').Trim(')').Split(',');
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
    private GameObject recording;
    private GameObject recordingBB;
    public string directory = @"C:\Users\v4rmini\Documents\RecordingsForRender";
    [HideInInspector]
    public string sampleDir;
    private string replayDir;
    public bool playback;
    public bool useMostRecentRecording;

    private PlaybackState ps;
    
    public GameObject renderModelPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        if (!playback)
        {
            
            //TODO instantiate this hand, find out how to unpair it from vr input, find out how to move hand and fingers
            GameObject renderModelInstance = GameObject.Instantiate(renderModelPrefab);
            
            
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
            Directory.CreateDirectory(replayDir + @"\Faucet");
            Directory.CreateDirectory(replayDir + @"\Stove");

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
                replayDir = new DirectoryInfo(directory).GetDirectories().OrderByDescending(d=>d.LastWriteTimeUtc).First().ToString();
                replayDir += @"\ReplayFiles";
            }
            ps = new PlaybackState(replayDir);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playback && !ps.playbackFinished)
        {    
            ps.GetStateOfFrame();
        }
    }

    void UpdateDictWhenCutting(string oldGameObjectName, GameObject leftGO, GameObject rightGO)
    {
        
    }
}
