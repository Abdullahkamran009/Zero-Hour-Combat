using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Scope : MonoBehaviour
{
    public Animator animator;

    public GameObject scopeOverlay;
    public GameObject weaponCamera;
    public Camera mainCamera;

    public float scopedFOV = 15f;
    private float normalFOV;

    private bool Scoped = false;
    private PhotonView PV;

    private void Awake()
    {
        PV = GetComponentInParent<PhotonView>();
    }

    private void Start()
    {
        if (PV != null && PhotonNetwork.InRoom && !PV.IsMine)
        {
            this.enabled = false;
            return;
        }

        if (scopeOverlay == null)
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == "ScopeOverlay")
                {
                    scopeOverlay = obj;
                    break;
                }
            }

            if (scopeOverlay != null)
                Debug.Log("✅ ScopeOverlay found (even if inactive)");
            else
                Debug.LogWarning("❌ ScopeOverlay NOT FOUND");
        }

        if (weaponCamera == null)
            weaponCamera = GameObject.Find("WeaponCamera");

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
            normalFOV = mainCamera.fieldOfView;
    }

    private void Update()
    {
        if (PV != null && PhotonNetwork.InRoom && !PV.IsMine)
            return;

        if (Input.GetButtonDown("Fire2"))
        {
            Scoped = !Scoped;
            animator.SetBool("IsScoped", Scoped);

            if (Scoped)
                StartCoroutine(OnScoped());
            else
                OnUnScoped();
        }
    }

    void OnUnScoped()
    {
        if (scopeOverlay != null)
            scopeOverlay.SetActive(false);

        if (weaponCamera != null)
            weaponCamera.SetActive(true);

        if (mainCamera != null)
            mainCamera.fieldOfView = normalFOV;
    }

    IEnumerator OnScoped()
    {
        yield return new WaitForSeconds(0.15f);

        if (scopeOverlay != null)
            scopeOverlay.SetActive(true);

        if (weaponCamera != null)
            weaponCamera.SetActive(false);

        if (mainCamera != null)
        {
            normalFOV = mainCamera.fieldOfView;
            mainCamera.fieldOfView = scopedFOV;
        }
    }
}
