using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Vector3Serial
{
    public float x;
    public float y;
    public float z;

    public Vector3Serial(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x,y,z);
    }
}

public class SaveBlockData
{
    public int blockID;
    public Vector3Serial position;
    public Vector3Serial rotation;
}

public class MapDatas
{
    public string mapName;
    public string mapDesc;
    public SaveBlockData[] blocks;

}
