using System;
using UnityEngine;
using System.Collections;
using System.Security.Cryptography.X509Certificates;


public class storeScreenshot : MonoBehaviour {
    private int resWidth = 1024; 
    private int resHeight = 1024;
    private bool storeContinuously = true;
   
    private int image_index = 0;
 
    private bool takeHiResShot = false;

    public Shader idShader;

    //public MovementSpace space;
    public float speed = 0.1f;

    public string screenshotPath = "/Users/jeremy/Desktop/OpticalFlow/";
    
    public void TakeHiResShot() {
        takeHiResShot = true;
    }

    //public RenderTexture rt3;

    private RenderTexture idsLastFrame;
    private RenderTexture localGeomLastFrame;
    private RenderTexture depthLastFrame;
    private RenderTexture colorLastFrame;
    private Matrix4x4 camPosLastFrame;
    private Matrix4x4 camProjLastFrame;
    //maybe also projection matrix
    private Matrix4x4[] posesLastFrame;
    private ComputeBuffer poseBufferLastFrame;
    private bool secondFrame = false;
    
    
    void LateUpdate() {
        /*
        takeHiResShot = Input.GetKeyDown("k") | storeContinuously;
        if (takeHiResShot || secondFrame) {
           
            RenderTexture color = 
                TakeColorScreenshot();
            Matrix4x4[] objPoses = SetupIDMaterialsMatrices();
            RenderTexture ids =
                TakeIDScreenshot();
            UnsetupIDMaterials();
            takeHiResShot = false;

            // crazy debug effort
            //objPoses[0] = transform.worldToLocalMatrix;
            
            //http://kylehalladay.com/blog/tutorial/2014/06/27/Compute-Shaders-Are-Nifty.html
            ComputeBuffer posBuffer = new ComputeBuffer(objPoses.Length,
                4*4*4);
            posBuffer.SetData(objPoses);
            
            //print(posBuffer.IsValid());
            //print(posBuffer.count);
            Camera camera = GetComponent<Camera>();
            Matrix4x4 camPos = camera.worldToCameraMatrix;
            Matrix4x4 camProj = camera.projectionMatrix;
            //print(camera.projectionMatrix);
            
            if (secondFrame)
            {
                secondFrame = false;
                RenderTexture mask12 = new RenderTexture(resWidth,resHeight,24,RenderTextureFormat.ARGBFloat);
                mask12.enableRandomWrite = true;
                mask12.Create();
                RenderTexture mask21 = new RenderTexture(resWidth,resHeight,24,RenderTextureFormat.ARGBFloat);
                mask21.enableRandomWrite = true;
                mask21.Create();
                
                RenderTexture rt = new RenderTexture(resWidth,resHeight,24,RenderTextureFormat.ARGBFloat);
                rt.enableRandomWrite = true;
                rt.Create();
            
                Texture2D screenShot = new Texture2D(resWidth, resWidth, TextureFormat.RGBAFloat, false);

                Matrix4x4 debug = Matrix4x4.zero;
                debug[0,0] = 0;
                debug[0,1] = 0;
                debug[0,2] = 1;
                debug[0,3] = 1;

                RenderTexture.active = rt;
            
                screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
                byte[] bytes = screenShot.EncodeToPNG();
                string filename = screenshotPath + "debug.png";
                Color[] colors = screenShot.GetPixels();

                //System.IO.File.WriteAllBytes(filename, bytes);
                //Debug.Log(string.Format("Took screenshot to: {0}", filename));
                
                
                RenderTexture.active = flow12;
                screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
                colors = screenShot.GetPixels();

                bytes = screenShot.EncodeToPNG();
                filename = screenshotPath + "flow12.png";
                //System.IO.File.WriteAllBytes(filename, bytes);
                //Debug.Log(string.Format("Took screenshot to: {0}", filename));

                RenderTexture.active = flow21;
            
                screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
                bytes = screenShot.EncodeToPNG();
                filename = screenshotPath + "flow21.png";
                //System.IO.File.WriteAllBytes(filename, bytes);
                //Debug.Log(string.Format("Took screenshot to: {0}", filename));

                /////StoreAs(colorLastFrame,"color1.png",false);
                /////StoreAs(color,"color2.png",false);
                image_index++;

                //TODO: randomize object positions
            }
            else
            {
                secondFrame = true;
                
                
                //Destroy(idsLastFrame);
                //Destroy(localGeomLastFrame);
                //Destroy(depthLastFrame);
                //Destroy(poseBufferLastFrame);
                if (poseBufferLastFrame != null)
                {
                    poseBufferLastFrame.Dispose();
                    
                }
                
                idsLastFrame = ids;
                poseBufferLastFrame = posBuffer;
                posesLastFrame = objPoses;
                colorLastFrame = color;

                camPosLastFrame = camPos;
                camProjLastFrame = camProj;
                
            }
        }
        */
    }

    //TODO: actually return texture with the data
    RenderTexture TakeColorScreenshot()
    {
        RenderTexture rt = new RenderTexture(resWidth, resHeight,24, RenderTextureFormat.ARGBFloat);
        Camera camera = GetComponent<Camera>();
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGBAFloat, false);
        camera.Render();
            
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        Color[] colors = screenShot.GetPixels();
        RenderTexture.active = null; // JC: added to avoid errors
        //Destroy(rt);
        return rt;
    }
    
    RenderTexture TakeIDScreenshot()
    {
        RenderTexture rt = new RenderTexture(resWidth, resHeight,24, RenderTextureFormat.ARGBFloat);
        Camera camera = GetComponent<Camera>();
        camera.targetTexture = rt;
        CameraClearFlags formerFlags = camera.clearFlags;
        Color formerColor = camera.backgroundColor;
        camera.clearFlags = CameraClearFlags.SolidColor;// | CameraClearFlags.Depth;
        camera.backgroundColor = new Color(1.0f, 1.0f, -1.0f, 1.0f);
        
        camera.SetReplacementShader(idShader,"RenderType");
        
        //TextureFormat.RGBAFloat;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGBAFloat, false);
        camera.Render();
        
        camera.clearFlags = formerFlags;
        camera.backgroundColor = formerColor;
            
        camera.ResetReplacementShader();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        Color[] colors = screenShot.GetPixels();
        RenderTexture.active = null; // JC: added to avoid errors
        //Destroy(rt);


        return rt;
    }
    
    private MeshRenderer[] renderers;
    private Material[] originalMaterials;
    Matrix4x4[] SetupIDMaterialsMatrices()
    {
        //setup the materials
        renderers = FindObjectsOfType<MeshRenderer>();
        originalMaterials = new Material[renderers.Length];
        Matrix4x4[] matrices = new Matrix4x4[renderers.Length];
        for (int i=0;i<renderers.Length;i++)
        {
            Renderer renderer = renderers[i];
            originalMaterials[i] = renderer.material;
            Material material = new Material(Shader.Find("Unlit/ObjectID"));
            material.SetInt("_Index",i);
            renderer.material = material;
            matrices[i] = renderer.localToWorldMatrix;
        }

        return matrices;

    }
    
    void UnsetupIDMaterials()
    {
        //unsetup the materials
        for (int i=0;i<renderers.Length;i++)
        {
            Renderer renderer = renderers[i];
            renderer.material = originalMaterials[i];
        
        }
    }

    Color[] GetColors(RenderTexture tex)
    {
        RenderTexture old = RenderTexture.active;
        RenderTexture.active = tex;
        Texture2D screenShot = new Texture2D(resWidth, resWidth, TextureFormat.RGBAFloat, false);

        
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        //Destroy(rt);
        Color[] colors = screenShot.GetPixels();
        RenderTexture.active = old;
        return colors;
    }

    Vector3 ToVector(Color c)
    {
        return new Vector3(c.r,c.g,c.b);
    }

    Vector4 Homogonize(Vector3 v)
    {
        return new Vector4(v.x,v.y,v.z,1.0f);
    }

    void StoreAs(RenderTexture rt,string filename,bool exr)
    {
        RenderTexture old = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(resWidth, resWidth, TextureFormat.RGBAFloat, false);
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        StoreAs(screenShot,filename,exr);
        RenderTexture.active = old;
    }

    void StoreAs(Texture2D tex,string filename,bool exr)
    {
        byte[] bytes;
        if (exr)
        {
            bytes = tex.EncodeToEXR(Texture2D.EXRFlags.CompressZIP);
        }
        else
        {
            bytes = tex.EncodeToPNG();
        }
        
        string filepath = screenshotPath + image_index + filename;
        System.IO.File.WriteAllBytes(filepath, bytes);
        //Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }

    void StoreAs(Color[] c, string filename,bool exr)
    {
        Texture2D screenShot= new Texture2D(resWidth, resHeight, TextureFormat.RGBAFloat, false);

        screenShot.SetPixels(c);
        StoreAs(screenShot,filename,exr);
    }
   
}