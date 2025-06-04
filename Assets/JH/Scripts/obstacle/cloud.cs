using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloud : MonoBehaviour
{
    [SerializeField] private GameObject effect;


    void Start()
    {
        effect.SetActive(true);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //이거는 태민이가 플레이어 작업 하면 추가함
        }
    }
}
