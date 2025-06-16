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

    public Vector3Serial startPos = null;
    private bool isSettingStartPosition = false;

    private List<GameObject> instantiatedEditorBlocks = new List<GameObject>();

    public HashSet<Vector2Int> gridMap;

    private float camaerMoveSpeed = 5;
    [SerializeField] private TMP_Text description;

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
        description.text = blockDataList.data[0].description;
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
        CmaeraWheels();
    }


    private void Init()
    {
        currentMapData = new MapDatas
        {
            mapName = "New Map",
            mapDesc = "This is a new map."
        };

        pointerButton = Instantiate(pointerButton);

        foreach (var blockData in blockDataList.data)
        {
            GameObject tempObj = Instantiate(buttonPrefab, buttonSpawnPrefab);
            tempObj.GetComponent<Button>().onClick.AddListener(() => OnSelectClick(blockData));
            tempObj.GetComponentInChildren<Image>().sprite = blockData.image;
            tempObj.GetComponentInChildren<Text>().text = blockData.name;
        }
        if (blockDataList == null) Debug.Log("스크립터블 오브젝트가 없음");

        gridMap = new HashSet<Vector2Int>();
    }

    public void CameraMovement()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            camaerMoveSpeed = 10f;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            camaerMoveSpeed = 5f;
        }
        Camera.main.transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * Time.deltaTime * camaerMoveSpeed;
    }

    private void CmaeraWheels()
    {
        if(Input.GetKey(KeyCode.Mouse2))
        {
            Debug.Log("마우스 휠");
        }
    }

    public void MouseMove()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        realPos = new Vector3(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y), 0);
        pointerButton.transform.position = realPos;
    }

    public void OnSpawnButton()
    {
        RaycastHit2D hit = Physics2D.Raycast(realPos, Vector2.zero);
        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int curPos = new Vector2Int(Mathf.RoundToInt(realPos.x)
               , Mathf.RoundToInt(realPos.y));

            if (hit.collider == null)
            {
                return;
            }

            if (gridMap.Contains(curPos) == true)
            {
                Debug.Log("이미 블럭이 존재합니다");
                return;

            }
            gridMap.Add(curPos);
            foreach (SaveBlockData pos in currentBlocks)
            {
                if (new Vector3(pos.position.x, pos.position.y, pos.position.z) == realPos)
                {
                    return;
                }
            }

            if (currentBlockIndex == blockDataList.data.Length - 1)
            {
                if (isSettingStartPosition == true)
                {
                    return;

                }
                Debug.Log("시작 위치설치");
                startPos = new Vector3Serial(realPos.x, realPos.y, 0);
                isSettingStartPosition = true;
            }

            GameObject blockPrefab = Instantiate(

            blockDataList.data[currentBlockIndex].prefab,
            realPos,
            pointerButton.transform.rotation
             );

            instantiatedEditorBlocks.Add(blockPrefab);

            currentBlocks.Add(new SaveBlockData
            {
                blockID = currentBlockIndex,
                position = new Vector3Serial(realPos.x, realPos.y, 0),
                rotation = new Vector3Serial(
                   pointerButton.transform.rotation.eulerAngles.x,
                   pointerButton.transform.rotation.eulerAngles.y,
                   pointerButton.transform.rotation.eulerAngles.z
                 )
            });


        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            pointerButton.gameObject.transform.Rotate(0, 0, 45);
        }
    }

    private void RemoveClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(realPos, Vector2.zero);

            if (hit.collider == null) return;

            Vector2Int curPos = new Vector2Int(
                Mathf.RoundToInt(realPos.x),
                Mathf.RoundToInt(realPos.y)
            );

            if (gridMap.Contains(curPos) == false)
            {
                Debug.Log("블럭 존재하지 않음");
                return;

            }

            Vector3 pos = hit.collider.transform.position;

            SaveBlockData blockToRemove = currentBlocks.FirstOrDefault(b =>
                Mathf.Approximately(b.position.x, pos.x) &&
                Mathf.Approximately(b.position.y, pos.y)
            );

            if (blockToRemove != null &&
                blockToRemove.blockID == blockDataList.data.Length - 1)
            {
                Debug.Log("시작 위치 삭제.");
                isSettingStartPosition = false;
            }
            Debug.Log($"{hit.collider.name}");
            if (hit.collider.name == "Grid") return;
            instantiatedEditorBlocks.Remove(hit.collider.gameObject);
            Destroy(hit.collider.gameObject);
            currentBlocks.Remove(blockToRemove);
            gridMap.Remove(curPos);

            Debug.Log($"[{curPos}] 위치의 블록 삭제 완료");
        }

       
    }



    public void OnSaveButton()
    {
        currentMapData.mapName = mapNameInput.text;
        currentMapData.blocks = currentBlocks.ToArray();
        currentMapData.startPosition = this.startPos;

        if (currentMapData.startPosition == null)
        {
            Debug.LogError("맵을 저장하려면 시작 위치를 설정해야 합니다!");
            return;
        }

        string jsonData = JsonUtility.ToJson(currentMapData, true);

        string folderPath = Path.Combine(Application.persistentDataPath, "Maps");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fileName = currentMapData.mapName + ".json";
        string path = Path.Combine(folderPath, fileName);
        File.WriteAllText(path, jsonData);

        Debug.Log("저장 경로: " + path);
        SceneController.Instance.TitleLoad();
    }


    public void LoadMapButton()
    {
        // 1. mapNameInput 필드에서 사용자가 입력한 맵 이름을 가져옵니다.
        string mapNameToLoad = mapNameInput.text;

        if (string.IsNullOrWhiteSpace(mapNameToLoad))
        {
            Debug.LogWarning("맵 이름을 입력해주세요!");
            return;
        }

        string mapsFolderPath = Path.Combine(Application.persistentDataPath, "Resources", "Maps");
        if (!Directory.Exists(mapsFolderPath))
        {
            Debug.LogWarning($"맵 파일 폴더가 존재하지 않습니다: {mapsFolderPath}");
            return;
        }

        // 2. 입력된 맵 이름에 해당하는 JSON 파일 경로를 만듭니다.
        // 저장할 때 ".json" 확장자를 붙였으므로, 로드할 때도 붙여줘야 합니다.
        string selectedMapFilePath = Path.Combine(mapsFolderPath, mapNameToLoad + ".json");

        // 3. 해당 파일이 실제로 존재하는지 확인합니다.
        if (!File.Exists(selectedMapFilePath))
        {
            Debug.LogWarning($"'{mapNameToLoad}.json' 맵 파일을 찾을 수 없습니다. 경로: {selectedMapFilePath}");
            return;
        }

        string jsonText = File.ReadAllText(selectedMapFilePath); // 파일에서 JSON 텍스트 읽기

        MapDatas loadedMap = JsonUtility.FromJson<MapDatas>(jsonText);
        Debug.Log($"불러온 맵: {loadedMap.mapName} (파일 경로: {selectedMapFilePath})");

        // --- 여기서부터 기존 에디터 블록을 지우고 새 맵을 로드하는 핵심 로직입니다. ---

        // 1. 기존에 에디터에 인스턴스화된 모든 블록 게임 오브젝트들을 파괴합니다.
        foreach (GameObject block in instantiatedEditorBlocks)
        {
            Destroy(block);
        }
        instantiatedEditorBlocks.Clear(); // 리스트도 비워줍니다.
        currentBlocks.Clear(); // 현재 블록 데이터 리스트도 비워줍니다.

        // 2. 불러온 맵 데이터로 currentMapData 및 mapNameInput 업데이트
        currentMapData = loadedMap;
        mapNameInput.text = loadedMap.mapName; // 실제 불러온 맵의 이름으로 InputField 업데이트

        // 3. 불러온 맵 데이터(loadedMap.blocks)를 바탕으로 실제 블록들을 에디터 씬에 생성합니다.
        if (loadedMap.blocks != null)
        {
            foreach (var block in loadedMap.blocks)
            {
                if (block.blockID < 0 || block.blockID >= blockDataList.data.Length)
                {
                    Debug.LogWarning($"유효하지 않은 블록 ID: {block.blockID}. 해당 블록은 스킵됩니다.");
                    continue;
                }

                GameObject prefab = blockDataList.data[block.blockID].prefab;
                if (prefab == null)
                {
                    Debug.LogWarning($"블록 ID {block.blockID}에 해당하는 프리팹이 BlockDataList에 할당되지 않았습니다.");
                    continue;
                }

                Vector3 position = block.position.ToVector3();
                Quaternion rotation = Quaternion.Euler(block.rotation.ToVector3());

                // 블록을 인스턴스화하고 추적 리스트에 추가
                GameObject instantiatedBlock = Instantiate(prefab, position, rotation);
                instantiatedEditorBlocks.Add(instantiatedBlock);
                currentBlocks.Add(block); // currentBlocks에도 다시 추가하여 저장 시 반영되도록 함
            }
            Debug.Log($"'{loadedMap.mapName}' 맵을 에디터에 성공적으로 불러왔습니다.");
        }
        else
        {
            Debug.LogWarning("불러온 맵 데이터에 블록 정보가 없습니다.");
        }
    }



    void OnSelectClick(BlockData blockData)
    {
        Debug.Log($"Click On : {blockData.name}");
        currentBlockIndex = blockDataList.data.ToList().IndexOf(blockData);
        pointerButton.GetComponent<SpriteRenderer>().sprite = blockData.image;
    }

    public void OnClearClick()
    {
        currentBlocks.Clear();
        instantiatedEditorBlocks.Clear();
        isSettingStartPosition = false;
        gridMap.Clear();
    }
}
