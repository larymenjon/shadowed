using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialDialogController : MonoBehaviour
{
    private enum FinishAction
    {
        HideOnly = 0,
        LoadScene = 1
    }

    [Header("UI")]
    [SerializeField] private GameObject dialogRoot;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TextMeshProUGUI continueHintText;
    [SerializeField] private string continueButtonLabel = "Continuar";

    [Header("Text")]
    [TextArea(3, 8)]
    [SerializeField] private string[] lines =
    {
        "Preciso ir para fora desse castelo, mas toda vez que me mexo, coisas estranhas acontecem.",
        "Preciso calcular meus passos para conseguir sair daqui vivo. Boa sorte para nos.",
        "Existem 3 portas neste comodo. So uma leva para o proximo lugar. Observe e escolha com cuidado."
    };

    [Header("Input")]
    [SerializeField] private KeyCode continueKey = KeyCode.Return;
    [SerializeField] private KeyCode alternateContinueKey = KeyCode.KeypadEnter;
    [Header("Auto Start")]
    [SerializeField] private bool showOnStart = true;
    [SerializeField] private float showDelay = 0.15f;
    [Header("On Finish")]
    [SerializeField] private FinishAction onFinish = FinishAction.HideOnly;
    [SerializeField] private string nextSceneName = "Level_01";
    [SerializeField] private float nextSceneDelay = 0.2f;

    private int lineIndex = -1;
    private bool visible;
    private bool finishing;
    private CanvasGroup dialogCanvasGroup;

    private void OnValidate()
    {
        AutoWireIfNeeded();
    }

    private void Awake()
    {
        AutoWireIfNeeded();
        CacheCanvasGroup();
        SetDialogVisible(false);
    }

    private void Start()
    {
        // Garantia para cenas que venham de pause/menu com timescale alterado.
        Time.timeScale = 1f;

        if (showOnStart)
        {
            if (showDelay <= 0f)
            {
                Show();
            }
            else
            {
                StartCoroutine(ShowAfterDelayRealtime(showDelay));
            }
        }
    }

    private void Update()
    {
        if (!visible)
            return;

        if (Input.GetKeyDown(continueKey) || Input.GetKeyDown(alternateContinueKey) || Input.GetMouseButtonDown(0))
        {
            Advance();
        }
    }

    public void Show()
    {
        AutoWireIfNeeded();
        CacheCanvasGroup();
        SetDialogVisible(true);

        if (continueHintText != null)
        {
            continueHintText.text = continueButtonLabel;
        }

        visible = true;
        lineIndex = -1;
        Advance();
    }

    private void Advance()
    {
        lineIndex++;
        if (lines == null || lineIndex >= lines.Length)
        {
            FinishDialog();
            return;
        }

        if (dialogText != null)
        {
            dialogText.text = lines[lineIndex];
        }
    }

    private void Hide()
    {
        visible = false;
        SetDialogVisible(false);
    }

    private void FinishDialog()
    {
        if (finishing)
            return;

        finishing = true;
        Hide();

        if (onFinish == FinishAction.LoadScene && !string.IsNullOrWhiteSpace(nextSceneName))
        {
            Invoke(nameof(LoadNextScene), Mathf.Max(0f, nextSceneDelay));
        }
        else
        {
            finishing = false;
        }
    }

    private void LoadNextScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }

    [ContextMenu("Test Show Dialog")]
    private void TestShowDialog()
    {
        Show();
    }

    public void ContinueFromButton()
    {
        if (!visible)
            return;

        Advance();
    }

    private void AutoWireIfNeeded()
    {
        if (dialogRoot == null)
        {
            dialogRoot = gameObject;
        }

        if (dialogText == null && dialogRoot != null)
        {
            TextMeshProUGUI[] texts = dialogRoot.GetComponentsInChildren<TextMeshProUGUI>(true);
            if (texts.Length > 0)
            {
                dialogText = texts[0];
            }
            if (texts.Length > 1)
            {
                continueHintText = texts[1];
            }
        }
    }

    private System.Collections.IEnumerator ShowAfterDelayRealtime(float delay)
    {
        float elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Show();
    }

    private void CacheCanvasGroup()
    {
        if (dialogRoot == null)
            return;

        if (dialogCanvasGroup == null)
            dialogCanvasGroup = dialogRoot.GetComponent<CanvasGroup>();

        if (dialogCanvasGroup == null)
            dialogCanvasGroup = dialogRoot.AddComponent<CanvasGroup>();
    }

    private void SetDialogVisible(bool isVisible)
    {
        if (dialogRoot == null)
            return;

        // Nao desativa o GameObject do proprio script para evitar bloquear o Start.
        if (dialogRoot == gameObject)
        {
            CacheCanvasGroup();
            if (dialogCanvasGroup == null)
                return;

            dialogCanvasGroup.alpha = isVisible ? 1f : 0f;
            dialogCanvasGroup.interactable = isVisible;
            dialogCanvasGroup.blocksRaycasts = isVisible;
            return;
        }

        dialogRoot.SetActive(isVisible);
    }
}
