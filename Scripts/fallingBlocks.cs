using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallingBlocks : MonoBehaviour
{
    private bool contact = false;

    [SerializeField] private float timer;

    private float timerCounter;

    
    private void Start()
    {
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (HitBox.isDead)
            gameObject.SetActive(true);

        if (!contact)
        {
            timerCounter = timer;
        }
        else
        {
            timerCounter -= Time.deltaTime;
        }

        if (timerCounter < 0)
        {
            contact = false;
            gameObject.SetActive(false);

        }

    }
 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
            contact = true;
    }
}
