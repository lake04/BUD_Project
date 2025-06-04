using UnityEditor;
using UnityEngine;
using System.IO;

public class MapEditorWindow : EditorWindow
{
    private int width = 10;
    private int height = 20;
    private TileType[,] mapData;

    [MenuItem("Tools/맵 에디터")]
    public static void ShowWindow()
    {
        GetWindow<MapEditorWindow>("맵 에디터");
    }

    private void OnEnable()
    {
        mapData = new TileType[width, height];
    }

    private void OnGUI()
    {
        GUILayout.Label("맵 에디터 툴", EditorStyles.boldLabel);
        GUILayout.Label("클릭하여 장애물 배치 (순환됨)");

        for (int y = 0; y < height; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < width; x++)
            {
                TileType current = mapData[x, y];
                string label = current == TileType.None ? "." : current.ToString().Substring(0, 1);
                if (GUILayout.Button(label, GUILayout.Width(25), GUILayout.Height(25)))
                {
                    mapData[x, y] = (TileType)(((int)current + 1) % System.Enum.GetValues(typeof(TileType)).Length);
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("맵 저장하기"))
        {
            SaveMap();
        }

        if (GUILayout.Button("맵 불러오기"))
        {
            LoadMap();
        }
    }

    private void SaveMap()
    {
        string fileName = EditorUtility.SaveFilePanel("맵 저장", "Assets/Resources", "MapChunk1", "json");
        if (string.IsNullOrEmpty(fileName)) return;

        if (!fileName.Contains("/Resources/"))
        {
            EditorUtility.DisplayDialog("오류", "반드시 Assets/Resources 폴더 안에 저장해야 합니다.", "확인");
            return;
        }

        string json = JsonUtility.ToJson(new SerializableMap(mapData), true);
        File.WriteAllText(fileName, json);

        Debug.Log("맵이 저장되었습니다: " + fileName);
    }


    private void LoadMap()
    {
        string filePath = EditorUtility.OpenFilePanel("맵 불러오기", "Assets/Resources", "json");
        if (string.IsNullOrEmpty(filePath)) return;

        if (!filePath.Contains("/Resources/"))
        {
            EditorUtility.DisplayDialog("오류", "반드시 Resources 폴더 내의 파일을 선택해야 합니다.", "확인");
            return;
        }

        string json = File.ReadAllText(filePath);
        SerializableMap loaded = JsonUtility.FromJson<SerializableMap>(json);
        mapData = loaded.To2DArray();
        width = loaded.width;
        height = loaded.height;

        Debug.Log("맵 불러오기 완료: " + filePath);
    }

}
