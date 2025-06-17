using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{

    public static MapManager Instance;
    public BlockDataList blockDataList; 
    public Transform parentForBlocks;
    public GameObject player;

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
        this.gameObject.SetActive(true);
    }

    void Start()
    {
  
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scen, LoadSceneMode mode)
    {
        if (isEditorMode == false)
        {
            LoadRandomStageMap();
            //TestLoad();
        }
    }

    public List<SaveBlockData> TestLoad()
    {
        string json = PlayerPrefs.GetString("List<SaveBlockData>", "{ }");
        return JsonUtility.FromJson<List<SaveBlockData>>(json);
    }

    public void LoadRandomStageMap()
    {
        // 1. "Maps/Stage" ����� ��� TextAsset �ε�
        TextAsset[] mapAssets = Resources.LoadAll<TextAsset>("Maps/Stage");

        if (mapAssets == null || mapAssets.Length == 0)
        {
            Debug.LogWarning("Resources/Maps/Stage ������ json ���� �����ϴ�.");
            return;
        }

        // 2. ���� ����
        int randomIndex = Random.Range(0, mapAssets.Length);
        TextAsset selectedMap = mapAssets[randomIndex];

        Debug.Log($"[�������� ��] ���� �ε�: {selectedMap.name}");

        // 3. json �Ľ� �� �� ����
        MapDatas loadedMap = JsonUtility.FromJson<MapDatas>(selectedMap.text);

        // ���� ��� ����
        if (parentForBlocks != null)
        {
            foreach (Transform child in parentForBlocks)
            {
                Destroy(child.gameObject);
            }
        }

        // ��� ��ġ
        if (loadedMap.blocks != null)
        {
            foreach (var block in loadedMap.blocks)
            {
                if (block.blockID < 0 || block.blockID >= blockDataList.data.Length)
                    continue;

                if (block.blockID == blockDataList.data.Length - 1)
                    continue; // ���� ��ġ�� ���� ó��

                GameObject prefab = blockDataList.data[block.blockID].prefab;
                if (prefab == null)
                    continue;

                Vector3 pos = block.position.ToVector3();
                Quaternion rot = Quaternion.Euler(block.rotation.ToVector3());
                Instantiate(prefab, pos, rot, parentForBlocks);
            }
        }

        // �÷��̾� ��ġ
        if (loadedMap.startPosition != null)
        {
            Vector3 spawnPos = loadedMap.startPosition.ToVector3();
            Instantiate(player, spawnPos, Quaternion.identity);
            Debug.Log($"[�������� ��] �÷��̾� ���� ��ġ: {spawnPos}");
        }
        else
        {
            Debug.LogError("���� ��ġ ������ �����ϴ�.");
        }
    }


}
