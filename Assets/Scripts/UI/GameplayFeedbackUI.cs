using System.Collections;
using UnityEngine;
using TMPro;

public class GameplayFeedbackUI : MonoBehaviour
{
    public static GameplayFeedbackUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private CanvasGroup rootCanvasGroup;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Timing")]
    [SerializeField] private float visibleTime = 1.6f;
    [SerializeField] private float fadeDuration = 0.3f;

    private Coroutine showRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        HideImmediate();
    }

    public void ShowMessage(string message, Color color)
    {
        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
        }

        showRoutine = StartCoroutine(ShowRoutine(message, color));
    }

    private IEnumerator ShowRoutine(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
        }

        if (rootCanvasGroup != null)
        {
            rootCanvasGroup.gameObject.SetActive(true);
            rootCanvasGroup.alpha = 1f;
        }

        yield return new WaitForSeconds(visibleTime);

        if (rootCanvasGroup != null)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                rootCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
                yield return null;
            }
        }

        HideImmediate();
        showRoutine = null;
    }

    private void HideImmediate()
    {
        if (rootCanvasGroup == null)
            return;

        rootCanvasGroup.alpha = 0f;
        rootCanvasGroup.gameObject.SetActive(false);
    }
}
