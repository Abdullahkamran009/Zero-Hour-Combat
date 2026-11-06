using Photon.Pun;
using System.Collections;
using UnityEngine;

public class AutoGun : MGun
{
    [SerializeField] Camera cam;
    [SerializeField] float fireRate = 0.1f;

    PhotonView PV;
    bool isShooting = false;
    float nextFireTime = 0f;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void Use()
    {
        if (!isShooting && Time.time >= nextFireTime)
        {
            isShooting = true;
            StartCoroutine(ShootContinuously());
        }
    }

    IEnumerator ShootContinuously()
    {
        while (Input.GetMouseButton(0))
        {
            nextFireTime = Time.time + fireRate;

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            ray.origin = cam.transform.position;

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
                PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
            }

            yield return new WaitForSeconds(fireRate);
        }

        isShooting = false;
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if (colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }
}
