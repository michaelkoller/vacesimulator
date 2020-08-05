using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



public class PlaybackState
{   
    public string path;
    public int frameNumber;
    public float time;
    public float deltaTime;
    private System.IO.StreamReader reader;
    private int frameCounter;
    
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
        this.frameCounter = 0;
        
    }

    public void GetStateOfFrame()
    {
        this.frameNumber = int.Parse(reader.ReadLine().Trim().Split(' ')[1]);
        this.time = int.Parse(reader.ReadLine().Trim());
        this.deltaTime = int.Parse(reader.ReadLine().Trim());
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
            this.gameObjectDict[name].transform.rotation = new Quaternion(rotFloat[0],rotFloat[1],rotFloat[2], rotFloat[3]);
            
        }
    }
}

public class PlayModeManager : MonoBehaviour
{
    private string directory = "Assets/RecordingsForRender/";
    private DirectoryInfo di;
    private FileInfo[] fileInfos;
    public bool playback = false;

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
            //Adjust physics settings to prepare for playback mode
            Physics.autoSimulation = false;
            di = new DirectoryInfo(directory);
            fileInfos = di.GetFiles();
            Debug.Log("FITEST");
            foreach (var fileInfo in fileInfos)
            {
                Debug.Log(fileInfo);
            }
            Debug.Log("FILE NAME "+fileInfos[0].FullName);
            ps = new PlaybackState(fileInfos[0].FullName);
            Debug.Log(ps);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ps.GetStateOfFrame();
    }
}
