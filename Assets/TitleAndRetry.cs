using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.BoolParameter;

public class TitleAndRetry : MonoBehaviour
{
    float score = 0;
    public GameObject image;
    public Text scoreText;

    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        image.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            player = Player.playerInstance;
        }
        else
        {
            if (player.isDie)
            {
                score = -1*player.transform.position.y/10;
                Invoke("Result", 0.5f);
                //Result();
                scoreText.text = "SCORE : "+ Mathf.Floor(score * 100f) / 100f; ;
            }
        }
       
    }
    public void here()
    {
        Invoke("Result",0.5f);
        
    }
    void Result()
    {
        image.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Corou());
    }
    IEnumerator Corou()
    {
        Time.timeScale = 1f;
        new WaitForSeconds(0.3f);
        Time.timeScale = 0.5f;
        new WaitForSeconds(0.3f);
        Time.timeScale = 0;
        yield return 0;
    }
    public void Title()
    {
        SceneManager.LoadScene("Test");
        Time.timeScale = 1f;
    }
    public void Retry()
    {
        SceneManager.LoadScene("Test2");
        Time.timeScale = 1f;
    }
}
