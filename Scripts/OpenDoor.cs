using UnityEngine;
using Cinemachine;
using System.Linq;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] private GameObject[] coins;

    private Vector3 oldPos;

    private Vector3 newPos2D;

    private Vector2 velocity;

    public float smoothTime = 2f;

    [SerializeField] private Vector2 offset;

    private void Awake()
    {
        oldPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
    private void Update()
    {

        for (int i = 0; i < coins.Length; i++)
        {
            //Checks if all the coins are hit

            if (coins.All(coins => coins.CompareTag("hit")))
            {
          
                newPos2D.y = Mathf.SmoothDamp(transform.position.y, oldPos.y + offset.y, ref velocity.y, smoothTime);

                newPos2D.x = Mathf.SmoothDamp(transform.position.x, oldPos.x + offset.x, ref velocity.x, smoothTime);

                Vector3 newPos = new Vector3(newPos2D.x, newPos2D.y, transform.position.z);

                transform.position = Vector3.Slerp(transform.position, newPos, Time.time);

            }
            else
            {
                transform.position = oldPos;

            }

        }

    }

}
