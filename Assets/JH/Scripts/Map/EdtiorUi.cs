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
        if (blockDataList == null) Debug.Log("��ũ���ͺ� ������Ʈ�� ����");

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
            Debug.Log("���콺 ��");
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
                Debug.Log("�̹� ���� �����մϴ�");
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
                Debug.Log("���� ��ġ��ġ");
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
                Debug.Log("�� �������� ����");
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
                Debug.Log("���� ��ġ ����.");
                isSettingStartPosition = false;
            }
            Debug.Log($"{hit.collider.name}");
            if (hit.collider.name == "Grid") return;
            instantiatedEditorBlocks.Remove(hit.collider.gameObject);
            Destroy(hit.collider.gameObject);
            currentBlocks.Remove(blockToRemove);
            gridMap.Remove(curPos);

            Debug.Log($"[{curPos}] ��ġ�� ��� ���� �Ϸ�");
        }

       
    }



    public void OnSaveButton()
    {
        currentMapData.mapName = mapNameInput.text;
        currentMapData.blocks = currentBlocks.ToArray();
        currentMapData.startPosition = this.startPos;

        if (currentMapData.startPosition == null)
        {
            Debug.LogError("���� �����Ϸ��� ���� ��ġ�� �����ؾ� �մϴ�!");
            return;
        }

        string jsonData = JsonUtility.ToJson(currentMapData, true);

        string folderPath = Path.Combine(Application.persistentDataPath, "Maps");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fileName = currentMapData.mapName + ".json";
        string path = Path.Combine(folderPath, fileName);
        File.WriteAllText(path, jsonData);

        Debug.Log("���� ���: " + path);
        SceneController.Instance.TitleLoad();
    }


    public void LoadMapButton()
    {
        // 1. mapNameInput �ʵ忡�� ����ڰ� �Է��� �� �̸��� �����ɴϴ�.
        string mapNameToLoad = mapNameInput.text;

        if (string.IsNullOrWhiteSpace(mapNameToLoad))
        {
            Debug.LogWarning("�� �̸��� �Է����ּ���!");
            return;
        }

        string mapsFolderPath = Path.Combine(Application.persistentDataPath, "Resources", "Maps");
        if (!Directory.Exists(mapsFolderPath))
        {
            Debug.LogWarning($"�� ���� ������ �������� �ʽ��ϴ�: {mapsFolderPath}");
            return;
        }

        // 2. �Էµ� �� �̸��� �ش��ϴ� JSON ���� ��θ� ����ϴ�.
        // ������ �� ".json" Ȯ���ڸ� �ٿ����Ƿ�, �ε��� ���� �ٿ���� �մϴ�.
        string selectedMapFilePath = Path.Combine(mapsFolderPath, mapNameToLoad + ".json");

        // 3. �ش� ������ ������ �����ϴ��� Ȯ���մϴ�.
        if (!File.Exists(selectedMapFilePath))
        {
            Debug.LogWarning($"'{mapNameToLoad}.json' �� ������ ã�� �� �����ϴ�. ���: {selectedMapFilePath}");
            return;
        }

        string jsonText = File.ReadAllText(selectedMapFilePath); // ���Ͽ��� JSON �ؽ�Ʈ �б�

        MapDatas loadedMap = JsonUtility.FromJson<MapDatas>(jsonText);
        Debug.Log($"�ҷ��� ��: {loadedMap.mapName} (���� ���: {selectedMapFilePath})");

        // --- ���⼭���� ���� ������ ����� ����� �� ���� �ε��ϴ� �ٽ� �����Դϴ�. ---

        // 1. ������ �����Ϳ� �ν��Ͻ�ȭ�� ��� ��� ���� ������Ʈ���� �ı��մϴ�.
        foreach (GameObject block in instantiatedEditorBlocks)
        {
            Destroy(block);
        }
        instantiatedEditorBlocks.Clear(); // ����Ʈ�� ����ݴϴ�.
        currentBlocks.Clear(); // ���� ��� ������ ����Ʈ�� ����ݴϴ�.

        // 2. �ҷ��� �� �����ͷ� currentMapData �� mapNameInput ������Ʈ
        currentMapData = loadedMap;
        mapNameInput.text = loadedMap.mapName; // ���� �ҷ��� ���� �̸����� InputField ������Ʈ

        // 3. �ҷ��� �� ������(loadedMap.blocks)�� �������� ���� ��ϵ��� ������ ���� �����մϴ�.
        if (loadedMap.blocks != null)
        {
            foreach (var block in loadedMap.blocks)
            {
                if (block.blockID < 0 || block.blockID >= blockDataList.data.Length)
                {
                    Debug.LogWarning($"��ȿ���� ���� ��� ID: {block.blockID}. �ش� ����� ��ŵ�˴ϴ�.");
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

                // ����� �ν��Ͻ�ȭ�ϰ� ���� ����Ʈ�� �߰�
                GameObject instantiatedBlock = Instantiate(prefab, position, rotation);
                instantiatedEditorBlocks.Add(instantiatedBlock);
                currentBlocks.Add(block); // currentBlocks���� �ٽ� �߰��Ͽ� ���� �� �ݿ��ǵ��� ��
            }
            Debug.Log($"'{loadedMap.mapName}' ���� �����Ϳ� ���������� �ҷ��Խ��ϴ�.");
        }
        else
        {
            Debug.LogWarning("�ҷ��� �� �����Ϳ� ��� ������ �����ϴ�.");
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
