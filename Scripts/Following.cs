using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Following : MonoBehaviour
{
    public static bool isFollowing = false;
    public Transform target;
    public float smoothTime = 0.3f;
    private Transform thisTransform;
    private Vector2 velocity;
    public float yOffset = 0.7f;
    private float direction;

    public bool useSmoothing = false;

    private void Awake()
    {   
        // only works when you lauch the game  

        if(!isFollowing)
        isFollowing = false;
        
    }
    void Start()
    {
        direction = transform.localScale.x;
        thisTransform = transform;
        velocity = new Vector2(0.5f, 0.5f);
        
    }

    void Update()
    {
    
        if (isFollowing)
        {
            Vector2 newPos2D = Vector2.zero;
            if (useSmoothing)
            {
                newPos2D.x = Mathf.SmoothDamp(thisTransform.position.x, target.position.x - (target.localScale.x * 0.5f), ref velocity.x, smoothTime);
                newPos2D.y = Mathf.SmoothDamp(thisTransform.position.y, target.position.y + yOffset, ref velocity.y, smoothTime);
            }
            else
            {
                newPos2D.x = target.position.x;
                newPos2D.y = target.position.y + yOffset;

            }


            Vector3 newPos = new Vector3(newPos2D.x, newPos2D.y, transform.position.z);

            transform.position = Vector3.Slerp(transform.position, newPos, Time.time);

            transform.localScale = new Vector3(Mathf.Sign(target.transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isFollowing)
        {
            isFollowing = true;
        }
    }
}
