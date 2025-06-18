using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserMapListUI : MonoBehaviour
{
    public Transform contentParent; 
    public GameObject userMapItemPrefab;
    public GameObject pos;

    void Start()
    {
        LoadUserMapList();
    }

    void LoadUserMapList()
    {
        string userMapPath = Path.Combine(Application.dataPath, "Maps", "User");

        if (!Directory.Exists(userMapPath))
        {
            Debug.LogWarning("유저 맵 폴더 없음");
            return;
        }
        System.IO.DirectoryInfo di = new DirectoryInfo(userMapPath);

        foreach (FileInfo file in di.GetFiles("*.json"))
        {
            string fileName = file.Name; 

            GameObject go = Instantiate(userMapItemPrefab, pos.transform);
            go.GetComponentInChildren<Text>().text = fileName;

            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                PlayerPrefs.SetString("selectedMap", fileName);
                MapManager.Instance.isEditorMode = false;
                SceneManager.LoadScene(1);
                MapManager.Instance.LoadUserMap(fileName);
                MapManager.Instance.isCustom = true;
            });

            Debug.Log("파일명 : " + fileName);
        }

    }
}
