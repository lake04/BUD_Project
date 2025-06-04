using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Turret : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float attackDistance;
    [SerializeField] private float coolTime;
    private float distance;
    private bool isAttck = true;
    private GameObject player; //플레이어로 바꿀거임
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform  firePoint;



    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
      
       distance = Vector2.Distance(this.transform.position, player.transform.position);
        
       if(distance < attackDistance && isAttck)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        isAttck = false;
        Vector2 dir = (player.transform.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<TurrntBullt>().Init(dir, damage);
        yield return new WaitForSeconds(coolTime);
        isAttck = true;
    }
}
