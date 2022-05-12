using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerMovement : MonoBehaviour
{
    public SteamVR_Input_Sources myInputSource;
    public SteamVR_Action_Vector2 LeftTrackInputVector;
    public SteamVR_Action_Boolean EnableMovement;


    public float MovementSpeed = 0;
    private Transform PlayerTransform;
    private Vector3 PostitionChange;
    private bool Enabled = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayerTransform = GetComponent<Transform>();
        LeftTrackInputVector = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("ControllerMovement");
        EnableMovement = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("EnableControllerMovement");

    }

    // Update is called once per frame
    void Update()
    {

        //print(PlayerTransform.position);

        if (EnableMovement.GetStateDown(myInputSource)) {
            Enabled = !Enabled;
            print("i am pressing button");
        }

        if (Enabled) { 
            PostitionChange = new Vector3(LeftTrackInputVector.axis[0], 0, LeftTrackInputVector.axis[1]);

            PlayerTransform.position= new Vector3 (PlayerTransform.position.x - MovementSpeed*LeftTrackInputVector.axis[0], PlayerTransform.position.y, PlayerTransform.position.z - MovementSpeed*LeftTrackInputVector.axis[1]) ;
        }
    }
}
