using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform_Hor : MonoBehaviour
{
    private float x = 0;
    private float pos;
    [SerializeField] private float displacement;
    [Range(0f, 0.05f)] public float speed;

    private void Awake()
    {
        pos = gameObject.transform.position.x; 
    }
    // Update is called once per frame
    void Update()
    {
        x += Time.deltaTime + speed;

        gameObject.transform.position = new Vector3(pos + displacement*Mathf.Sin(x), gameObject.transform.position.y,gameObject.transform.position.z);

    }
}
