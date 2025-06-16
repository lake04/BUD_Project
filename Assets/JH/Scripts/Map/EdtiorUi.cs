using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

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

    private float cameraMoveSpeed = 5;
    [SerializeField] private TMP_Text description;

    private const string DefaultMapName = "New Map";
    private const string DefaultMapDesc = "This is a new map.";

    private string MapSavePath => Path.Combine(Application.persistentDataPath, "Maps");

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
            tempObj.GetComponentInChildren<Text>().text = blockData.name;
        }

        gridMap = new HashSet<Vector2Int>();
    }

    public void CameraMovement()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) cameraMoveSpeed = 10f;
        if (Input.GetKeyUp(KeyCode.LeftShift)) cameraMoveSpeed = 5f;

        Camera.main.transform.position += new Vector3(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"),
            0) * Time.deltaTime * cameraMoveSpeed;
    }

    public void MouseMove()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        realPos = new Vector3(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y), 0);
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

        RaycastHit2D hit = Physics2D.Raycast(realPos, Vector2.zero);
        if (hit.collider == null || hit.collider.CompareTag("Unremovable")) return;

        Vector2Int curPos = new Vector2Int(Mathf.RoundToInt(realPos.x), Mathf.RoundToInt(realPos.y));
        if (!gridMap.Contains(curPos)) return;

        Vector3 pos = hit.collider.transform.position;

        SaveBlockData blockToRemove = currentBlocks.FirstOrDefault(b => Mathf.Approximately(b.position.x, pos.x) && Mathf.Approximately(b.position.y, pos.y));

        if (blockToRemove.blockID == blockDataList.data.Length - 1) isStartPositionSet = false;

        DestroyBlock(hit.collider.gameObject, blockToRemove, curPos);
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
        if (!isStartPositionSet)
        {
            Debug.LogError("맵을 저장하려면 시작 위치를 설정해야 합니다!");
            return;
        }

        currentMapData.mapName = mapNameInput.text;
        currentMapData.blocks = currentBlocks.ToArray();
        currentMapData.startPosition = startPos;

        string jsonData = JsonUtility.ToJson(currentMapData, true);
        if (!Directory.Exists(MapSavePath)) Directory.CreateDirectory(MapSavePath);

        string path = Path.Combine(MapSavePath, currentMapData.mapName + ".json");
        File.WriteAllText(path, jsonData);

        Debug.Log("저장 경로: " + path);
    }

    public void LoadMapButton()
    {
        string mapNameToLoad = mapNameInput.text;
        if (string.IsNullOrWhiteSpace(mapNameToLoad))
        {
            Debug.LogWarning("맵 이름을 입력해주세요!");
            return;
        }

        string selectedMapFilePath = Path.Combine(MapSavePath, mapNameToLoad + ".json");
        if (!File.Exists(selectedMapFilePath))
        {
            Debug.LogWarning($"'{mapNameToLoad}.json' 맵 파일을 찾을 수 없습니다.");
            return;
        }

        string jsonText = File.ReadAllText(selectedMapFilePath);
        MapDatas loadedMap = JsonUtility.FromJson<MapDatas>(jsonText);

        foreach (GameObject block in instantiatedEditorBlocks)
        {
            Destroy(block);
        }

        instantiatedEditorBlocks.Clear();
        currentBlocks.Clear();
        gridMap.Clear();

        currentMapData = loadedMap;
        mapNameInput.text = loadedMap.mapName;

        if (loadedMap.blocks != null)
        {
            foreach (var block in loadedMap.blocks)
            {
                if (block.blockID < 0 || block.blockID >= blockDataList.data.Length) continue;

                GameObject prefab = blockDataList.data[block.blockID].prefab;
                if (prefab == null) continue;

                Vector3 position = block.position.ToVector3();
                Quaternion rotation = Quaternion.Euler(block.rotation.ToVector3());

                GameObject instantiatedBlock = Instantiate(prefab, position, rotation);
                instantiatedEditorBlocks.Add(instantiatedBlock);
                currentBlocks.Add(block);
                gridMap.Add(new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y)));
            }
            isStartPositionSet = true;
        }
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
        currentBlocks.Clear();
        instantiatedEditorBlocks.Clear();
        isStartPositionSet = false;
        gridMap.Clear();
    }
}
