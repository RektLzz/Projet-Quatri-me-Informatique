using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spring : MonoBehaviour
{
    public Rigidbody2D player;
    public GameObject ScriptHolder;
    private float timerCounter = 0.3f;
    private bool bouncy = false;
    private Vector3 pos;
    private Vector2 speed;
    [Range(0, 500f)] public float force;
    private float damp = 5f;
    PlayerMovement pl;
 

    private void Awake()
    {
        pos = gameObject.transform.position;

        pl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void Update()
    {
     
        if (bouncy)
        {
            timerCounter -= Time.deltaTime;

            pl.NoSpaceBarGravityMultiplier = pl.GravityMultiplier;

        }
       
        if (timerCounter < 0)
        {
            gameObject.transform.position = pos;

            ScriptHolder.GetComponent<PlayerMovement>().CanDash = true;

            ScriptHolder.GetComponent<PlayerMovement>().DashNumber = 1;

            bouncy = false;

            timerCounter = 0.3f;

          

        }
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            bouncy = true;

            ScriptHolder.GetComponent<PlayerMovement>().isDashing = false;

            ScriptHolder.GetComponent<PlayerMovement>().CanDash = false;

            pl.Jump(transform.up.x,transform.up.y,force);

            gameObject.transform.position = new Vector3(gameObject.transform.position.x-(transform.up.x/damp), gameObject.transform.position.y - (transform.up.y/damp), gameObject.transform.position.z);
   
        }
        
    }

}
