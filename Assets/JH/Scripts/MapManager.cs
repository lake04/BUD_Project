using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public BlockDataList blockDataList; 
    public Transform parentForBlocks;   

    void Start()
    {
        LoadRandomMap();
    }

    public void LoadRandomMap()
    {
        TextAsset[] mapFiles = Resources.LoadAll<TextAsset>("Maps");

        if (mapFiles.Length == 0)
        {
            Debug.LogWarning("맵 파일이 없습니다.");
            return;
        }

        int index = Random.Range(0, mapFiles.Length);
        TextAsset selectedMapFile = mapFiles[index];

        MapDatas loadedMap = JsonUtility.FromJson<MapDatas>(selectedMapFile.text);

        Debug.Log($"불러온 맵: {loadedMap.mapName}");

        foreach (var block in loadedMap.blocks)
        {
            if (block.blockID < 0 || block.blockID >= blockDataList.data.Count())
            {
                Debug.LogWarning($"없는 블럭ID: {block.blockID}");
                continue;
            }

            GameObject prefab = blockDataList.data[block.blockID].prefab;
            Vector3 position = block.position.ToVector3();
            Quaternion rotation = Quaternion.Euler(block.rotation.ToVector3());

            Instantiate(prefab, position, rotation, parentForBlocks);
        }
    }
}
