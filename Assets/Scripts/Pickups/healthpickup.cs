using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthpickup : MonoBehaviour
{
    public int heal;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playerhealth.instance.HealPlayer(heal);

            Destroy(gameObject);
        }
    } 
}
