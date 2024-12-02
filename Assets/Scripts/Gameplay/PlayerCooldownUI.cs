using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCooldownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI blinkText;
    [SerializeField] private Transform player;  // Player's transform
    [SerializeField] private RectTransform uiRectTransform;  // The RectTransform of the PlayerCooldownUI
    private Vector3 offset = new (0, -0.5f, 0);  // Offset to position UI above the player

    void Start()
    {
        // Check if we have valid references
        if (player == null || uiRectTransform == null)
        {
            Debug.LogError("Player or RectTransform is not assigned.");
        }
    }

    void Update()
    {
        if (player != null && uiRectTransform != null)
        {
            // Convert the player's world position to screen space
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(player.position + offset);

            // Update the position of the UI element in screen space
            uiRectTransform.position = screenPosition;

            Debug.Log("Player Screen Position: " + screenPosition);


            // You can update the cooldown UI elements here (e.g., sliders, texts)
            UpdateCooldownUI();
        }
    }

    public void UpdateTeleportCooldown(float score)
    {
        Debug.Log("UpdateTeleportCooldown: " + score);

        if (score <= 0)
        {
            blinkText.text = "READY".ToString();
        }
        else
        {
            blinkText.text = score.ToString();
        }
    }


    private void UpdateCooldownUI()
    {
        // Call UpdateTeleportCooldown and UpdateUltimateCooldown from external logic
        // This ensures sliders are always up-to-date
    }
}
