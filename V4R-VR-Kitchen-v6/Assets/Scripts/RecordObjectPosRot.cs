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
    private GameObject[] allGameObjects;
    private Renderer[] allRenderers;
    private GameObject[] allGameObjectsWithRenderer;
    private StringBuilder sb;
    private PlayModeManager playModeManager;
    private bool playback;

    string GetPath()
    {
        return "Assets/RecordingsForRender/test"+ currentFrame.ToString() +".txt";
    }
    
    // Start is called before the first frame update
    void Start()
    {
        playModeManager = GetComponent<PlayModeManager>();
        playback = playModeManager.playback;
        if (!playback)
        {
            allGameObjects = GameObject.FindObjectsOfType<GameObject>();
            allRenderers = FindObjectsOfType<Renderer>();
            allGameObjectsWithRenderer = new GameObject[allRenderers.Length];
            for (int i = 0; i < allRenderers.Length; i++)
            {
                allGameObjectsWithRenderer[i] = allRenderers[i].gameObject;
            }
            path = GetPath();
            sb = new StringBuilder();
        }
    }



    // Update is called once per frame
    void Update()
    {   if(!playback){
            sb.AppendLine("frame " + currentFrame.ToString());
            sb.AppendLine(Time.time.ToString());
            sb.AppendLine(Time.deltaTime.ToString());
            for (int i = 0; i < allGameObjectsWithRenderer.Length; i++)
            {
                sb.AppendLine(allGameObjectsWithRenderer[i].name.ToString());
                sb.AppendLine(allGameObjectsWithRenderer[i].transform.position.ToString("F3"));
                sb.AppendLine(allGameObjectsWithRenderer[i].transform.eulerAngles.ToString("F3"));
            }

            if (currentFrame % 200 == 0)
            {
                File.WriteAllText(path, sb.ToString());
                sb.Clear();
            }
            currentFrame++;
            path = GetPath();
        }
    }
}
