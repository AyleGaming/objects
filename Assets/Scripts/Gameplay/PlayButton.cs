using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public CanvasGroup canvasGroup; // Reference to the CanvasGroup
    public float fadeDuration = 1.0f; // Duration of the fade effect
    public string nextSceneName;

    private void Start()
    {
        // Ensure CanvasGroup is fully visible and interactive at the start
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPlayButtonClicked()
    {
        StartCoroutine(FadeOutAndLoadScene(nextSceneName));
    }

    private System.Collections.IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        float elapsed = 0f;

        // Fade out
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - (elapsed / fadeDuration));
            yield return null;
        }

        // Ensure alpha is fully transparent
        canvasGroup.alpha = 0;

        // Load the new scene
        SceneManager.LoadScene(sceneName);
    }
}
