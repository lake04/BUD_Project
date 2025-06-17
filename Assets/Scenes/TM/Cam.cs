using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomCam
{
    public Transform room;
    public float camSize = 35;
}

public class Cam : MonoBehaviour
{
    public float cameraSpeed = 5.0f;
    private bool isPlayerGet = false;

    [Header("Singleton")]
    public static Cam camInstance;


    [Header("Object Or Script")]
    public Camera mainCamera;
    public GameObject playerObj;
    public Player player;

    [Header("Shake")]
    public bool usingShake = false;
    public float m_roughness;      // ��ĥ�� ����
    public float m_magnitude;      // ������ ����
    public float m_rotation = 1f;       // ȸ�� ���� (ī�޶� ����ũ ����)
    public float shakeTime = 0.3f;


    private void Awake()
    {
        if (camInstance == null)
        {
            camInstance = this;
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    private void Update()
    {
        if(playerObj == null && isPlayerGet==false)
        {
            playerObj = FindObjectOfType<Player>().gameObject;
            isPlayerGet = true;
        }
        else
        {
            return;
        }
           
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerObj != null)
        {
            if (usingShake == false)
            {
                Vector3 dir;
                Vector3 moveVector;


                dir = (playerObj.transform.position + playerObj.transform.position) / 2 - this.transform.position;
                moveVector = new Vector3(dir.x * cameraSpeed * Time.deltaTime, dir.y * cameraSpeed * Time.deltaTime, 0.0f);
                this.transform.Translate(moveVector);
            }
        }
    }
    public void Shaking(float parametersShakeTime) // 0.35�� ���� ����ũ and 0.7�� ū ����ũ
    {
        shakeTime = parametersShakeTime;

        usingShake = true;
        StartCoroutine(Shake(shakeTime)); // �ڷ�ƾ ����
    }
    IEnumerator Shake(float duration)
    {
        float halfDuration = duration / 2;
        float elapsed = 0f;
        float tick = Random.Range(-1f, 1f); //5�� -5 ������ ���� �� �� ������ ����

        while (elapsed < duration) //elapsed�� duration���� ���� ���� �ݺ�
        {
            elapsed += Time.deltaTime / halfDuration; //elapsed�� 1������ ���� �ɸ��� �ð��� ���� (+halfDuration���� ������)

            tick += Time.deltaTime * m_roughness; // tick�� 1������ ���� �ɸ��� �ð� ���ϱ� m_roughness �� ����
            Vector3 position = transform.position; // ���� ��ġ�� �����ǿ� ����
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, tick * duration * m_rotation));
            position.y += (Mathf.PerlinNoise(tick * 0.5f, 0) - 0.5f) * m_magnitude * Mathf.PingPong(elapsed, halfDuration); // �ε巴�� �̵�
            transform.position = position; // ���� ���������� �̵�

            
            yield return null; // �ߴ�
        }

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        usingShake = false;
    }
}
