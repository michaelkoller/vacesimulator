using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderDepth : MonoBehaviour
{

    static Camera cam;

    public Shader depthShader;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.SetReplacementShader(depthShader,"RenderType");    
    }

    // Update is called once per frame
    void Update()
    {
    }
    
}
