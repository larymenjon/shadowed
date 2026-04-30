using System.Collections;
using UnityEngine;
using TMPro;

public class LevelIntroOverlay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup overlayGroup;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private string title = "LEVEL 1";

    [Header("Timing")]
    [SerializeField] private float growDuration = 0.85f;
    [SerializeField] private float holdTime = 0.5f;
    [SerializeField] private float fadeDuration = 1.2f;
    [SerializeField] private Vector3 startScale = new Vector3(0.4f, 0.4f, 0.4f);
    [SerializeField] private Vector3 endScale = Vector3.one;

    [Header("Optional")]
    [SerializeField] private MonoBehaviour[] disableDuringIntro;

    private void Start()
    {
        Time.timeScale = 1f;
        if (levelText != null)
        {
            levelText.text = title;
        }

        SetGameplayEnabled(false);
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        if (overlayGroup != null)
        {
            overlayGroup.alpha = 1f;
            overlayGroup.blocksRaycasts = true;
        }

        if (levelText != null)
        {
            levelText.transform.localScale = startScale;
        }

        float growTimer = 0f;
        while (growTimer < growDuration)
        {
            growTimer += Time.deltaTime;
            float t = Mathf.Clamp01(growTimer / Mathf.Max(0.01f, growDuration));
            float eased = Mathf.SmoothStep(0f, 1f, t);

            if (levelText != null)
            {
                levelText.transform.localScale = Vector3.Lerp(startScale, endScale, eased);
            }

            yield return null;
        }

        yield return new WaitForSeconds(holdTime);

        if (overlayGroup != null)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                overlayGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
                yield return null;
            }

            overlayGroup.alpha = 0f;
            overlayGroup.blocksRaycasts = false;
            overlayGroup.gameObject.SetActive(false);
        }

        SetGameplayEnabled(true);
    }

    private void SetGameplayEnabled(bool enabled)
    {
        if (disableDuringIntro == null)
            return;

        for (int i = 0; i < disableDuringIntro.Length; i++)
        {
            if (disableDuringIntro[i] != null)
            {
                disableDuringIntro[i].enabled = enabled;
            }
        }
    }
}
