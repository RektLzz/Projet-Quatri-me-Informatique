using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifePickUp : MonoBehaviour
{
    private void Awake()
    {
        if (Knife_attack.canSlash)
            Destroy(gameObject);    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Knife_attack.canSlash = true;
        Destroy(gameObject);  
    }
}
