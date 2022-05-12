using UnityEditor;
using UnityEngine;
     
public class ScenePhysicsTool : EditorWindow {
     
    private void OnGUI()
    {
        if (GUILayout.Button("Run Physics"))
        {
            StepPhysics();
        }
    }
     
    private void StepPhysics()
    {
        Physics.autoSimulation = false;
        Physics.Simulate(Time.fixedDeltaTime);
        Physics.autoSimulation = true;
    }
     
    [MenuItem("Tools/Scene Physics")]
    private static void OpenWindow()
    {
        GetWindow<ScenePhysicsTool>(false, "Physics", true);
    }
}