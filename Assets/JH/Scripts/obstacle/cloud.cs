using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloud : MonoBehaviour
{
    [SerializeField] private GameObject effect;

    private bool isSlowing = false;

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
            if(isSlowing == false)
            {
                Debug.Log("이속 저하");
                StartCoroutine(SlowDown(collision));

            }
        }
    }

    private IEnumerator SlowDown(Collider2D collision)
    {
        if(isSlowing == false)
        {
            collision.GetComponent<Player>().moveSpeed -= 0.6f;
            isSlowing = true;
        }
        yield return new WaitForSeconds(0.4f);
        isSlowing = true;
        collision.GetComponent <Player>().moveSpeed += 0.6f;
    }
}
