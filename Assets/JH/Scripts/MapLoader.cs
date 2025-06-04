using UnityEngine;
using System.IO;

public class MapLoader : MonoBehaviour
{
    public string mapFileName = "MapChunk1.json"; // Resources 폴더 안 파일
    public GameObject bkPrefab;
    public GameObject wallPrefab;
    public GameObject spikePrefab;
    public GameObject padPrefab;
    public GameObject turretPrefab;

    void Start()
    {
        LoadMap(mapFileName);
    }

    public void LoadMap(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("MapChunk1");

        if (jsonFile == null)
        {
            Debug.LogError("맵 파일을 찾을 수 없습니다!");
            return;
        }

        SerializableMap map = JsonUtility.FromJson<SerializableMap>(jsonFile.text);

        TileType[,] grid = map.To2DArray();

        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                TileType tile = grid[x, y];
                Vector3 pos = new Vector3(x, -y, 0); 

                GameObject prefab = GetPrefab(tile);
                if (prefab != null)
                {
                    Instantiate(prefab, pos, Quaternion.identity);
                }
            }
        }
    }

    private GameObject GetPrefab(TileType type)
    {
        switch (type)
        {
            case TileType.Wall: return wallPrefab;
            case TileType.Spike: return spikePrefab;
            case TileType.BouncePad: return padPrefab;
            case TileType.Turrnt: return turretPrefab;
            default: return null;
        }
    }
}
