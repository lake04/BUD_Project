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
    public float m_roughness;      // 거칠기 정도
    public float m_magnitude;      // 움직임 범위
    public float m_rotation = 1f;       // 회전 강도 (카메라 쉐이크 강도)
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
    public void Shaking(float parametersShakeTime) // 0.35는 작은 쉐이크 and 0.7은 큰 쉐이크
    {
        shakeTime = parametersShakeTime;

        usingShake = true;
        StartCoroutine(Shake(shakeTime)); // 코루틴 실행
    }
    IEnumerator Shake(float duration)
    {
        float halfDuration = duration / 2;
        float elapsed = 0f;
        float tick = Random.Range(-1f, 1f); //5와 -5 사이의 랜덤 값 을 변수에 저장

        while (elapsed < duration) //elapsed가 duration보다 작을 동안 반복
        {
            elapsed += Time.deltaTime / halfDuration; //elapsed에 1프레임 동안 걸리는 시간을 더함 (+halfDuration으로 나누기)

            tick += Time.deltaTime * m_roughness; // tick에 1프레임 동안 걸리는 시간 곱하기 m_roughness 를 더함
            Vector3 position = transform.position; // 현재 위치를 포지션에 저장
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, tick * duration * m_rotation));
            position.y += (Mathf.PerlinNoise(tick * 0.5f, 0) - 0.5f) * m_magnitude * Mathf.PingPong(elapsed, halfDuration); // 부드럽게 이동
            transform.position = position; // 계산된 포지션으로 이동

            
            yield return null; // 중단
        }

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        usingShake = false;
    }
}
