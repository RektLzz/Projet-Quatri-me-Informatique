using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dashRefill : MonoBehaviour
{
    public GameObject ScriptHolder;
    public ParticleSystem particles;
    public float Timer = 4f;
    private bool hit;
    private SpriteRenderer sr;

    private void Awake()
    {
        hit = false;
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (hit)
        {
            Timer -= Time.deltaTime;


        }
        else
        {
            Timer = 4f;

            sr.color = new Color(0, 1, 0);
        }

        if(Timer < 0)
        {
   
           
            hit = false;

        }
    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hit)
        { 

            particles.transform.position = gameObject.transform.position;

            particles.Play();

            hit = true;

            ScriptHolder.GetComponent<PlayerMovement>().DashNumber = 1;

            ScriptHolder.GetComponent<PlayerMovement>().CanDash = true;

            sr.color = new Color(1, 0, 0);

        }

    }

}
