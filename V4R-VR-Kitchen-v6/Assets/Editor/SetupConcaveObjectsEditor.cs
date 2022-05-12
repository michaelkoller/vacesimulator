using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SetupConcaveObjects))]
public class SetupConcaveObjectsEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Call Fct"))
        {
            SetupConcaveObjects setupFct = (SetupConcaveObjects)target;
            setupFct.Call();
        }
    }
}
