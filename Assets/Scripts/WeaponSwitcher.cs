using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject[] weapons;
    private int currentWeapon = 0;

    void Start()
    {
        SelectWeapon(currentWeapon);
    }

    void Update()
    {
        // Number keys (1, 2, 3...)
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchTo(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchTo(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchTo(2);

        // Scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            currentWeapon = (currentWeapon + 1) % weapons.Length;
            SelectWeapon(currentWeapon);
        }
        else if (scroll < 0f)
        {
            currentWeapon = (currentWeapon - 1 + weapons.Length) % weapons.Length;
            SelectWeapon(currentWeapon);
        }
    }

    void SwitchTo(int index)
    {
        if (index >= 0 && index < weapons.Length)
        {
            currentWeapon = index;
            SelectWeapon(index);
        }
    }

    void SelectWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            bool isActive = (i == index);
            weapons[i].SetActive(isActive);

            if (isActive)
            {
                Gun currentGun = weapons[i].GetComponentInChildren<Gun>();
                if (currentGun != null)
                {
                    UI.instance.UpdateAmmo(currentGun.currentAmmo, currentGun.maxAmmo, currentGun.reserveAmmo);
                }

            }
        }
    }

}
