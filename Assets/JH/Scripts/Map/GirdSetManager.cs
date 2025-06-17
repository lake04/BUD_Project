using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirdSetManager : MonoBehaviour
{
    public SpriteRenderer gridSprite;
    public int width;
    public int height;

    public static GirdSetManager instance = null;
    

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        
    }

    private void Init()
    {
        gridSprite = GetComponent<SpriteRenderer>();
        gridSprite.size = new Vector2(width, height);
        this.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(width, height);

        gridSprite.transform.position = new Vector3((width / 2) - 7.0f, -(height / 2) + 7.0f, 0);
    }


}
