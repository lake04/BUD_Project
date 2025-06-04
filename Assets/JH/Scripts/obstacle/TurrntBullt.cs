using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurrntBullt : MonoBehaviour
{
    private Vector2 direction;
    private float speed = 10f;
    private float damage;

    public void Init(Vector2 dir, float dmg)
    {
        direction = dir;
        damage = dmg;
        VDestroy(5f); 
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("АјАн");
            VDestroy(0f);
        }
    }

    private void VDestroy(float time)
    {
        Destroy(gameObject, time);
    }
}
