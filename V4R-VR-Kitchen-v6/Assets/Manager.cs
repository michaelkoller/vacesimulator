using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{   private List<GameObject> graspableGameObjects;
    private List<GameObject> cuttableGameObjects;
    private List<GameObject> cuttingGameObjects;
    private List<GameObject> furnitureGameObjects;
    private List<GameObject> concaveObjects;

    // Start is called before the first frame update
    void Start()
    {
        graspableGameObjects = new List<GameObject>();
        cuttableGameObjects = new List<GameObject>();
        cuttingGameObjects = new List<GameObject>();
        furnitureGameObjects = new List<GameObject>();
        concaveObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
