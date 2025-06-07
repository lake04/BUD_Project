using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowObstacle : MonoBehaviour
{
    public bool isSlowing = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isSlowing == false)
            {
                Debug.Log("�̼� ����");
                StartCoroutine(SlowDown(collision));
            }
        }
    }

    private IEnumerator SlowDown(Collider2D collision)
    {
        if (isSlowing == false)
        {
            collision.GetComponent<Player>().moveSpeed -= 0.6f;
            isSlowing = true;
        }
        yield return new WaitForSeconds(1f);
        isSlowing = false;
        collision.GetComponent<Player>().moveSpeed += 0.6f;
    }
}
