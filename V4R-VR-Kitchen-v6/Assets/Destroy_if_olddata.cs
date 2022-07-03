using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy_if_olddata : MonoBehaviour
{
    private PlayModeManager playModeManager;
    // Start is called before the first frame update

    private void Awake()
    {
        GameObject gameobject = GameObject.Find("Manager");
        playModeManager = gameobject.GetComponent<PlayModeManager>();
        if (playModeManager.OldAnnotationData == true)
        {
            gameObject.SetActive(false);
        }
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
