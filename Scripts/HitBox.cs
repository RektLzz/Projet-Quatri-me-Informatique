using UnityEngine;
using XInputDotNetPure;
using System.Collections;

public class HitBox : MonoBehaviour
{
    public static bool isDead;

    public Transform playerTrans;

    private GameMaster gm;

    [SerializeField] private Rigidbody2D rb;

    PlayerIndex playerindex;

    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isDead = true;

            GamePad.SetVibration(playerindex, 0, 0);

            StartCoroutine(Death());
        }
    }

    IEnumerator Death()
    {

        yield return new WaitForSeconds(.1f);

        playerTrans.position = gm.lastCheckPointPos;

        isDead = false;
    }
}
