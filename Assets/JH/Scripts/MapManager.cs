using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    public static MapManager Instance;
    public BlockDataList blockDataList; 
    public Transform parentForBlocks;

    public bool isEditorMode;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadRandomMap();
    }

    public void LoadRandomMap()
    {
      
        string mapsFolderPath = Path.Combine(Application.persistentDataPath, "Resources", "Maps");

        if (!Directory.Exists(mapsFolderPath))
        {
            Debug.LogWarning($"�� ���� ������ �������� �ʽ��ϴ�: {mapsFolderPath}");
            return;
        }

        string[] mapFilePaths = Directory.GetFiles(mapsFolderPath, "*.json");

        if (mapFilePaths.Length == 0)
        {
            Debug.LogWarning($"����� �� ������ �����ϴ�. ���: {mapsFolderPath}");
            return;
        }

        int index = Random.Range(0, mapFilePaths.Length);
        string selectedMapFilePath = mapFilePaths[index];

        string jsonText = File.ReadAllText(selectedMapFilePath);

        MapDatas loadedMap = JsonUtility.FromJson<MapDatas>(jsonText);
        isEditorMode = false;
        Debug.Log($"�ҷ��� ��: {loadedMap.mapName} (���� ���: {selectedMapFilePath})");

        if (parentForBlocks != null)
        {
            foreach (Transform child in parentForBlocks)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("parentForBlocks�� �Ҵ���� �ʾҽ��ϴ�. ��ϵ��� ���� ��Ʈ�� �����˴ϴ�.");
        }


        if (loadedMap.blocks != null)
        {
            foreach (var block in loadedMap.blocks)
            {
                if (block.blockID < 0 || block.blockID >= blockDataList.data.Length)
                {
                    Debug.LogWarning($"���� ��� ID: {block.blockID}. �ش� ����� ��ŵ�˴ϴ�.");
                    continue;
                }

                GameObject prefab = blockDataList.data[block.blockID].prefab;
                if (prefab == null)
                {
                    Debug.LogWarning($"��� ID {block.blockID}�� �ش��ϴ� �������� BlockDataList�� �Ҵ���� �ʾҽ��ϴ�.");
                    continue;
                }

                Vector3 position = block.position.ToVector3();
                Quaternion rotation = Quaternion.Euler(block.rotation.ToVector3());

                Instantiate(prefab, position, rotation, parentForBlocks);
            }
        }
        else
        {
            Debug.LogWarning("�ҷ��� �� �����Ϳ� ��� ������ �����ϴ�.");
        }

        
    }

}
