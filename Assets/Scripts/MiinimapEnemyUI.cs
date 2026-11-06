using UnityEngine;

public class MinimapEnemyUI : MonoBehaviour
{
    public Transform minimapCamera;         // This is your MiniMap Camera
    public Transform enemy;
    public RectTransform icon;
    public RectTransform minimapPanel;

    public float minimapScale = 1f; // Adjust depending on orthographic size

    void Update()
    {
        if (enemy == null || minimapCamera == null || icon == null || minimapPanel == null)
            return;

        // Calculate world offset from minimap camera (not player)
        Vector3 offset = enemy.position - minimapCamera.position;

        // Convert to 2D minimap space (X/Z only), and scale
        Vector2 pos2D = new Vector2(offset.x, offset.z) * minimapScale;

        // Clamp to minimap bounds (half width or height)
        float maxRange = minimapPanel.rect.width / 2f;
        pos2D = Vector2.ClampMagnitude(pos2D, maxRange);

        // Apply to UI
        icon.anchoredPosition = pos2D;
    }
}
