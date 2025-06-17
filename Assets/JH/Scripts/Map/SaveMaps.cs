using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vector3Serial
{
    public float x;
    public float y;
    public float z;

    public override bool Equals(object obj)
    {
        if (!(obj is Vector3Serial)) return false;
        Vector3Serial other = (Vector3Serial)obj;
        return Mathf.Approximately(x, other.x) && Mathf.Approximately(y, other.y) && Mathf.Approximately(z, other.z);
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2;
    }

    public static bool operator ==(Vector3Serial a, Vector3Serial b)
    {
        return Mathf.Approximately(a.x, b.x) &&
               Mathf.Approximately(a.y, b.y) &&
               Mathf.Approximately(a.z, b.z);
    }

    public static bool operator !=(Vector3Serial a, Vector3Serial b)
    {
        return !(a == b);
    }

    public Vector3Serial(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3Serial(Vector3 vec)
    {
        this.x = vec.x;
        this.y = vec.y;
        this.z = vec.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

[System.Serializable]
public class SaveBlockData
{
    public int blockID;
    public Vector3Serial position;
    public Vector3Serial rotation;
}

[System.Serializable]
public class MapDatas
{
    public string mapName;
    public string mapDesc;
    public Vector3Serial startPosition;
    public SaveBlockData[] blocks;
}
