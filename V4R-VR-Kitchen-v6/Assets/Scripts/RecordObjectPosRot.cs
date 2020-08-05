using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;


//https://support.unity3d.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file-

public class RecordObjectPosRot : MonoBehaviour
{
    int currentFrame = 0;
    private string path;
    private GameObject[] allGameObjects;
    private Renderer[] allRenderers;
    private GameObject[] allGameObjectsWithRenderer;
    private StringBuilder sb;

    string GetPath()
    {
        return "Assets/RecordingsForRender/test"+ currentFrame.ToString() +".txt";
    }
    
    // Start is called before the first frame update
    void Start()
    { 
        allGameObjects = GameObject.FindObjectsOfType<GameObject>();
        allRenderers = FindObjectsOfType<Renderer>();
        allGameObjectsWithRenderer = new GameObject[allRenderers.Length];
        for (int i = 0; i < allRenderers.Length; i++)
        {
            allGameObjectsWithRenderer[i] = allRenderers[i].gameObject;
        }
        path = GetPath();
        //Debug.Log(allGameObjects.Length);
        sb = new StringBuilder();
        Debug.Log("ALL RENDERERS LENGTH " + allGameObjectsWithRenderer.Length.ToString());
    }



    // Update is called once per frame
    void Update()
    {
        sb.AppendLine("frame " + currentFrame.ToString());
        sb.AppendLine(Time.time.ToString());
        sb.AppendLine(Time.deltaTime.ToString());
        for (int i = 0; i < allGameObjectsWithRenderer.Length; i++)
        {
            sb.AppendLine(allGameObjectsWithRenderer[i].name);
            sb.AppendLine(allGameObjectsWithRenderer[i].transform.position.ToString("F3"));
            sb.AppendLine(allGameObjectsWithRenderer[i].transform.rotation.ToString("F3"));
        }

        if (currentFrame % 100 == 0)
        {
            File.WriteAllText(path, sb.ToString());
            sb.Clear();
        }
        currentFrame++;
        path = GetPath();
    }
}
