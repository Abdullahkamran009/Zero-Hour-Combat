using UnityEngine;
using System.Collections;

public class EnemyMinimapReveal : MonoBehaviour
{
    public GameObject minimapIcon; // Assign in Inspector
    public float revealDuration = 3f;

    public void RevealOnMinimap()
    {
        if (!minimapIcon.activeSelf)
        {
            StartCoroutine(ShowTemporarily());
        }
    }

    private IEnumerator ShowTemporarily()
    {
        minimapIcon.SetActive(true);
        yield return new WaitForSeconds(revealDuration);
        minimapIcon.SetActive(false);
    }
}
