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
        // 1. "Maps/Stage" 경로의 모든 TextAsset 로드
        TextAsset[] mapAssets = Resources.LoadAll<TextAsset>("Maps/Stage");

        if (mapAssets == null || mapAssets.Length == 0)
        {
            Debug.LogWarning("Resources/Maps/Stage 폴더에 json 맵이 없습니다.");
            return;
        }

        // 2. 랜덤 선택
        int randomIndex = Random.Range(0, mapAssets.Length);
        TextAsset selectedMap = mapAssets[randomIndex];

        Debug.Log($"[스테이지 맵] 랜덤 로드: {selectedMap.name}");

        // 3. json 파싱 및 맵 생성
        MapDatas loadedMap = JsonUtility.FromJson<MapDatas>(selectedMap.text);

        // 기존 블록 삭제
        if (parentForBlocks != null)
        {
            foreach (Transform child in parentForBlocks)
            {
                Destroy(child.gameObject);
            }
        }

        // 블록 배치
        if (loadedMap.blocks != null)
        {
            foreach (var block in loadedMap.blocks)
            {
                if (block.blockID < 0 || block.blockID >= blockDataList.data.Length)
                    continue;

                if (block.blockID == blockDataList.data.Length - 1)
                    continue; // 시작 위치는 따로 처리

                GameObject prefab = blockDataList.data[block.blockID].prefab;
                if (prefab == null)
                    continue;

                Vector3 pos = block.position.ToVector3();
                Quaternion rot = Quaternion.Euler(block.rotation.ToVector3());
                Instantiate(prefab, pos, rot, parentForBlocks);
            }
        }

        // 플레이어 배치
        if (loadedMap.startPosition != null)
        {
            Vector3 spawnPos = loadedMap.startPosition.ToVector3();
            Instantiate(player, spawnPos, Quaternion.identity);
            Debug.Log($"[스테이지 맵] 플레이어 생성 위치: {spawnPos}");
        }
        else
        {
            Debug.LogError("시작 위치 정보가 없습니다.");
        }
    }


}
