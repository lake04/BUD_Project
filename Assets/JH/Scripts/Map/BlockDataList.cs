using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockData
{
    public Sprite image;
    public GameObject prefab;
    public string name;
    public string description;
}


[CreateAssetMenu(fileName = "BlockDataList", menuName = "ScriptableObjects/BlockDataList", order = 1)]
public class BlockDataList : ScriptableObject
{
    public BlockData[] data;
}

