using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinDetection : MonoBehaviour
{
    private HitBox hb;

    private SpriteRenderer sp;

    [SerializeField] GameObject coin;

    private Color color;

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();    

        color = sp.color;

        hb = GameObject.FindGameObjectWithTag("Hitbox").GetComponent<HitBox>();
    }
    private void Update()
    {
        
        if (HitBox.isDead)
        {
            tag = "Object";
            sp.color = color;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        tag = "hit";
        sp.color = Color.clear;
    }
}
