using UnityEngine;

public class RecoilSystem: MonoBehaviour
{
    public Transform recoilPosition;    // Where the recoil happens (Gun model)
    public float kickBackAmount = 0.2f;  // How much back movement
    public float kickUpAmount = 0.1f;    // How much up movement
    public float returnSpeed = 5f;       // How fast it comes back

    private Vector3 originalPosition;
    private Vector3 currentRecoilOffset;

    void Start()
    {
        originalPosition = recoilPosition.localPosition;
    }

    void Update()
    {
        currentRecoilOffset = Vector3.Lerp(currentRecoilOffset, Vector3.zero, returnSpeed * Time.deltaTime);
        recoilPosition.localPosition = originalPosition + currentRecoilOffset;
    }

    public void Recoil()
    {
        currentRecoilOffset += new Vector3(0f, Random.Range(kickUpAmount * 0.5f, kickUpAmount), -Random.Range(kickBackAmount * 0.5f, kickBackAmount));
    }
}
