using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball_Switch : MonoBehaviour
{

    public GameObject Door;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fireball"))
        {
            Door.SetActive(false);
            gameObject.transform.localScale = new Vector3(transform.localScale.x - 0.5f, transform.localScale.y, transform.localScale.z);
        }
    }
}
