using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spr;


    public bool isDie = false;
    public bool isSucc = false;

    [Header("Singleton")]
    public static Player playerInstance;

    [Header("물리")]
    //public PhysicsMaterial2D pysic;

    [Header("이동")]
    public float moveSpeed;
    public float maxMoveSpeed;
    public float maxFallingSpeed;
    private bool isSuperDown = false;
    private bool isMovingOk = true;

    [Header("오브젝트,스크립트")]
    public Cam cam;
    public TrailRenderer trail;

    //public Portal portal;

    [Header("파티클")]
    public GameObject superDownParticle;
    public GameObject groundParticle;

    [Header("FMOD")]
    public EventReference playerBounce1;
    public EventReference playerBounce2;
    public EventReference playerBounce3;
    // Start is called before the first frame update

    void Start()
    {
        if (playerInstance == null)
        {
            playerInstance = this;
        }
        cam = Cam.camInstance.GetComponent<Cam>();

        //portal = Portal.portalInstance.GetComponent<Portal>();

        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame

    private void Update()
    {
        if (Input.GetKey(KeyCode.F11) && Input.GetKey(KeyCode.T)) isDie = true;

        if (isDie)
        {
            GameObject Particle = Instantiate(groundParticle, transform.position, transform.rotation);
            Destroy(gameObject, 0.3f);
            gameObject.SetActive(false);
        }
        else
        {
            if (isMovingOk)
            {
                float moveInput = Input.GetAxis("Horizontal");
                rb.AddForce(new Vector2(moveInput * moveSpeed, 0));
                if (Mathf.Abs(rb.velocity.x) > maxMoveSpeed) rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxMoveSpeed, rb.velocity.y); //x축 가속도 제한
            }

            if (Mathf.Abs(rb.velocity.y) > maxFallingSpeed) rb.velocity = new Vector2(rb.velocity.x, Mathf.Sign(rb.velocity.y) * maxFallingSpeed); //y축 가속도 제한

            if (Input.GetKeyDown(KeyCode.Space))
            {
                cam.Shaking(0.7f);
                GameObject Particle = Instantiate(superDownParticle, transform.position, transform.rotation);
                isSuperDown = true;
                isMovingOk = false;
                rb.velocity = Vector2.zero;
                rb.gravityScale = 100;
                spr.color = Color.red;
                trail.startColor = Color.red;
            }
            if (isSuperDown == false)
            {
                isMovingOk = true;
                rb.gravityScale = 5;
                spr.color = Color.cyan;
                trail.startColor = Color.cyan;
            }

        }
        
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isSuperDown = false;

        int randSound = Random.Range(1, 3);
        if (randSound == 1)
        {
            RuntimeManager.CreateInstance(playerBounce1).start();
        }
        else if(randSound == 2)
        {
            RuntimeManager.CreateInstance(playerBounce2).start();
        }
        else if (randSound == 3)
        {
            RuntimeManager.CreateInstance(playerBounce3).start();
        }
        if (collision.gameObject.CompareTag("Block"))
        {
            if (rb.velocity.y > 33)
            {
                cam.Shaking(0.5f);
                Debug.Log("bigShake");
                GameObject Particle1 = Instantiate(groundParticle, transform.position, transform.rotation);
                GameObject Particle = Instantiate(groundParticle, transform.position, transform.rotation);
                //pysic.bounciness = 0.9f;
            }
            else if (rb.velocity.y > 30)
            {
                cam.Shaking(0.2f);
                Debug.Log("Shake");
                GameObject Particle = Instantiate(groundParticle, transform.position, transform.rotation);
                //pysic.bounciness = 0.9f;
            }
            else
            {
                //pysic.bounciness = 0;
            }
        }
        if (collision.gameObject.CompareTag("Spike"))
        {
            Destroy(gameObject);
            GameObject Particle = Instantiate(groundParticle, transform.position, transform.rotation);
        }
        //rb.sharedMaterial = pysic;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDie==false)
        {
            if (collision.gameObject.CompareTag("Portal1"))
            {
                //transform.position = portal.portal2.position;
            }
        }
        if (collision.gameObject.CompareTag("End"))
        {
            isSucc = true;
            Destroy(gameObject, 0.3f);
            gameObject.SetActive(false);
        }
    }
}
