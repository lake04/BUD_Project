using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Singleton")]
    public static Portal portalInstance;

    [Header("Portal")]
    public Transform portal1;
    public Transform portal2;

    private void Awake()
    {
        if (portalInstance == null)
        {
            portalInstance = this;
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
