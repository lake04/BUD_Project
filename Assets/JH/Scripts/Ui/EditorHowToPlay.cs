using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EditorHowToPlay : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private bool isPop = false;



    void Start()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPop == true)
            {
                PopDown();
            }
            else
            {
                PopUP();
            }
        }
    }
    

    public void ClickPop()
    {
        if (isPop == true)
        {
            PopDown();
        }
        else
        {
            PopUP();
        }
    }
    public void PopUP()
    {
        isPop = true;
        panel.SetActive(true);
    }

    public void PopDown()
    {
        isPop = false;
        panel.SetActive(false);
    }

}
