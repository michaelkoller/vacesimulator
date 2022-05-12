//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor.Recorder;
//
//
//public class UnityRecorderFromScript : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {
//        StartAndStopRecording_WithValidSettings_ShouldStartThenStopRecording();
//    }
//
//    // Update is called once per frame
//    void Update()
//    {
//        
//    }
//    
//    public void StartAndStopRecording_WithValidSettings_ShouldStartThenStopRecording()
//    {
//        var settings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
//        var imageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
//        
//        settings.AddRecorderSettings(imageRecorder);
//        var recorderController = new RecorderController(settings);
//
//        // recorderController.StartRecording();
//        // recorderController.IsRecording();
//
//        // recorderController.StopRecording();
//        // recorderController.IsRecording();
//			
//        Object.DestroyImmediate(imageRecorder);
//        Object.DestroyImmediate(settings);
//    }
//}
