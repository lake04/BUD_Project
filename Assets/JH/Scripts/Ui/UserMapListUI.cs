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

        foreach (FileInfo file in di.GetFiles("*.cs"))
        {
            GameObject go = Instantiate(userMapItemPrefab,pos.transform);
            go.GetComponentInChildren<Text>().text = file.Name;
            Debug.Log("파일명 : " + file.Name);
        }
       
    }
}
