using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class EditorUI : MonoBehaviour
{
    public static EditorUI Instance;
    public MapDatas currentMapData;
    public List<SaveBlockData> currentBlocks = new List<SaveBlockData>();

    public GameObject buttonPrefab;
    public InputField mapNameInput;
    public Transform buttonSpawnPrefab;
    public BlockDataList blockDataList;

    public GameObject pointerButton;

    Vector3 realPos;
    public int currentBlockIndex;

    public Vector3Serial startPos;
    private bool isStartPositionSet = false;

    private List<GameObject> instantiatedEditorBlocks = new List<GameObject>();

    public HashSet<Vector2Int> gridMap;

    private float cameraMoveSpeed = 100f;
    [SerializeField] private TMP_Text description;

    private const string DefaultMapName = "New Map";
    private const string DefaultMapDesc = "This is a new map.";

    private string MapSavePath;

    public float snapSize;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        Init();
    }

    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            MouseMove();
            OnSpawnButton();
            RemoveClick();
        }

        CameraMovement();
    }

    private void Init()
    {
        if (blockDataList == null)
        {
            Debug.LogError("스크립터블 오브젝트가 없습니다.");
            return;
        }

        currentMapData = new MapDatas
        {
            mapName = DefaultMapName,
            mapDesc = DefaultMapDesc
        };

        pointerButton = Instantiate(pointerButton);

        foreach (var blockData in blockDataList.data)
        {
            GameObject tempObj = Instantiate(buttonPrefab, buttonSpawnPrefab);
            tempObj.GetComponent<Button>().onClick.AddListener(() => OnSelectClick(blockData));
            tempObj.GetComponentInChildren<Image>().sprite = blockData.image;
            tempObj.GetComponentsInChildren<Text>()[0].text = blockData.name;
            tempObj.GetComponentsInChildren<Text>()[1].text = blockData.description;
        }

        gridMap = new HashSet<Vector2Int>();

        MapSavePath = Path.Combine(Application.persistentDataPath, "Resources", "Maps");
    }

    public void CameraMovement()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) cameraMoveSpeed = 150f;
        if (Input.GetKeyUp(KeyCode.LeftShift)) cameraMoveSpeed = 100;

        Camera.main.transform.position += new Vector3(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"),
            0) * Time.deltaTime * cameraMoveSpeed;
    }

    public void MouseMove()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        float snappedX = Mathf.Round(mousePos.x / snapSize) * snapSize;
        float snappedY = Mathf.Round(mousePos.y / snapSize) * snapSize;

        realPos = new Vector3(snappedX, snappedY, 0);
        pointerButton.transform.position = realPos;
    }



    public void OnSpawnButton()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int curPos = new Vector2Int(Mathf.RoundToInt(realPos.x), Mathf.RoundToInt(realPos.y));

            RaycastHit2D hit = Physics2D.Raycast(realPos, Vector2.zero);
            if (hit.collider == null || gridMap.Contains(curPos)) return;

            if (currentBlockIndex == blockDataList.data.Length - 1 && isStartPositionSet) return;

            gridMap.Add(curPos);

            if (currentBlockIndex == blockDataList.data.Length - 1)
            {
                Debug.Log("시작 위치 설치");
                startPos = new Vector3Serial(realPos.x, realPos.y, 0);
                isStartPositionSet = true;
            }

            GameObject blockPrefab = Instantiate(blockDataList.data[currentBlockIndex].prefab, realPos, pointerButton.transform.rotation);
            instantiatedEditorBlocks.Add(blockPrefab);

            currentBlocks.Add(new SaveBlockData
            {
                blockID = currentBlockIndex,
                position = new Vector3Serial(realPos.x, realPos.y, 0),
                rotation = new Vector3Serial(pointerButton.transform.rotation.eulerAngles.x,
                                             pointerButton.transform.rotation.eulerAngles.y,
                                             pointerButton.transform.rotation.eulerAngles.z)
            });

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            pointerButton.transform.Rotate(0, 0, 45);
        }
    }

    private void RemoveClick()
    {
        if (!Input.GetMouseButtonDown(1)) return;
        int blockIndex = currentBlocks.FindIndex(x => x.position == new Vector3Serial(realPos.x, realPos.y, 0));
        Debug.Log(blockIndex);
        try{
            Destroy(instantiatedEditorBlocks[blockIndex].gameObject);
            instantiatedEditorBlocks.RemoveAt(blockIndex);
            currentBlocks.RemoveAt(blockIndex);
            Vector2Int curPos = new Vector2Int(Mathf.RoundToInt(realPos.x), Mathf.RoundToInt(realPos.y));
            gridMap.Remove(curPos);
        }
        catch
        {
            Debug.LogWarning("삭제할 블럭이 없습니다!");
        }
    }

    private void DestroyBlock(GameObject obj, SaveBlockData data, Vector2Int pos)
    {
        Destroy(obj);
        instantiatedEditorBlocks.Remove(obj);
        currentBlocks.Remove(data);
        gridMap.Remove(pos);
    }

    public void OnSaveButton()
    {
        currentMapData.mapName = mapNameInput.text;
        currentMapData.blocks = currentBlocks.ToArray();
        currentMapData.startPosition = startPos;

        string saveFolder = currentMapData.mapName.ToLower().Contains("stage") ?
                            Path.Combine(Application.dataPath, "Resources", "Maps", "Stage") :
                            Path.Combine(Application.persistentDataPath, "Maps", "User");

        if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

        string jsonData = JsonUtility.ToJson(currentMapData, true);
        string path = Path.Combine(saveFolder, currentMapData.mapName + ".json");
        File.WriteAllText(path, jsonData);

        Debug.Log($"맵 저장됨: {path}");
    }


    public void LoadUserMap(string mapName)
    {
        string path = Path.Combine(Application.persistentDataPath, "Maps", "User", mapName + ".json");

        if (!File.Exists(path))
        {
            Debug.LogWarning($"유저 맵 파일을 찾을 수 없습니다: {path}");
            return;
        }

        string jsonText = File.ReadAllText(path);
        MapDatas loadedMap = JsonUtility.FromJson<MapDatas>(jsonText);

        // 기존 블록들 제거
        foreach (GameObject block in instantiatedEditorBlocks)
            Destroy(block);

        instantiatedEditorBlocks.Clear();
        currentBlocks.Clear();
        gridMap.Clear();

        // 데이터 갱신
        currentMapData = loadedMap;
        mapNameInput.text = loadedMap.mapName;

        if (loadedMap.blocks != null)
        {
            foreach (var block in loadedMap.blocks)
            {
                if (block.blockID < 0 || block.blockID >= blockDataList.data.Length)
                    continue;

                GameObject prefab = blockDataList.data[block.blockID].prefab;
                if (prefab == null)
                    continue;

                Vector3 position = block.position.ToVector3();
                Quaternion rotation = Quaternion.Euler(block.rotation.ToVector3());

                GameObject instantiatedBlock = Instantiate(prefab, position, rotation);
                instantiatedEditorBlocks.Add(instantiatedBlock);
                currentBlocks.Add(block);
                gridMap.Add(new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y)));
            }
            isStartPositionSet = true;
            startPos = loadedMap.startPosition;
        }

        Debug.Log($"유저 맵 로드 완료: {mapName}");
    }


    public void LoadMapButton()
    {
        string mapNameToLoad = mapNameInput.text.Trim();

        if (string.IsNullOrEmpty(mapNameToLoad))
        {
            Debug.LogWarning("맵 이름을 입력해주세요!");
            return;
        }

        LoadUserMap(mapNameToLoad);
    }



    public void OnClickTitle()
    {
        SceneController.Instance.TitleLoad();
    }

    void OnSelectClick(BlockData blockData)
    {
        if (blockData == null || blockData.image == null) return;
        currentBlockIndex = blockDataList.data.ToList().IndexOf(blockData);
        pointerButton.GetComponent<SpriteRenderer>().sprite = blockData.image;
    }

    public void OnClearClick()
    {
        isStartPositionSet = false;
        while(instantiatedEditorBlocks.Count > 0)
        {
            int i = 0;
            Vector2Int blockPos = new Vector2Int(Mathf.RoundToInt(instantiatedEditorBlocks[i].transform.position.x), Mathf.RoundToInt(instantiatedEditorBlocks[i].transform.position.y));
            DestroyBlock(instantiatedEditorBlocks[i], currentBlocks[i], blockPos);
            Debug.Log("블럭 삭제");
            i++;
        }
     
        Debug.Log(instantiatedEditorBlocks.Count);
        Debug.Log(currentBlocks.Count);
    }
}
