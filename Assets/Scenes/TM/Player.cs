using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spr;

    [Header("�̵�")]
    public float moveSpeed;
    public float maxMoveSpeed;
    public float maxFallingSpeed;
    private bool isSuperDown = false;
    private bool isMovingOk = true;

    [Header("������Ʈ,��ũ��Ʈ")]
    public Cam cam;
    public TrailRenderer trail;

    [Header("��ƼŬ")]
    public GameObject superDownParticle;
    public GameObject groundParticle;
    // Start is called before the first frame update
    void Start()
    {
        cam = Cam.camInstance.GetComponent<Cam>();
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame

    private void Update()
    {
        if (isMovingOk)
        {
            float moveInput = Input.GetAxis("Horizontal");
            rb.AddForce(new Vector2(moveInput * moveSpeed, 0));
            if (Mathf.Abs(rb.velocity.x) > maxMoveSpeed) rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxMoveSpeed, rb.velocity.y); //x�� ���ӵ� ����
        }

        if (Mathf.Abs(rb.velocity.y) > maxFallingSpeed) rb.velocity = new Vector2(rb.velocity.x, Mathf.Sign(rb.velocity.y) * maxFallingSpeed); //y�� ���ӵ� ����

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

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isSuperDown = false;
        if (rb.velocity.y > 13)
        {
            cam.Shaking(0.5f);
            GameObject Particle = Instantiate(groundParticle, transform.position, transform.rotation);
        }
    }
}
