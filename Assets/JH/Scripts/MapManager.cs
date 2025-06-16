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
            LoadRandomMap();
            TestLoad();
        }
    }

    public List<SaveBlockData> TestLoad()
    {
        string json = PlayerPrefs.GetString("List<SaveBlockData>", "{ }");
        return JsonUtility.FromJson<List<SaveBlockData>>(json);
    }

    public void LoadRandomMap()
    {

        string mapsFolderPath = Path.Combine(Application.persistentDataPath, "Resources", "Maps");
     
        if (!Directory.Exists(mapsFolderPath))
        {
            Debug.LogWarning($"맵 파일 폴더가 존재하지 않습니다: {mapsFolderPath}");
            return;
        }

        string[] mapFilePaths = Directory.GetFiles(mapsFolderPath, "*.json");

        if (mapFilePaths.Length == 0)
        {
            Debug.LogWarning($"저장된 맵 파일이 없습니다. 경로: {mapsFolderPath}");
            return;
        }

        int index = Random.Range(0, mapFilePaths.Length);
        string selectedMapFilePath = mapFilePaths[index];

        string jsonText = File.ReadAllText(selectedMapFilePath);

        MapDatas loadedMap = JsonUtility.FromJson<MapDatas>(jsonText);
        isEditorMode = false;
        Debug.Log($"불러온 맵: {loadedMap.mapName} (파일 경로: {selectedMapFilePath})");

        if (parentForBlocks != null)
        {
            foreach (Transform child in parentForBlocks)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("parentForBlocks가 할당되지 않았습니다. 블록들이 씬의 루트에 생성됩니다.");
        }


        if (loadedMap.blocks != null)
        {
            foreach (var block in loadedMap.blocks)
            {
                // (유효성 검사 및 프리팹 생성 로직은 기존과 동일)
                if (block.blockID < 0 || block.blockID >= blockDataList.data.Length)
                {
                    Debug.LogWarning($"없는 블록 ID: {block.blockID}. 스킵합니다.");
                    continue;
                }
                GameObject prefab = blockDataList.data[block.blockID].prefab;
                if (prefab == null)
                {
                    Debug.LogWarning($"블록 ID {block.blockID}의 프리팹이 없습니다.");
                    continue;
                }

                // 시작 위치 블록은 씬에 생성하지 않음 (게임 플레이 시에는 보일 필요가 없으므로)
                if (block.blockID == blockDataList.data.Length - 1)
                {
                    continue;
                }

                Vector3 position = block.position.ToVector3();
                Quaternion rotation = Quaternion.Euler(block.rotation.ToVector3());
                Instantiate(prefab, position, rotation, parentForBlocks);
            }
        }

        // 5. 모든 맵 로딩이 끝난 후, 불러온 맵의 시작 위치에 플레이어 생성
        if (loadedMap.startPosition != null)
        {
            Vector3 spawnPos = loadedMap.startPosition.ToVector3();
            Instantiate(player, spawnPos, Quaternion.identity);
            Debug.Log($"플레이어를 {spawnPos} 위치에 생성했습니다.");
        }
        else
        {
            Debug.LogError($"'{loadedMap.mapName}' 맵에 시작 위치 데이터가 없습니다! 플레이어를 생성할 수 없습니다.");
        }

    }

}
