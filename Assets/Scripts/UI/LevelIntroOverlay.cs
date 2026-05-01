using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
        EnsureOverlayReferences();

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

    private void EnsureOverlayReferences()
    {
        if (overlayGroup == null)
            overlayGroup = GetComponentInChildren<CanvasGroup>(true);

        if (overlayGroup == null)
        {
            Canvas canvas = GetComponentInChildren<Canvas>(true);
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("LevelIntroCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvasGO.transform.SetParent(transform, false);

                canvas = canvasGO.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 999;

                CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.matchWidthOrHeight = 0.5f;
            }

            GameObject overlayGO = new GameObject("LevelIntroOverlay", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            overlayGO.transform.SetParent(canvas.transform, false);

            RectTransform overlayRect = overlayGO.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            Image bg = overlayGO.GetComponent<Image>();
            bg.color = Color.black;
            bg.raycastTarget = true;

            overlayGroup = overlayGO.GetComponent<CanvasGroup>();
        }

        if (levelText == null && overlayGroup != null)
        {
            levelText = overlayGroup.GetComponentInChildren<TextMeshProUGUI>(true);
        }

        if (levelText == null && overlayGroup != null)
        {
            GameObject textGO = new GameObject("LevelTitleText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGO.transform.SetParent(overlayGroup.transform, false);

            RectTransform textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = new Vector2(900f, 220f);

            TextMeshProUGUI tmp = textGO.GetComponent<TextMeshProUGUI>();
            tmp.text = title;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 96f;
            tmp.color = Color.white;

            levelText = tmp;
        }
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
