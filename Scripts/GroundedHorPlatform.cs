using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedHorPlatform : MonoBehaviour
{

    private bool contact;

    [SerializeField] private float velocity;

    private HitBox hb;

    private Vector3 oldPos;

    private bool stop;

    private PlayerMovement pl;

    private void Awake()
    {
        contact = false;

        hb = GameObject.FindGameObjectWithTag("Hitbox").GetComponent<HitBox>();

        oldPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        stop = false;

        pl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void FixedUpdate()
    {

        if (HitBox.isDead)
        { 
            transform.position = oldPos;
            contact = false;
            stop = false;
        }

        if (contact)
            transform.position = new Vector3(transform.position.x + (velocity * Time.fixedDeltaTime), transform.position.y, transform.position.z);

    }

    

    private void OnCollisionEnter2D(Collision2D collision)
    {

        for (int i = 0; i < collision.contacts.Length; i++)
        {

            if (!collision.collider.CompareTag("Ground"))
            {
                Debug.Log(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isTouchingGround);

                if (collision.collider.CompareTag("Player") && pl.isTouchingGround && !stop)
                {

                    contact = true;

                    collision.collider.transform.SetParent(transform);
                }

             
            }
            else
            {
                contact = false;

                stop = true;
            }
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(null);
        }      
      
    }

  


}
