//using UnityEngine;
//using UnityEditor.Animations;
//
//public class MyGameObjectRecorder : MonoBehaviour
//{
//    public AnimationClip clip;
//    private bool save = true;
//    private GameObjectRecorder m_Recorder;
//    public GameObject recordThisGameObject;
//
//    void Start()
//    {
//        // Create recorder and record the script GameObject.
//        m_Recorder = new GameObjectRecorder(gameObject);
//
//        // Bind all the Transforms on the GameObject and all its children.
//        m_Recorder.BindComponentsOfType<Transform>(recordThisGameObject, true);
//    }
//
//    void LateUpdate()
//    {
//        if (clip == null)
//            return;
//
//        // Take a snapshot and record all the bindings values for this frame.
//        m_Recorder.TakeSnapshot(Time.deltaTime);
//
//        if (Time.time > 5f && save)
//        {
//            save = false;
//            if (m_Recorder.isRecording)
//            {
//                // Save the recorded session to the clip.
//                m_Recorder.SaveToClip(clip);
//            }
//        }
//    }
//
//    void OnDisable()
//    {
//        if (clip == null)
//            return;
//
//        if (m_Recorder.isRecording)
//        {
//            // Save the recorded session to the clip.
//            m_Recorder.SaveToClip(clip);
//        }
//    }
//}