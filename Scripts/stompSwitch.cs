using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stompSwitch : MonoBehaviour
{
    [SerializeField] private Transform doorTrans;

    private PlayerMovement pl;

    private Vector3 DashDirection;

    private HitBox hb;

    private string tag;

    [SerializeField] private GameObject virtualCam;

    private Vector3 oldDoorPos;

    private Vector3 newPos2D;

    [SerializeField] private Vector2 offset;

    private Vector2 velocity;

    [SerializeField] private float smoothTime = 2f;

    private void Awake()
    {
        hb = GameObject.FindGameObjectWithTag("Hitbox").GetComponent<HitBox>();

        tag = gameObject.tag;

        oldDoorPos = new Vector3(doorTrans.position.x,doorTrans.position.y,doorTrans.position.z);

        pl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

    }

    private void Update()
    {

        if (HitBox.isDead && virtualCam.activeInHierarchy)
        {
            doorTrans.position = oldDoorPos;

            gameObject.tag = tag;
        }

        Vector2 newPos2D = Vector2.zero;

        if (gameObject.CompareTag("hit"))
        {

            newPos2D.y = Mathf.SmoothDamp(doorTrans.position.y, oldDoorPos.y + (doorTrans.up.y * offset.y), ref velocity.y, smoothTime);

            newPos2D.x = Mathf.SmoothDamp(doorTrans.position.x, oldDoorPos.x + (doorTrans.up.x * offset.x), ref velocity.x, smoothTime);

            Vector3 newPos = new Vector3(newPos2D.x, newPos2D.y, doorTrans.position.z);

            doorTrans.position = Vector3.Slerp(doorTrans.position, newPos, Time.time);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
         
        DashDirection = new Vector3(pl.DiscretDirectionX, pl.DiscretDirectionY, 0);

        if (collision.CompareTag("Player") && pl.CanDash == false && DashDirection == - transform.up || collision.CompareTag("Object")) 
        {
                     
            gameObject.tag = "hit";

            StartCoroutine(Press());

        }

    }

    IEnumerator Press()
    {
        gameObject.transform.position = new Vector3(transform.position.x - transform.up.x/5, transform.position.y - transform.up.y / 5, transform.position.z);

        yield return new WaitForSeconds(.5f);

        gameObject.transform.position = new Vector3(transform.position.x + transform.up.x / 5, transform.position.y + transform.up.y / 5, transform.position.z);
    }

}
