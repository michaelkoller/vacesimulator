using UnityEditor;
using UnityEngine;
     
public class HeadResetTool : EditorWindow {
     
    private void OnGUI()
    {
        if (GUILayout.Button("Reset Head Position"))
        {
            ResetHead();
        }
    }
     
    private void ResetHead()
    {
        GameObject head = GameObject.Find("Head");
        head.transform.localPosition = new Vector3(0f,0f,0f);
    }
     
    [MenuItem("Tools/Reset Head")]
    private static void OpenWindow()
    {
        GetWindow<HeadResetTool>(false, "Reset Head", true);
    }
}