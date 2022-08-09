using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class bossDoor : MonoBehaviour
{
    private bool entered = false;
    public Transform playerPos;
    [SerializeField] private Vector2 velocity;
    public float smoothTime = 2f;
    private Vector2 newPos2D;
    public float duration;
    public float yOffset;
    [SerializeField] CinemachineImpulseSource _source;

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        newPos2D = Vector2.zero;
        if (collision.CompareTag("Player"))
        {
               
            entered = true;
        }

    }
    private void Update()
    {
  
        if (entered && duration > 0)
        {
            duration -= Time.deltaTime;
            newPos2D.y = Mathf.SmoothDamp(transform.position.y, transform.position.y + yOffset, ref velocity.x, smoothTime);
            newPos2D.x = transform.position.x;


            Vector3 newPos = new Vector3(newPos2D.x, newPos2D.y, transform.position.z);
            transform.position = Vector3.Slerp(transform.position, newPos, Time.time );
            _source.GenerateImpulse();
        }

    }
}
