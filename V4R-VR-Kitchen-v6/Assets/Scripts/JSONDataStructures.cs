using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONDataStructures
{
    
[System.Serializable]
public class PositionAndRotationFrameArr
{
    public string type = "position_and_rotation_frame_arr";
    public string recipe = "";
    public string creation_time = "";
    public List<PositionAndRotationFrame> positionAndRotationFrameArr = new List<PositionAndRotationFrame>();
}

[System.Serializable]
public class PositionAndRotationFrame
{
    public string type = "position_and_rotation_frame";
    public int frame_number = 0;
    public float time = 0.0f;
    public float delta_time = 0.0f;
    public List<ObjectPosition> objectPositionAndRotationArr = new List<ObjectPosition>();
}

[System.Serializable]
public class ObjectPosition
{
    public string type = "object_pos";
    public string name = "";
    public float posX = 0.0f;
    public float posY = 0.0f;
    public float posZ = 0.0f;
    public float angX = 0.0f;
    public float angY = 0.0f;
    public float angZ = 0.0f;
}


[System.Serializable]
public class ColormapJSON
{
    public List<ColormapEntryJSON> object_colors = new List<ColormapEntryJSON>();
}

[System.Serializable]
public class ColormapEntryJSON
{
    public string name = "";
    public float r = 0f;
    public float g = 0f;
    public float b = 0f;
    public float a = 0f;
    public int id_no = -1;
}


[System.Serializable]
public class BbFrameArray
{
    public List<BbFrame> bb_frame_arr = new List<BbFrame>();
}

[System.Serializable]
public class BbFrame
{
    public int frame_number = -1;
    public List<BbObject> bb_obect_arr = new List<BbObject>();
}

[System.Serializable]
public class BbObject
{
    public string name = "";
    public int id_no = -1;
    public int x_max = -1;
    public int x_min = -1;
    public int y_max = -1;
    public int y_min = -1;
}

[System.Serializable]
public class CutRecord
{
    public int frame = -1;
    public string name = "";
    public float contact_point_x = 0f;
    public float contact_point_y = 0f;
    public float contact_point_z = 0f;
    public float cut_direction_x = 0f;
    public float cut_direction_y = 0f;
    public float cut_direction_z = 0f;
}

[System.Serializable]
public class CutRecords
{
    public List<CutRecord> cuts = new List<CutRecord>();
}

[System.Serializable]
public class GraspRecord
{
    public int frame = -1;
    public string grasp_type = ""; //grasp / release
    public string grasped_object = "";
    public string hand = "";
}

[System.Serializable]
public class PushRecords
{
    public List<PushRecord> pushes = new List<PushRecord>();
}


[System.Serializable]
public class PushRecord
{
    public int frame = -1;
    public string push_type = ""; //start_pushing / stop_pushing
    public string pushed_object = "";
    public string hand = "";
}

[System.Serializable]
public class GraspRecords
{
    public List<GraspRecord> grasps = new List<GraspRecord>();
}

[System.Serializable]
public class InPredicateRecord
{
    public int frame = -1;
    public string inside_object = "";
    public string container_object = "";
    public string relation_type = ""; //entered or left
}

[System.Serializable]
public class InPredicateRecords
{
    public List<InPredicateRecord> inPredicateRecords = new List<InPredicateRecord>();
}

[System.Serializable]
public class OnPredicateRecord
{
    public int frame = -1;
    public string top_object = "";
    public string bottom_object = "";
    public string relation_type = ""; //start_touching or end_touching
}

[System.Serializable]
public class OnPredicateRecords
{
    public List<OnPredicateRecord> onPredicateRecords = new List<OnPredicateRecord>();
}
}
