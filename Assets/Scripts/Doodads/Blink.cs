using System.Collections;
using UnityEngine;

public class Blink : MonoBehaviour
{
    [SerializeField] private GameObject blinkEffect;
    [SerializeField] private float fadeDuration = 2f; // Time to fully fade in
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void FadeIn()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsed / fadeDuration)); // Decrease alpha

            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }

            yield return null;
        }

        // Ensure fully transparent at the end
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0f;
            spriteRenderer.color = color;
        }
        Instantiate(blinkEffect, transform.position, Quaternion.identity);
    }
}
