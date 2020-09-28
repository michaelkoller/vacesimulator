using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.UIElements;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using UnityEditor.Media;
using UnityEngine.iOS;
using Valve.VR.InteractionSystem;

public class ColorByNumber : MonoBehaviour {
    
    //[BurstCompile(CompileSynchronously = true)] //TODO Reenable after debugging
    struct CalcBBs : IJob
    {
        public int width;
        public int height;
        public NativeArray<Color> pixel;

        [NativeDisableParallelForRestriction]
        public NativeArray<MinMax> bbs;
        
        // The code actually running on the job
        public void Execute()
        {   //Debug.Log(pixel[0].ToString("G17"));
            // Move the positions based on delta time and velocity
            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    Color c = pixel[y*width + x];
                    //int ind = (int)Mathf.Round(c.b);// simons variant
                    int ind = Mathf.RoundToInt(c.b * 256f);// simons variant

                    // int ind = (int) (c.b * 100f); //TODO hier und in object id --> richtige Farbzuweisung machen 
                    // ind += (int) (c.g * 10f);
                    // ind += (int) (c.r);

                    if (ind != 0)
                    {
                        MinMax mm = bbs[ind-1];
                        if (mm.xMax < x)
                        {
                            mm.xMax = x;
                        }
                        if (mm.xMin > x)
                        {
                            mm.xMin = x;
                        }
                        if (mm.yMax < y)
                        {
                            mm.yMax = y;
                        }
                        if (mm.yMin > y)
                        {
                            mm.yMin = y;
                        }

                        bbs[ind-1] = mm;
                    }
                }
            }
        }
    }

    public string screenshotPath;
    private MeshRenderer[] renderers;
    private List<Material> originalMaterials; //actually don't need this anymore, because bbs now can handle objs with multiple materials
    private List<Material[]> originalMaterialArrays;
    public Camera cam;
    public Camera rgbCam;
    public Camera rgbCamNoBB;
    public Camera depthCam;
    public ObjectId[] objectIds;
    public float deltaTimeBoundBoxUpdate = 0.1f;
    public float lastTime = 0;
    
    private int camTexHeight;
    private int camTexWidth;
    
    //for bound boxes
    private Rect bbox;
    private Canvas vidCapCanvas;
    private GameObject[] trackObjects;
    //private GameObject[] boundingBoxes;
    private GameObject[] staticTrackObjects;
    private GameObject[] staticBoundingBoxes;
    public GameObject bbDisp;
    private int trackObjectsAmount;
    private int staticTrackObjectsAmount;
    private float dt = 0f;
    public int fixedUpdateCounter = 0;
    
    //BBCube
    public Camera bbCubeCam;
    private GameObject[] boundingBoxCubes;

    //Hands
    public List<Material> rightHandMaterials;
    public List<Material> leftHandMaterials;

    public SkinnedMeshRenderer eyebrow;
    public SkinnedMeshRenderer eyelashes;
    public SkinnedMeshRenderer suit;
    public SkinnedMeshRenderer eyes;
    public SkinnedMeshRenderer skin;
    public SkinnedMeshRenderer hair;
    public SkinnedMeshRenderer shoes;
    private Material eyebrowMaterial;
    private Material eyelashesMaterial;
    private Material suitMaterial;
    private Material eyesMaterial;
    private Material skinMaterial;
    private Material hairMaterial;
    private Material shoesMaterial;

    public Hand leftHand;
    public Hand rightHand;
    public SkinnedMeshRenderer smrLeftHand;
    public SkinnedMeshRenderer smrRightHand;
    public Material leftHandMat;
    public Material rightHandMat;
    private bool handSetupDone = false;
    private bool arrayOrdered = false;

    private PlayModeManager playModeManager;
    
    private Texture2D screenShot;
    private void Start()
    {
        //cam = GetComponent<Camera>();
        playModeManager = GameObject.FindWithTag("Manager").GetComponent<PlayModeManager>();
        screenshotPath = playModeManager.sampleDir +
                         @"\RecordingsFiles\Videos\Cam1\";
        objectIds = FindObjectsOfType<ObjectId>();
        objectIds = objectIds.Where(objId => objId.gameObject.tag != "LeftHand").ToArray(); //this excludes the non-playback hands. for playback there is another set of hands instantiated.
        objectIds = objectIds.Where(objId => objId.gameObject.tag != "RightHand").ToArray();
        Array.Sort(objectIds,delegate(ObjectId x, ObjectId y) { return x.id.CompareTo(y.id); });
        
        //for bound boxes
        //vidCapCanvas = GameObject.FindGameObjectWithTag("VideoCaptureCanvas").GetComponent<Canvas>();
        //boundingBoxes = new GameObject[objectIds.Length];
        boundingBoxCubes = new GameObject[objectIds.Length];
        for (int i = 0; i< objectIds.Length; i++)
        {
            //boundingBoxes[i] = (GameObject) Instantiate(Resources.Load("BBPrefab"));
            //boundingBoxes[i].transform.SetParent(vidCapCanvas.transform, false);
            boundingBoxCubes[i]= (GameObject) Instantiate(Resources.Load("BBCubePrefab"));
            boundingBoxCubes[i].transform.SetParent(bbCubeCam.transform, false);
            boundingBoxCubes[i].GetComponent<MeshRenderer>().material.color = new Color(objectIds[i].c.r, objectIds[i].c.g, objectIds[i].c.b, 0.4f);
            boundingBoxCubes[i].name = "BBCubePrefab_" + objectIds[i].objectName+ "_id"+objectIds[i].id;
        }

        int width = cam.targetTexture.width;
        int height = cam.targetTexture.height;
        screenShot = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
        shaderPropertyIndex = Shader.PropertyToID("_Index");
        //print(shaderPropertyIndex); 
        camTexHeight = cam.targetTexture.height;
        camTexWidth = cam.targetTexture.width;
        
        materials = new Material[1000];
        for (int i = 0; i < 1000; i++)
        {
            materials[i] = new Material(shader);
            materials[i].SetInt(shaderPropertyIndex, i);
        }

        eyebrowMaterial = new Material(eyebrow.material);
        eyelashesMaterial = new Material(eyelashes.material);
        suitMaterial = new Material(suit.material);
        eyesMaterial = new Material(eyes.material);
        skinMaterial = new Material(skin.material);
        hairMaterial = new Material(hair.material);
        shoesMaterial = new Material(shoes.material);
    }

    public void StoreAllAs(string path, int frameNo)
    {   
        StoreAs(cam.targetTexture, "segmentation-" + frameNo +  ".jpg", false);
        StoreAs(rgbCamNoBB.targetTexture, "rgb-" + frameNo +  ".jpg", false);
        StoreAs(depthCam.targetTexture, "depth-" + frameNo +  ".jpg", false);
    }
    
    private void Update()
    {   
        for (int i = 0; i < objectIds.Length; i++)
        {
            if(objectIds[i].xMax != int.MinValue && objectIds[i].xMin != int.MaxValue && objectIds[i].yMax != int.MinValue && objectIds[i].yMin != int.MaxValue){
                boundingBoxCubes[i].transform.localPosition = new Vector3(((objectIds[i].xMax + objectIds[i].xMin)*0.5f /camTexHeight) - 0.66f, ((objectIds[i].yMax + objectIds[i].yMin)*0.5f/camTexHeight)  - 0.5f,1.0f);
                boundingBoxCubes[i].transform.localScale = new Vector3((objectIds[i].xMax - objectIds[i].xMin)/(float)camTexHeight, (objectIds[i].yMax - objectIds[i].yMin)/(float)camTexHeight,0.001f);
                boundingBoxCubes[i].SetActive(true);
            }
            else
            {
                boundingBoxCubes[i].SetActive(false);
            }
        }
    }

    bool SetHandRefs()
    {
        GameObject leftHandGo = GameObject.FindGameObjectWithTag("LeftHandReplay");
        smrLeftHand = leftHandGo.transform.GetComponentInChildren<SkinnedMeshRenderer>();
        if (smrLeftHand == null)
        {
            return false;
        }
        leftHandMat = new Material(smrLeftHand.material); 
        //Hand leftHand = leftHandGo.transform.GetComponent<Hand>();
        //leftHand.mainRenderModel.SetControllerVisibility(false, permanent:true);
        
        GameObject rightHandGo = GameObject.FindGameObjectWithTag("RightHandReplay");
        smrRightHand = rightHandGo.transform.GetComponentInChildren<SkinnedMeshRenderer>();
        if (smrRightHand == null)
        {
            return false;
        }
        rightHandMat = new Material(smrRightHand.material); 
        //Hand rightHand = rightHandGo.transform.GetComponent<Hand>();
        //rightHand.mainRenderModel.SetControllerVisibility(false, permanent:true);
        return true;
    }

    public Shader shader;
    private int shaderPropertyIndex;
    private Material[] materials;
    void SetupIDMaterials()
    {
        originalMaterials = new List<Material>();
        originalMaterialArrays = new List<Material[]>();
        leftHandMaterials = new List<Material>();
        rightHandMaterials = new List<Material>();
        //setup the materials
        renderers = FindObjectsOfType<MeshRenderer>();
        for (int i=0;i<renderers.Length;i++)
        {
            Renderer renderer = renderers[i];
            
            ObjectId id = renderer.gameObject.GetComponentInParent<ObjectId>();
            originalMaterials.Add(renderer.material);
            originalMaterialArrays.Add(renderer.materials);

            if (!handSetupDone)
            {
                handSetupDone = SetHandRefs();
            }
            
            if (id != null)
            {
                if (id.objectName == "LeftHand" )
                {
                    if(handSetupDone) smrLeftHand.material = materials[id.id];
                }else if(id.objectName == "RightHand")
                {
                    if(handSetupDone) smrRightHand.material = materials[id.id];
                }else if (id.objectName == "human")
                {
                    eyebrow.material = materials[id.id];
                    eyelashes.material = materials[id.id];
                    suit.material = materials[id.id];
                    eyes.material = materials[id.id];
                    skin.material = materials[id.id];
                    hair.material = materials[id.id];
                    shoes.material = materials[id.id];
                }
                else
                {
                    //renderer.material = materials[id.id];   //CHANGE THIS BACK IF PROBLEM WITH RENDERER OCCURS
                    Material[] newMats = new Material[renderer.materials.Length];
                    for (int j = 0; j < renderer.materials.Length; j++)
                    {
                        newMats[j] = materials[id.id];
                    }
                    renderer.materials = newMats;

                }
            }
            else
            {
                renderer.material = materials[0];
            }
        }
    }
    
    void UnsetupIDMaterials()
    {   
        //unsetup the materials
        for (int i=0;i<renderers.Length;i++)
        {
            Renderer renderer = renderers[i];
            //renderer.material = originalMaterials[i]; //CHANGE THIS BACK IF PROBLEM WITH RENDERER OCCURS
            renderer.materials = originalMaterialArrays[i];
        }
        eyebrow.material = eyebrowMaterial;
        eyelashes.material = eyelashesMaterial;
        suit.material = suitMaterial;
        eyes.material = eyesMaterial;
        skin.material = skinMaterial;
        hair.material = hairMaterial;
        shoes.material = shoesMaterial;
        
        if(handSetupDone) smrLeftHand.material = leftHandMat;
        if(handSetupDone) smrRightHand.material = rightHandMat;
    }

    Color[] GetColors(RenderTexture tex)
    {
        int width = tex.width;
        int height = tex.height;
        RenderTexture old = RenderTexture.active;
        RenderTexture.active = tex;
        
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        //Destroy(rt);
        Color[] colors = screenShot.GetPixels();
        RenderTexture.active = old;
        return colors;
    }

    void StoreAs(RenderTexture rt,string filename,bool exr)
    {    
        int width = rt.width;
        int height = rt.height;
        RenderTexture old = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        StoreAs(screenShot,filename,exr);
        RenderTexture.active = old;
        Destroy(screenShot);
        screenShot = null;
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
            //bytes = tex.EncodeToPNG();
            bytes = tex.EncodeToJPG();
        }
        
        string filepath = screenshotPath + filename;
        System.IO.File.WriteAllBytes(filepath, bytes);
        //Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }
    
    private void OnPreRender()
    {
        SetupIDMaterials();
    }
    
    private struct MinMax
    {
        public int xMin;
        public int xMax;
        public int yMin;
        public int yMax;
    }
    ;
    private void OnPostRender()
    {
        Color[] colors = GetColors(cam.targetTexture);
      
        var output = new NativeArray<MinMax>(objectIds.Length, Allocator.TempJob);
        var input = new NativeArray<Color>(colors, Allocator.TempJob);
        for (int i = 0; i < output.Length; i++)
        {
            var mm = new MinMax();
            
            mm.xMax = int.MinValue;
            mm.xMin = int.MaxValue;
            mm.yMax = int.MinValue;
            mm.yMin = int.MaxValue;
            output[i] = mm;
        }

        var job = new CalcBBs
        {
            width = camTexWidth,
            height = camTexHeight,
            bbs = output,
            pixel = input,
        };
        
        job.Schedule().Complete();
        
        for (int i = 0; i < output.Length; i++)
        {
            objectIds[i].xMax = output[i].xMax;
            objectIds[i].xMin = output[i].xMin;
            objectIds[i].yMax = output[i].yMax;
            objectIds[i].yMin = output[i].yMin;
        }
        
        output.Dispose();
        input.Dispose();
        
        UnsetupIDMaterials();
    }
}