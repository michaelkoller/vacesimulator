using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthCamScript : MonoBehaviour
{
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        // cam.SetReplacementShader(shader, null);
        // Shader.SetGlobalTexture("_DepthTex", depth);
        cam.depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
