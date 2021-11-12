# VACE: Virtual Annotated Cooking Environment
A Virtual Reality Interactive Simulator for Kitchen Tasks

The VACE simulator is an richly furnished interactive 3D VR kitchen environment which enables users to efficiently record well annotated object interaction samples. The VACE dataset is available at https://sites.google.com/view/vacedataset.

 <p float="left">
   <img src="readme-resources/cuc-color.gif" width="100" />
   <img src="readme-resources/cucumber-depth.gif" width="100" /> 
   <img src="readme-resources/cucumber-seg.gif" width="100" />
 </p>

Tested on Hardware:
* 16GB RAM
* AMD Ryzen 7 3700X 8-Core Processor
* GeForce RTX 2060 SUPER

Tested on Software:
* Unity 2019.2.11f1
* SteamVR 1.20.4
* Windows 10
* HTC Vive (headset, 2 controllers, 2 base stations)
* HTC Vive tracker

## Setup Instructions:
* Setup HTC Vive and tracker
** https://www.vive.com/setup/vive/
** https://www.vive.com/de/accessory/tracker3/
* Install SteamVR
** https://store.steampowered.com/app/250820/SteamVR/
* Start SteamVR
** Do room measurement and set up at least a 4m x 4m space
* Clone repo to your machine
* Install Unity Hub: 
** https://unity3d.com/get-unity/download
** Instruction: https://docs.unity3d.com/Manual/GettingStartedInstallingHub.html
* Open Unity Hub --> Installs --> Add --> Click "Visit our download archive" -->Redirect to: https://unity3d.com/get-unity/download/archive --> Scroll to Unity 2019.2.9 --> Click "Unity Hub" --> Back in Unity Hub, complete installing this version
* Now in Unity Hub --> Projects --> Open --> Open the downloaded project --> Choose Unity version 2019.2.9
* In Unity
** Start the project
** Window -> Package Manager and let it show all available packages --> Install Burst compiler
** Install the SteamVR Unity plugin
*** https://valvesoftware.github.io/steamvr_unity_plugin/
*** Window --> SteamVR Input --> Verify that the actions "Record" and "ShowInstructions" are defined in the active action set, see below
![Action set settings](readme-resources/steamvrinput-settings.PNG)
** File --> Open Scene --> Assets/Scenes/SampleScene.unity
** In scene hierarchy, find the game object Manager, click it
** In the inspector of the Manager, make sure the controller settings are correct, see below
![Controller settings](readme-resources/input-settings.PNG)
** More controller trouble shooting available at "Assets/SteamVR/SteamVR Unity Plugin - Input System"
***http://localhost:27062/dashboard/controllerbinding.html?app=application.generated.unity.v4rvrkitchenv3.exe
** In the same inspector window, set a path for the recordings, see below
![Recording settings](readme-resources/manager-settings.PNG)
** In the hierarchy, find PlayerWithAvatar --> SteamVRObjects --> Tracker
*** In its inspector, check the device index of the tracker and choose the correct number
** Make sure "Playback" is unchecked, click "Play" (the triangle) in the top

## How to Interact
* Use the trigger or the grip button in both hands for grasping
* Grasp and hold non-furniture objects to pick them up
* Push objects without grasping them
* Grasp and hold the handles of drawers, the stove, the fridge, doors to open them
* Touch the stove buttons and the water faucet handle to turn them on/off
* Use knives and the grater to cut any food item into smaller pieces

## How to HUD
* Click X on the right controller to toggle the MPII 2 Cooking dataset recipe collection
* Click up and down on the right controller trackpad to select a recipe
* Click on the right controller right to show the next step of the recipe, left to show the previous step of the recipe
![HUD visuals](readme-resources/hud.gif)

## How to Record Samples
* When ready, click Y on the right controller, and then again to stop the recording

## How to Postprocess
* Stop the play mode after recording one or more samples, then put a check on "Playback" in the inspector of the Manager game object, and press play again. Post-processing is slower than real time.
* Add information about sample number, high level description, etc. to the readme file of the sample. That is the only annotation you need to perform manually. 

## Labeling Process and Sample Description
Please consult readme-resources/sample-description.txt to get an overview of the structure of a generated sample after postprocessing. Ground truth comes for free, and the post-processing stage allows for computationally expensive annotation like the logical predicates (on, in, etc.) and rendering multiple images per frame. The segmentation mask is the result of a shader that computes unique colors from object IDs. The depth mask uses a depth shader.
Automatic annotation is the main reason for the project and users do not have to manually label anything except provide a sample name and the high level steps they performed (i.e., which variation of a recipe they created).

## Citation
If you use this repository in your publications, please cite

```
@inproceedings{VACE2022,
author = {X},
title = {Virtual Annotated Cooking Environment: A New VR Kitchen Environment for Recording Well Annotated Object Interaction Tasks}},
booktitle= {Proceedings of the 2022 ACM/IEEE International Conference on Human-Robot Interaction},
pages = {1-2},
year = {2022},
}
```