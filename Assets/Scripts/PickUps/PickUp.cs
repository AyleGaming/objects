using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickUp : MonoBehaviour
{
    [SerializeField] private Weapon pickUp;
    [SerializeField] protected AudioClip pickUpAudio;
    protected float pickUpVolume = 1f;

    [SerializeField] private float fadeDuration = .5f; // Time it takes for one fade-in/out cycle
    [SerializeField] private float lifetime = 10f;   // Total time before the pickup is destroyed
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isFadingIn = true;

    // Start is called before the first frame update
    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on PickUp!");
            return;
        }

        // Store the original color
        originalColor = spriteRenderer.color;

        // Start the fading effect
        StartCoroutine(FadeEffect());

        // Destroy the object after its lifetime
        Destroy(gameObject, lifetime);
    }
    private System.Collections.IEnumerator FadeEffect()
    {
        float fadeTimer = 0f;

        while (true)
        {
            fadeTimer += Time.deltaTime;

            // Calculate the alpha value based on fade duration and current direction
            float alpha = isFadingIn
                ? Mathf.Lerp(0f, originalColor.a, fadeTimer / fadeDuration)
                : Mathf.Lerp(originalColor.a, 0f, fadeTimer / fadeDuration);

            // Update the sprite's alpha
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            // Check if the fade cycle is complete
            if (fadeTimer >= fadeDuration)
            {
                fadeTimer = 0f; // Reset the timer
                isFadingIn = !isFadingIn; // Switch fade direction
            }

            yield return null; // Wait until the next frame
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody.CompareTag("Player"))
        {
            PickMeUp(collision.attachedRigidbody.GetComponent<Player>());


            //            collision.attachedRigidbody.GetComponent<Player>().currentWeapon = pickUp;
        }
    }

    protected abstract void PickMeUp(Player playerInTrigger);
}
