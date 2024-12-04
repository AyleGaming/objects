using System.Collections;
using UnityEngine;

public class EffectCleanup : MonoBehaviour
{
    public float cleanupDelay = 3f; // Time in seconds to wait before destroying the GameObject

    public void PlayEffectAndCleanup()
    {
        StartCoroutine(CleanupAfterDelay());
    }

    private IEnumerator CleanupAfterDelay()
    {
        // Optional: Wait for an animation, particle system, or other effect to play
        yield return new WaitForSeconds(cleanupDelay);

        // Destroy the GameObject
        Destroy(gameObject);
    }
}
