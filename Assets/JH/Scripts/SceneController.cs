using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GameStart()
    {
        SceneLoad(1);
    }
    public void TitleLoad()
    {
        SceneLoad(0);
    }

    public void EdtiorLoad()
    {
        SceneLoad(2);
    }

    public void Quit()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.OpenURL("http://google.com");
        Application.Quit();
    }

    //씬 이동 번호로 할 생각 인데 이거 순서 맞추자
    //0 : 타이틀
    //1 : 인 게임
    //2 : 에디터
    public void SceneLoad(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }
}
