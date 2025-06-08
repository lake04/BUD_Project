using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


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
    private bool isSettingStartPosition; 

    private List<GameObject> instantiatedEditorBlocks = new List<GameObject>();

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
    }

    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            MouseMove();
            OnSpawnButton();
        }

        CameraMovement();
    }

    public void CameraMovement()
    {
        Camera.main.transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * Time.deltaTime * 5f;
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
            RaycastHit2D hit = Physics2D.Raycast(realPos, Vector2.zero);
            if (hit.collider == null ||!hit.collider.CompareTag("Grid")  )
            {
                return;
            }
                foreach (SaveBlockData pos in currentBlocks)
                {
                    if (new Vector3(pos.position.x, pos.position.y, pos.position.z) == realPos)
                    {
                        return;
                    }
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

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(realPos, Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("Block"))
            {
                Vector3 pos = hit.collider.transform.position;
                instantiatedEditorBlocks.Remove(hit.collider.gameObject);
                Destroy(hit.collider.gameObject);

                currentBlocks.RemoveAll(b =>
                    Mathf.Approximately(b.position.x, pos.x) &&
                    Mathf.Approximately(b.position.y, pos.y)
                );

                Debug.Log($"{pos} ��� ���� ");
            }
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            pointerButton.gameObject.transform.Rotate(0, 0, 45);
        }
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
}
