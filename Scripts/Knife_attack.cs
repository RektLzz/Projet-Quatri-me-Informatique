using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife_attack : MonoBehaviour
{
    public Material def;
    public Material mat;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float attackCooldown;
    private float cooldownTimer = Mathf.Infinity;
    public static bool canSlash = false;

    private void Awake()
    { 
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {

        if (canSlash)
        {
            if (Input.GetButtonDown("Y") && cooldownTimer > attackCooldown)
            {

                anim.SetTrigger("Slash1");
                spriteRenderer.material = mat;
                cooldownTimer = 0;

                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                {
                    anim.SetTrigger("Slash2");
                    spriteRenderer.material = mat;
                    cooldownTimer = 0;


                }
            }
            else
            {
                cooldownTimer += Time.deltaTime;

                if (cooldownTimer > attackCooldown)
                    spriteRenderer.material = def;
            }
        }
    }
}
