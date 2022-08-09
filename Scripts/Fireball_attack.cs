using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball_attack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoints;
    [SerializeField] private GameObject[] fireballs;

    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("B") && cooldownTimer > attackCooldown)
            Shoot();

        cooldownTimer += Time.deltaTime;
        
    }
    private void Shoot()
    {
        //anim.SetTrigger("Fireball");
        cooldownTimer = 0;

        fireballs[findFireball()].transform.position = firePoints.position;
        fireballs[findFireball()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private int findFireball()
    {
        for(int i = 0; i< fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return i;      
        }
        return 0;
    }
}
