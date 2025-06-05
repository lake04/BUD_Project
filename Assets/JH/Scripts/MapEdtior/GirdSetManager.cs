using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirdSetManager : MonoBehaviour
{
    public SpriteRenderer gridSprite;
    public int width;
    public int height;


    void Start()
    {
        gridSprite = GetComponent<SpriteRenderer>();
        gridSprite.size = new Vector2(width, height);

        gridSprite.transform.position = new Vector3((width/2) - 0.5f, -(height/2) + 0.5f,0);
    }

    void Update()
    {
        
    }
}
