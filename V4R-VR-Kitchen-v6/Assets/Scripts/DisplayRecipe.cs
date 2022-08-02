using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
using Valve.VR;
using System.Text;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;

public class DisplayRecipe : MonoBehaviour
{        
    public SteamVR_Input_Sources myInputSource;
    public SteamVR_Action_Boolean clickRightAction;
    public SteamVR_Action_Boolean clickLeftAction;
    public SteamVR_Action_Boolean clickUpAction;
    public SteamVR_Action_Boolean clickDownAction;
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
    private FileStream fs;
    private BinaryFormatter bf;
    private StringBuilder[] sbs;
    public string dish_numbers_path;
    public string annotation_path;
    
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
    {   //Next step in current recipe
        if (clickRightAction.GetStateDown(myInputSource)) 
        {
            currentEntryNumber = Math.Min(currentEntryNumber + 1, stepsList.Count - 1);
            textField.text = string.Join(" ", stepsList[currentEntryNumber]);
            vrTextField.text = textField.text;
            currentFileId = Int32.Parse(stepsList[currentEntryNumber][2]);
        }
        //Previous step in current recipe
        if (clickLeftAction.GetStateDown(myInputSource))
        {
            currentEntryNumber = Math.Max(currentEntryNumber-1, 0);
            textField.text = string.Join(" ", stepsList[currentEntryNumber]);
            vrTextField.text = textField.text;
            currentFileId = Int32.Parse(stepsList[currentEntryNumber][2]);
        }
        //Next recipe
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
        //Previous recipe
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
        //Toggle visibility of Recipe HUD
        if (clickShowInstructions.GetStateDown(myInputSource))
        {
            showInstructions = !showInstructions;
            vrCanvas.SetActive(showInstructions);
        }
    }

    void InitReadString(Text textField)
    {
        //Read in dish numbers and descriptions
        //dish_numbers_path = "C:/Users/v4rmini/Documents/Datasets/MPIICooking2/mpii-annots/dish-numbers.txt";
        //string line;

        //reader = new StreamReader(dish_numbers_path);
        //while ((line = reader.ReadLine()) != null)
        //{
        //    string[] words = line.Split(',');
        //    dishDict.Add(words[0], line + "\n");
        //}


        ////Read in recipe steps
        ////string path = "/home/jeremy/projects/MPIICookingDataset/annotation.txt";
        ////annotation_path = "C:/Users/v4rmini/Documents/Datasets/MPIICooking2/mpii-annots/annotation.txt";

        ////Read the text from directly from the test.txt file
        //reader = new StreamReader(annotation_path);
        //step = 0;
        //currentFileId = -1;
        //line = reader.ReadLine();
        //if (line != null)
        //{
        //    lineDescription = line.Split(',');
        //    textField.text = line;
        //}
        //while((line = reader.ReadLine()) != null)  
        //{
        //    string[] words = line.Split(',');
        //    List<string> ttt = new List<string>();
        //    ttt.Add(dishDict[words[4].Trim()]);
        //    ttt.Add(words[0].Trim());
        //    ttt.Add(words[1].Trim());
        //    ttt.Add(words[2].Trim());
        //    for (int i = 9; i < words.Length; i++)
        //    {
        //        if (words[i] != "" && words[i] != " ")
        //        {
        //            ttt.Add("\n" + lineDescription[i].Trim() + ": " + words[i].Trim());
        //        }
        //    }
        //    stepsList.Add(ttt);
        //} 
        //reader.Close();
        var temp1 = Resources.Load<TextAsset>("Annotations/dish-numbers");
        string[] lines1 = temp1.text.Split(
           new string[] { "\r\n", "\r", "\n" },
           StringSplitOptions.None
       );

        //Debug.Log(lines1[0]);

        foreach (var line in lines1)
        {
            string[] words = line.Split(',');
            dishDict.Add(words[0], line + "\n");
        }


        var temp2 = Resources.Load<TextAsset>("Annotations/annotation");

        string[] lines2 = temp2.text.Split(
            new string[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
        );


        lineDescription = lines2[0].Split(',');
        textField.text = lines2[0];

        for (int j = 1; j < lines2.Length; j++)
        {
            string[] words = lines2[j].Split(',');
            List<string> ttt = new List<string>();

            if (words.Length >= 4) { 
                ttt.Add(dishDict[words[4].Trim()]);
                ttt.Add(words[0].Trim());
                ttt.Add(words[1].Trim());
                ttt.Add(words[2].Trim());
            }
            else {
                continue;
            }

            for (int i = 9; i < words.Length; i++)
            {
                if (words[i] != "" && words[i] != " ")
                {
                    ttt.Add("\n" + lineDescription[i].Trim() + ": " + words[i].Trim());
                }
            }
            stepsList.Add(ttt);
        }
    }
}

//https://www.youtube.com/watch?v=bn8eMxBcI70