using UnityEditor;
using UnityEngine;
     
public class CenterKitchenLocation : EditorWindow {
     
    private void OnGUI()
    {
        if (GUILayout.Button("Center Player Position in Kitchen"))
        {
            CenterPlayer();
        }
    }
     
    private void CenterPlayer()
    {
        GameObject player = GameObject.Find("PlayerWithAvatar");
        GameObject vrCamera = GameObject.Find("VRCamera");
        Debug.Log(player.transform.position);
        Debug.Log(vrCamera.transform.position);
        player.transform.position = player.transform.position +  new Vector3(player.transform.position.x - vrCamera.transform.position.x,0f,player.transform.position.z - vrCamera.transform.position.z);
    }
     
    [MenuItem("Tools/Center Player Position in Kitchen")]
    private static void OpenWindow()
    {
        GetWindow<CenterKitchenLocation>(false, "Center Player Position in Kitchen", true);
    }
}