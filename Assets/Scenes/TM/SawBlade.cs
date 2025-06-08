using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : MonoBehaviour
{
    public float rotationSpeed = 50f;
    private void Start()
    {
        //나는너무행복합니다
    }
    void Update()
    {
        Vector3 rotation = transform.eulerAngles;
        rotation.z -= rotationSpeed * Time.deltaTime;
        transform.eulerAngles = rotation;
    }
}
