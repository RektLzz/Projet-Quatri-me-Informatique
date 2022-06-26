using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

 class rigi : MonoBehaviour
{
    public Rigidbody mySquare;
    public static  BoxCollider2D coll;
    SpriteRenderer sprite;
    public static Rigidbody2D square;
    [SerializeField] public LayerMask jumpableGround;
    // Start is called before the first frame update
    public void Start()
        
    {
        sprite = GetComponent<SpriteRenderer>();
        square = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite.color = new Color (1,0.3f,0.9f,1);
    }

    // Update is called once per frame
    public void Update()
    {
        float dirX = Input.GetAxisRaw("Horizontal");
        square.velocity = new Vector2(dirX* 7f, square.velocity.y);

        if(Input.GetButtonDown("Jump") && isGrounded())
        {
            square.velocity = new Vector2(square.velocity.x, 7f);
        }
     if(isDead()){
           SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex *0);
     }

    }
    public bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    public bool isDead()
    {
        return square.position.y <= -10;
    }
}