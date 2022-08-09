using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Terrain : MonoBehaviour
{

    // Start is called before the first frame update
    public void Awake()
    {
        TilemapCollider2D t;
        t = GetComponent<TilemapCollider2D>();
        Debug.Log(t);

    }
}

 
