using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testui : MonoBehaviour
{
    private Canvas camCanvas;

    private Image testImage;

    private Camera vidCapCam; 
    // Start is called before the first frame update
    void Start()
    {
        camCanvas = GetComponent<Canvas>();
        testImage = transform.GetChild(0).GetComponent<Image>();
        vidCapCam = GameObject.FindGameObjectWithTag("VideoCaptureCamera").GetComponent<Camera>();
        print(testImage.name);
        print(camCanvas.name);
        print(vidCapCam.name);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
