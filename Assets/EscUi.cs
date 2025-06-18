using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscUi : MonoBehaviour
{
    public GameObject image;
    private bool isOn = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }
    }
    public void Back()
    {
        isOn = !isOn;
        image.SetActive(isOn);
    }
}
