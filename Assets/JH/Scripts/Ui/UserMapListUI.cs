using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserMapListUI : MonoBehaviour
{
    public Transform contentParent; 
    public GameObject userMapItemPrefab;

    void Start()
    {
        LoadUserMapList();
    }

    void LoadUserMapList()
    {
        string userMapPath = Path.Combine(Application.persistentDataPath, "Maps", "User");

        if (!Directory.Exists(userMapPath))
        {
            Debug.LogWarning("���� �� ���� ����");
            return;
        }

        string[] mapFiles = Directory.GetFiles(userMapPath, "*.json");

        foreach (string path in mapFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            GameObject item = Instantiate(userMapItemPrefab, contentParent);

            // ��ư ã�� (PlayButton)
            Transform buttonTr = item.transform.Find("Canvas/PlayButton");
            if (buttonTr == null)
            {
                Debug.LogError("PlayButton�� ����!");
                return;
            }

            Button playBtn = buttonTr.GetComponent<Button>();
            if (playBtn == null)
            {
                Debug.LogError("Button ������Ʈ�� PlayButton�� ����!");
                return;
            }

            // �ؽ�Ʈ ����
            Transform textTr = buttonTr.Find("MapNameText");
            if (textTr == null)
            {
                Debug.LogError("MapNameText�� PlayButton �ȿ� ����!");
                return;
            }

            Text text = textTr.GetComponent<Text>();
            if (text == null)
            {
                Debug.LogError("Text ������Ʈ�� MapNameText�� ����!");
                return;
            }
            text.text = fileName;

            // �÷��� ��ư ����
            playBtn.onClick.AddListener(() =>
            {
                PlayerPrefs.SetString("selectedMap", fileName);
                MapManager.Instance.isEditorMode = false;
                SceneManager.LoadScene("PlayScene");
            });

        }
    }
}
