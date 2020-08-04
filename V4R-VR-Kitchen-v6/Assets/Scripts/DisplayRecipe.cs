using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net.Configuration;
using UnityEngine.UI;
using Valve.VR;
using System.Text;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SerializedTransform
{
    public float[] _position = new float[3];
    public float[] _rotation = new float[4];
    public float[] _scale = new float[3];
    public string _name;
    public bool _active;
    public int _frame;
 
 
//    public SerializedTransform(Transform transform, bool worldSpace = false)
    public SerializedTransform(Transform transform, string name, bool active, int frame)
    {
        _position[0] = transform.localPosition.x;
        _position[1] = transform.localPosition.y;
        _position[2] = transform.localPosition.z;
 
        _rotation[0] = transform.localRotation.w;
        _rotation[1] = transform.localRotation.x;
        _rotation[2] = transform.localRotation.y;
        _rotation[3] = transform.localRotation.z;
 
        _scale[0] = transform.localScale.x;
        _scale[1] = transform.localScale.y;
        _scale[2] = transform.localScale.z;

        _name = name;
        _active = active;
        _frame = frame;

    }
}

public class DisplayRecipe : MonoBehaviour
{        
    public SteamVR_Behaviour_Pose myTrackedObject;
    public SteamVR_Input_Sources myInputSource;
    public SteamVR_Action_Boolean clickRightAction;
    public SteamVR_Action_Boolean clickLeftAction;
    public SteamVR_Action_Boolean clickUpAction;
    public SteamVR_Action_Boolean clickDownAction;
    public SteamVR_Action_Boolean clickRecordAction;
    public SteamVR_Action_Boolean clickShowInstructions;
    private StreamReader reader;
    private int step;
    private string[] lineDescription;
    private int currentFileId;
    Text textField;
    public List<List<string>> stepsList;
    private int currentEntryNumber;
    private Dictionary<string, string> dishDict;
    private bool showInstructions;
    public TMPro.TextMeshProUGUI vrTextField;
    public GameObject vrCanvas;
    private bool currentlyRecording = false;
    public ColorByNumber colByNum;
    private StreamWriter outputFile;
    private StreamWriter recordingFile;
    private int frameCounter = 0;
    public Image recordSign;
    public GameObject[] gos;
    public Transform[] transforms;
    private FileStream fs;
    private BinaryFormatter bf;
    private StringBuilder[] sbs;
    private string[] stringsasdf;
    
    #if UNITY_EDITOR
    [MenuItem("Tools/Read file")]
    #endif


    // Start is called before the first frame update
    void Start()
    {   clickRightAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ClickRight");
        textField = GetComponent<Text>();
        stepsList = new List<List<string>>();
        dishDict = new Dictionary<string, string>();
        //READ WHOLE DATASET FROM HERE
        InitReadString(textField);
        showInstructions = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (clickRightAction.GetStateDown(myInputSource))
        {
            currentEntryNumber = Math.Min(currentEntryNumber + 1, stepsList.Count - 1);
            textField.text = string.Join(" ", stepsList[currentEntryNumber]);
            vrTextField.text = textField.text;
            currentFileId = Int32.Parse(stepsList[currentEntryNumber][2]);
        }
        if (clickLeftAction.GetStateDown(myInputSource))
        {
            currentEntryNumber = Math.Max(currentEntryNumber-1, 0);
            textField.text = string.Join(" ", stepsList[currentEntryNumber]);
            vrTextField.text = textField.text;
            currentFileId = Int32.Parse(stepsList[currentEntryNumber][2]);
        }
        if (clickUpAction.GetStateDown(myInputSource))
        {
            while (currentFileId == Int32.Parse(stepsList[currentEntryNumber][2]) && currentEntryNumber > 0 &&
                   currentEntryNumber < stepsList.Count - 1)
            {   
                currentEntryNumber++;
            }
            currentFileId = int.Parse(stepsList[currentEntryNumber][2]);
            textField.text = string.Join(" ", stepsList[currentEntryNumber]);    
            vrTextField.text = textField.text;
        }
        if (clickDownAction.GetStateDown(myInputSource))
        {
            while (currentFileId == Int32.Parse(stepsList[currentEntryNumber][2]) && currentEntryNumber > 0 &&
                   currentEntryNumber < stepsList.Count - 1)
            {
                currentEntryNumber--;
            }
            currentFileId = int.Parse(stepsList[currentEntryNumber][2]);
            textField.text = string.Join(" ", stepsList[currentEntryNumber]);    
            vrTextField.text = textField.text;
        }
        
        if (clickRecordAction.GetStateDown(myInputSource))
        {
            if (!currentlyRecording)
            {
                StartRecording(currentEntryNumber);
            }
            else
            {
                StopRecording();
            }

        }
        if (clickShowInstructions.GetStateDown(myInputSource))
        {
            showInstructions = !showInstructions;
            vrCanvas.SetActive(showInstructions);
        }
    }

    private void FixedUpdate()
    {
        if(colByNum.fixedUpdateCounter % 3 == 0 && currentlyRecording)
        {   Debug.Log(Time.time);
            for (int i = 0; i < colByNum.objectIds.Length; i++)
            {    
                if (colByNum.objectIds[i].xMax != int.MinValue)
                {
                    outputFile.WriteLine(colByNum.objectIds[i].id + "\t" + colByNum.objectIds[i].xMin + "\t" +
                                         colByNum.objectIds[i].yMin + "\t" + colByNum.objectIds[i].xMax + "\t" +
                                         colByNum.objectIds[i].yMax + "\t" + frameCounter+
                                         "\t0\t0\t0");
                }
            }

            // string recordingString = "";
            // for (int i = 0; i < gos.Length; i++)
            // {
            //     // rs[i].sb.Append(string.Concat(frameCounter, 
            //     //                          "\t" , gos[i].name, 
            //     //                          "\t" , gos[i].active,  
            //     //                          "\t" , transforms[i].position.x,  
            //     //                          "\t" , transforms[i].position.y,
            //     //                          "\t" , transforms[i].position.z,
            //     //                          "\t" , transforms[i].rotation.x ,
            //     //                          "\t" , transforms[i].rotation.y , 
            //     //                          "\t" , transforms[i].rotation.z + "\n"));
            //     // sbs[i].Append(frameCounter + 
            //     //     "\t" + gos[i].name +
            //     //     "\t" + gos[i].active +  
            //     //     "\t" + transforms[i].position.x +  
            //     //     "\t" + transforms[i].position.y +
            //     //     "\t" + transforms[i].position.z +
            //     //     "\t" + transforms[i].rotation.x +
            //     //     "\t" + transforms[i].rotation.y + 
            //     //     "\t" + transforms[i].rotation.z + "\n");
            //     
            //     stringsasdf[i] += (frameCounter + 
            //                   "\t" + gos[i].name +
            //                   "\t" + gos[i].active +  
            //                   "\t" + transforms[i].position.x +  
            //                   "\t" + transforms[i].position.y +
            //                   "\t" + transforms[i].position.z +
            //                   "\t" + transforms[i].rotation.x +
            //                   "\t" + transforms[i].rotation.y + 
            //                   "\t" + transforms[i].rotation.z + "\n");
            // }

            // for (int i = 0; i < gos.Length; i++)
            // {
            //     SerializedTransform stf = new SerializedTransform(transforms[i], gos[i].name, gos[i].active, frameCounter);
            //     bf.Serialize(fs, stf);
            // }

            frameCounter++;
        }    
    }
    

    void InitReadString(Text textField)
    {   
        //Read in dish numbers and descriptions
        string path = "C:/Users/v4rmini/Documents/Datasets/MPIICooking2/mpii-annots/dish-numbers.txt";
        string line;

        reader = new StreamReader(path);
        while ((line = reader.ReadLine()) != null)
        {
            string[] words = line.Split(',');
            dishDict.Add(words[0], line + "\n");
        }
        
        //Read in recipe steps
        //string path = "/home/jeremy/projects/MPIICookingDataset/annotation.txt";
        path = "C:/Users/v4rmini/Documents/Datasets/MPIICooking2/mpii-annots/annotation.txt";

        //Read the text from directly from the test.txt file
        reader = new StreamReader(path);
        step = 0;
        currentFileId = -1;
        line = reader.ReadLine();
        if (line != null)
        {
            lineDescription = line.Split(',');
            textField.text = line;
        }
        while((line = reader.ReadLine()) != null)  
        {
            string[] words = line.Split(',');
            //Debug.Log(words[4]);
            List<string> ttt = new List<string>();
            ttt.Add(dishDict[words[4].Trim()]);
            ttt.Add(words[0].Trim());
            ttt.Add(words[1].Trim());
            ttt.Add(words[2].Trim());
            //string t =  dishDict[words[4].Trim()] + "\n" + words[0].Trim() + " " + words[1].Trim() + " " + words[2].Trim();
            for (int i = 9; i < words.Length; i++)
            {
                if (words[i] != "" && words[i] != " ")
                {
                    ttt.Add("\n" + lineDescription[i].Trim() + ": " + words[i].Trim());
                }
            }
            //textField.text = t; //words[0] + " " + words[1] + " " + words[2] + " " + words[words.Length - 1];
            stepsList.Add(ttt);
        } 
        reader.Close();
    }

    public void StartRecording(int currentEntryId)
    {   gos = FindObjectsOfType<GameObject>() ;
        transforms = new Transform[gos.Length];
        stringsasdf = new string[gos.Length];
        sbs = new StringBuilder[gos.Length];
        for (int i = 0; i < gos.Length; i++)
        {
            transforms[i] = gos[i].transform;
            sbs[i] = new StringBuilder();

        }
        List<string> currentStep = stepsList[currentEntryId];
        string trackLabelFileName = "track_label_" + currentStep[3] + ".txt";
        string outputFileName = "output_" + currentStep[3] +"_startframe"+ (int)(colByNum.fixedUpdateCounter / 3)+".txt";
        string recordingFileName = "recording" + currentStep[3];
        ObjectId[] objectIds = FindObjectsOfType<ObjectId>();
        Array.Sort(objectIds,
            delegate(ObjectId x, ObjectId y) { return x.id.CompareTo(y.id); });
        string folderPath = "C:/Users/v4rmini/Documents/Datasets/V4R-VR-Kitchen-Dataset/";
        using (System.IO.StreamWriter file = 
            new System.IO.StreamWriter(folderPath+trackLabelFileName))
        {
            for (int i = 0; i<objectIds.Length; i++)
            {
                file.WriteLine(objectIds[i].id.ToString() + "\t" + objectIds[i].objectName);
            }
        }

        frameCounter = 0;
        outputFile = new StreamWriter(folderPath+outputFileName);
        //fs = new FileStream(folderPath+recordingFileName, FileMode.Create );
        //bf= new BinaryFormatter();
        currentlyRecording = true;
        recordSign.gameObject.SetActive(true);
    }
    
    public void StopRecording()
    {
        currentlyRecording = false;
        outputFile.Close();
        //recordingFile.Close();
        //fs.Close();    
        recordSign.gameObject.SetActive(false);
    }
}

//https://www.youtube.com/watch?v=bn8eMxBcI70