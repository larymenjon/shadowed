using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FakeLoginFlow : MonoBehaviour
{
    [Header("Image")]
    [SerializeField] private Image splashImage;
    [SerializeField] private Sprite fixedImage;

    [Header("Fake Login Bar")]
    [SerializeField] private Slider loginProgressBar;
    [SerializeField] private float loginDuration = 5f;

    [Header("Rotating Phrases")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private string[] phrases =
    {
        "Conectando ao sistema do castelo...",
        "Mapeando corredores e portas...",
        "Sincronizando sombras...",
        "Ajustando passos calculados...",
        "Preparando entrada no LEVEL 1..."
    };
    [SerializeField] private float phraseInterval = 1f;

    [Header("Next")]
    [SerializeField] private string nextSceneName = "Level_01";

    private void Start()
    {
        Time.timeScale = 1f;
        if (splashImage != null && fixedImage != null)
        {
            splashImage.sprite = fixedImage;
        }

        StartCoroutine(RunFlow());
    }

    private IEnumerator RunFlow()
    {
        float elapsed = 0f;
        float phraseTimer = 0f;
        int phraseIndex = 0;
        UpdatePhrase(phraseIndex);

        while (elapsed < loginDuration)
        {
            elapsed += Time.deltaTime;
            phraseTimer += Time.deltaTime;

            if (loginProgressBar != null)
            {
                loginProgressBar.value = Mathf.Clamp01(elapsed / loginDuration);
            }

            if (phrases != null && phrases.Length > 0 && phraseInterval > 0f && phraseTimer >= phraseInterval)
            {
                phraseTimer = 0f;
                phraseIndex = (phraseIndex + 1) % phrases.Length;
                UpdatePhrase(phraseIndex);
            }

            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    private void UpdatePhrase(int index)
    {
        if (statusText == null || phrases == null || phrases.Length == 0)
            return;

        statusText.text = phrases[Mathf.Clamp(index, 0, phrases.Length - 1)];
    }
}
