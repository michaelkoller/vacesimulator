using UnityEngine;
using System.Collections;
public class GuiPosition : MonoBehaviour 
{

    public Vector2 WorldToGuiPoint(Vector3 GOposition)
    {
        var guiPosition = Camera.main.WorldToScreenPoint(GOposition);
        // Y axis coordinate in screen is reversed relative to world Y coordinate
        guiPosition.y = Screen.height - guiPosition.y;

        return guiPosition;
    }

    void OnGUI() 
    {
        var guiPosition = WorldToGuiPoint(gameObject.transform.position);
        var rect = new Rect(guiPosition, new Vector2(200, 70));
        GUI.Label(rect, "TEST");
    }
}