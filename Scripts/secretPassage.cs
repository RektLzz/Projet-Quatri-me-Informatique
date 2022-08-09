using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class secretPassage : MonoBehaviour
{

    private SpriteRenderer sp;
    private Color color;

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        color = sp.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            sp.color = new Color(1, 1, 1, 0.2f);
            
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            sp.color = color;
    }
}
