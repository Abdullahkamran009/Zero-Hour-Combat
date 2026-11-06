using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 30;
    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !collected)
        {
            Gun activeGun = other.GetComponentInChildren<Gun>();

            if (activeGun != null)
            {
                collected = true;

                // Limit total ammo if needed
                int newReserve = Mathf.Min(activeGun.reserveAmmo + ammoAmount, activeGun.maxReserveAmmo);
                activeGun.reserveAmmo = newReserve;

                    UI.instance.UpdateAmmo(activeGun.currentAmmo, activeGun.maxAmmo, activeGun.reserveAmmo);


                Destroy(gameObject);
            }
        }
    }
}
