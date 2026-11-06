using UnityEngine;

public class MiniMapIconRotation : MonoBehaviour
{
    public Transform playerBody;

    void LateUpdate()
    {
        // Rotate only on Z axis based on player Y rotation
        float yRotation = playerBody.eulerAngles.y;
        transform.localEulerAngles = new Vector3(90f, 0f, -yRotation);
    }
}