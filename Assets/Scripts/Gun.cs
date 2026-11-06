using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float impactforce = 30f;
    public float firerate = 15f;
    public bool isAutomatic = true;
    public Transform firePoint;
    public TrailRenderer BulletTrail;
    public Animator animator;

    public RecoilSystem recoil;


    public Camera fpsCam;
    public GameObject muzzleflash;

    private float nexttimetofire = 0f;

    [Header("Ammo Settings")]
    public int maxAmmo = 30; // bullets per magazine
    public int currentAmmo;

    public int reserveAmmo = 90; // total ammo carried
    public int maxReserveAmmo = 90; // limit reserve refill if needed

    public float reloadTime = 2f;
    private bool isReloading = false;

    [Header("Audio")]
    public AudioClip shootSound;
    private AudioSource audioSource;


    void Start()
    {
        currentAmmo = maxAmmo;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        if (isReloading) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        bool wantsToFire = isAutomatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");

        if (wantsToFire && Time.time >= nexttimetofire)
        {
            nexttimetofire = Time.time + 1f / firerate;
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }




    void Shoot()
    {

        if (currentAmmo <= 0) return;

        currentAmmo--;

        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        StartCoroutine(WaitAndSetActiveFalse());

        RaycastHit hit;
        Vector3 targetPoint = fpsCam.transform.position + fpsCam.transform.forward * range; // 🛠 Initialize to forward by default


        Debug.DrawRay(firePoint.position, firePoint.forward * range, Color.red, 1f);

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            targetPoint = hit.point;

            float finalDamage = damage;

            // Check if we hit a tagged "Head" collider
            if (hit.collider.CompareTag("Head"))
            {
                finalDamage *= 2f; // Headshot! Double damage
                Debug.Log("Headshot! Double damage applied.");
            }

            // Apply damage to target script
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(finalDamage);
            }


            // Apply impact force if hit has rigidbody
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactforce);
            }

        }

        recoil.Recoil();

        // 🔥 Always spawn Bullet Trail (whether hit or not)
        TrailRenderer Trail = Instantiate(BulletTrail, firePoint.position, Quaternion.identity);
        StartCoroutine(SpawnTrail(Trail, targetPoint));

        UI.instance.UpdateAmmo(currentAmmo, maxAmmo, reserveAmmo);
    }


    void OnEnable ()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    IEnumerator WaitAndSetActiveFalse()
    {
        muzzleflash.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        muzzleflash.SetActive(false);
    }

    IEnumerator Reload()
    {
        if (isReloading || reserveAmmo <= 0 || currentAmmo == maxAmmo) yield break;

        isReloading = true;
        Debug.Log("Reloading...");

        animator.SetBool("Reloading", true);
        //while reloading animation is playing
        yield return new WaitForSeconds(reloadTime - .25f);
        animator.SetBool("Reloading", false);
        yield return new WaitForSeconds(.25f);

        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);

        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;

        UI.instance.UpdateAmmo(currentAmmo, maxAmmo, reserveAmmo);
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 hitPoint)
    {
        float time = 0;
        Vector3 startPosition = Trail.transform.position;
        Vector3 endPosition = hitPoint;


        while (time < 1)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, endPosition, time);
            time += Time.deltaTime / Trail.time;
            yield return null;
        }

         Trail.transform.position = endPosition;

        Destroy(Trail.gameObject, Trail.time);
    }






}
