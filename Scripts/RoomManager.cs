using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour { 

    [SerializeField] private GameObject virualCam;
    [SerializeField] private GameObject snow;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            virualCam.SetActive(true);

            if(snow !=null)
            snow.SetActive(true);
          
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virualCam.SetActive(false);

            if (snow != null)
                snow.SetActive(false);
         
        }
    }



}
