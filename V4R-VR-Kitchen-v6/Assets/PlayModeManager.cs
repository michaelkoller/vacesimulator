using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;


public class PlaybackState
{   
    public string path;
    public string pathCut = @"C:\Users\v4rmini\Documents\github\v4r-vr-kitchen-git\V4R-VR-Kitchen-v6\Assets\RecordingsForRender\testcut1.txt"; //TODO automate this
    public int frameNumber;
    public int nextFrameForCutting = -1;
    public float time;
    public float deltaTime;
    private System.IO.StreamReader reader;
    private StreamReader cutReader;
    
    private Renderer[] allRenderers;
    private GameObject[] allGameObjectsWithRenderer;
    private Dictionary<string, GameObject> gameObjectDict = new Dictionary<string, GameObject>();
    
    public PlaybackState(string path)
    {   
        allRenderers = GameObject.FindObjectsOfType<Renderer>();
        allGameObjectsWithRenderer = new GameObject[allRenderers.Length];
        for (int i = 0; i < allRenderers.Length; i++)
        {
            allGameObjectsWithRenderer[i] = allRenderers[i].gameObject;
            gameObjectDict.Add(allRenderers[i].gameObject.name, allRenderers[i].gameObject);
        }        
        this.path = path;
        this.reader = new StreamReader(this.path);
        this.cutReader = new StreamReader(this.pathCut);
        //get frame number of first cut
        nextFrameForCutting = int.Parse(cutReader.ReadLine().Trim().Split(' ')[1]);

    }

    public void GetStateOfFrame()
    {   
        Debug.Log("Frame "+frameNumber);
        string frameNumberString = reader.ReadLine().Trim().Split(' ')[1];
        this.frameNumber = int.Parse(frameNumberString);
        this.time = float.Parse(reader.ReadLine().Trim());
        this.deltaTime = float.Parse(reader.ReadLine().Trim());
        for (int i = 0; i < allGameObjectsWithRenderer.Length; i++)
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
            Debug.Log("NC " + nameCut);
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
    private string directory = @"C:\Users\v4rmini\Documents\github\v4r-vr-kitchen-git\V4R-VR-Kitchen-v6\Assets\RecordingsForRender";
    private DirectoryInfo di;
    private FileInfo[] fileInfos;
    public bool playback;

    private PlaybackState ps;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!playback)
        {
            //Leave everything as it is in the project settings
        }
        else
        {    
            //Turn off everything for creating bounding boxes and other frame rendering recording
            //recording = GameObject.Find("Recording");
            //recordingBB = GameObject.Find("RecordingBB");
            //recording.SetActive(false);
            //recordingBB.SetActive(false);
            
            //Adjust physics settings to prepare for playback mode
            Physics.autoSimulation = false;
            di = new DirectoryInfo(directory);
            fileInfos = di.GetFiles();
            ps = new PlaybackState(fileInfos[0].FullName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playback)
        {
            ps.GetStateOfFrame();
        }
    }

    void UpdateDictWhenCutting(string oldGameObjectName, GameObject leftGO, GameObject rightGO)
    {
        
    }
}
