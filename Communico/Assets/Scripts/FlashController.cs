using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlashController : MonoBehaviour
{
    [Header("Flash Settings")]
    public bool fadeInOnTrigger = false;   // If true, you trigger fade in manually
    public bool fadeOutOnEnable = false;   // If true, will automatically fade out when enabled
    public float fadeDuration = 0.3f;      // Duration of fade
    public float delay = 0f;               // Optional delay before fade

    private Image flashImage;

    void Awake()
    {
        flashImage = GetComponent<Image>();
        if (flashImage == null)
            Debug.LogError("FlashController: No Image component found!");
    }

    void OnEnable()
    {
        // Auto fade out when panel opens
        if (fadeOutOnEnable)
        {
            flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 1f);
            StartCoroutine(FadeTo(0f, fadeDuration, delay));
        }
        else if (!fadeInOnTrigger)
        {
            // Hide image by default if not fading in manually
            flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 0f);
        }
    }

    /// <summary>
    /// Call this to flash (fade in) manually, e.g., from your photo button
    /// </summary>
    public void TriggerFlash()
    {
        StopAllCoroutines();
        flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 1f);
        StartCoroutine(FadeTo(0f, fadeDuration, delay));
    }

    IEnumerator FadeTo(float targetAlpha, float duration, float startDelay = 0f)
    {
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        Color initial = flashImage.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            flashImage.color = new Color(initial.r, initial.g, initial.b, Mathf.Lerp(initial.a, targetAlpha, t));
            yield return null;
        }

        flashImage.color = new Color(initial.r, initial.g, initial.b, targetAlpha);
    }
}

