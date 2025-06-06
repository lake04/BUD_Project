using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorUI : MonoBehaviour
{
    public MapDatas currentMapData;
    public List<SaveBlockData> currentBlocks = new List<SaveBlockData>();

    public GameObject buttonPrefab;
    public InputField mapNameInput;
    public Transform buttonSpawnPrefab;
    public BlockDataList blockDataList;

    public SpriteRenderer pointerButton;

    Vector3 realPos;
    public int currentBlockIndex;

    void Start()
    {
        currentMapData = new MapDatas
        {
            mapName = "New Map",
            mapDesc = "This is a new map."
        };

        foreach (var blockData in blockDataList.data)
        {
            GameObject tempObj = Instantiate(buttonPrefab, buttonSpawnPrefab);
            tempObj.GetComponent<Button>().onClick.AddListener(() => OnSelectClick(blockData));
            tempObj.GetComponentInChildren<Image>().sprite = blockData.image;
            tempObj.GetComponentInChildren<Text>().text = blockData.name;
        }
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
        pointerButton.gameObject.transform.position = realPos;
    }

    public void OnSpawnButton()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject blockPrefab = Instantiate(blockDataList.data[currentBlockIndex].prefab, realPos, pointerButton.gameObject.transform.rotation);
            currentBlocks.Add(new SaveBlockData
            {
                blockID = currentBlockIndex,
                position = new Vector3Serial(realPos.x, realPos.y, 0),
                rotation = new Vector3Serial(pointerButton.transform.rotation.x, pointerButton.transform.rotation.y, pointerButton.transform.rotation.z)
            });
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            pointerButton.gameObject.transform.Rotate(0, 0, 90);
        }
    }

  

    public void OnSaveButton()
    {
        currentMapData.mapName = mapNameInput.text;
        currentMapData.blocks = currentBlocks.ToArray();
        Debug.Log($"Map Name: {currentMapData.mapName}, Map Desc: {currentMapData.mapDesc}");
        Debug.Log($"Total Blocks: {currentMapData.blocks.Length}");

        string jsonData = JsonUtility.ToJson(currentMapData);

        string folderPath = Path.Combine(Application.dataPath, "Resources", "Maps");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);


        string fileName = currentMapData.mapName + ".json";
        string path = Path.Combine(folderPath, fileName);
        File.WriteAllText(path, jsonData);
    }

    void OnSelectClick(BlockData blockData)
    {
        Debug.Log($"Click On : {blockData.name}");
        currentBlockIndex = blockDataList.data.ToList().IndexOf(blockData);
    }
}
