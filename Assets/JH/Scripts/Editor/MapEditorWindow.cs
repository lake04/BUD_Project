using UnityEditor;
using UnityEngine;
using System.IO;

public class MapEditorWindow : EditorWindow
{
    private int width = 10;
    private int height = 20;
    private TileType[,] mapData;

    [MenuItem("Tools/�� ������")]
    public static void ShowWindow()
    {
        GetWindow<MapEditorWindow>("�� ������");
    }

    private void OnEnable()
    {
        mapData = new TileType[width, height];
    }

    private void OnGUI()
    {
        GUILayout.Label("�� ������ ��", EditorStyles.boldLabel);
        GUILayout.Label("Ŭ���Ͽ� ��ֹ� ��ġ (��ȯ��)");

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

        if (GUILayout.Button("�� �����ϱ�"))
        {
            SaveMap();
        }

        if (GUILayout.Button("�� �ҷ�����"))
        {
            LoadMap();
        }
    }

    private void SaveMap()
    {
        string fileName = EditorUtility.SaveFilePanel("�� ����", "Assets/Resources", "MapChunk1", "json");
        if (string.IsNullOrEmpty(fileName)) return;

        if (!fileName.Contains("/Resources/"))
        {
            EditorUtility.DisplayDialog("����", "�ݵ�� Assets/Resources ���� �ȿ� �����ؾ� �մϴ�.", "Ȯ��");
            return;
        }

        string json = JsonUtility.ToJson(new SerializableMap(mapData), true);
        File.WriteAllText(fileName, json);

        Debug.Log("���� ����Ǿ����ϴ�: " + fileName);
    }


    private void LoadMap()
    {
        string filePath = EditorUtility.OpenFilePanel("�� �ҷ�����", "Assets/Resources", "json");
        if (string.IsNullOrEmpty(filePath)) return;

        if (!filePath.Contains("/Resources/"))
        {
            EditorUtility.DisplayDialog("����", "�ݵ�� Resources ���� ���� ������ �����ؾ� �մϴ�.", "Ȯ��");
            return;
        }

        string json = File.ReadAllText(filePath);
        SerializableMap loaded = JsonUtility.FromJson<SerializableMap>(json);
        mapData = loaded.To2DArray();
        width = loaded.width;
        height = loaded.height;

        Debug.Log("�� �ҷ����� �Ϸ�: " + filePath);
    }

}
