using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncPad : MonoBehaviour
{
    public float bounceForce = 30f; 

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (ballRb != null)
            {
 
                Vector2 normalForceDirection = collision.contacts[0].normal;
                ballRb.AddForce(normalForceDirection * bounceForce, ForceMode2D.Impulse);

         
            }
        }
    }

   
}
