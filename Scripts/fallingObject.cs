using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallingObject : MonoBehaviour
{
    private Rigidbody2D rb;
    private HitBox hb;
    private Vector3 oldPos;

    private void Awake()
    {
        oldPos = new Vector3 (transform.position.x,transform.position.y,transform.position.z);
        rb = GetComponent<Rigidbody2D>();
        hb = GameObject.FindGameObjectWithTag("Hitbox").GetComponent<HitBox>();
    }

    private void Update()
    {  

        if (HitBox.isDead)
        {
            transform.position = oldPos;
            rb.gravityScale = 0;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Fireball"))
        {
            rb.gravityScale = 9.81f;
         
        }
    }

}
