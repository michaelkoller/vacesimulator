using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderDepth : MonoBehaviour
{

    static Camera cam;

    public Shader depthShader;
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.SetReplacementShader(depthShader,"RenderType");    
    }
}
