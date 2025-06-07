using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splinter : MonoBehaviour
{
  
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("GameOver");
            SceneController.Instance.TitleLoad();
        }
    }
    private void Attack()
    {

    }
}
