using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncPad : MonoBehaviour
{
    public float bounceForce = 10000f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.rigidbody;

            if (rb != null)
            {
                //rb.velocity = new Vector2(rb.velocity.x, 0f);

                rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
            }
        }
    }
}






