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
            Debug.LogWarning("유저 맵 폴더 없음");
            return;
        }

        string[] mapFiles = Directory.GetFiles(userMapPath, "*.json");

        foreach (string path in mapFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            GameObject item = Instantiate(userMapItemPrefab, contentParent);

            // 버튼 찾기 (PlayButton)
            Transform buttonTr = item.transform.Find("Canvas/PlayButton");
            if (buttonTr == null)
            {
                Debug.LogError("PlayButton이 없음!");
                return;
            }

            Button playBtn = buttonTr.GetComponent<Button>();
            if (playBtn == null)
            {
                Debug.LogError("Button 컴포넌트가 PlayButton에 없음!");
                return;
            }

            // 텍스트 설정
            Transform textTr = buttonTr.Find("MapNameText");
            if (textTr == null)
            {
                Debug.LogError("MapNameText가 PlayButton 안에 없음!");
                return;
            }

            Text text = textTr.GetComponent<Text>();
            if (text == null)
            {
                Debug.LogError("Text 컴포넌트가 MapNameText에 없음!");
                return;
            }
            text.text = fileName;

            // 플레이 버튼 연결
            playBtn.onClick.AddListener(() =>
            {
                PlayerPrefs.SetString("selectedMap", fileName);
                MapManager.Instance.isEditorMode = false;
                SceneManager.LoadScene("PlayScene");
            });

        }
    }
}
