using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#if !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem.UI;
#endif
#endif

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject menuOptionsRoot;

    [Header("Menu Navigation")]
    [SerializeField] private Button[] menuButtons;
    [SerializeField] private bool selectFirstButtonOnStart = true;

    [Header("Hover Visual")]
    [SerializeField] private Image hoverImage;
    [SerializeField] private Vector2 hoverImageOffset = new Vector2(-120f, 0f);
    [SerializeField] private Vector2 hoverSizePadding = new Vector2(0f, 0f);

    [Header("Text Highlight")]
    [SerializeField] private Color normalTextColor = new Color(0.67f, 0.53f, 0.89f, 1f);
    [SerializeField] private Color highlightedTextColor = Color.white;

    [Header("Audio")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip hoverSfx;
    [SerializeField] private float hoverVolume = 1f;
    [SerializeField] private AudioClip clickSfx;
    [SerializeField] private float clickVolume = 1f;

    [Header("Flow")]
    [SerializeField] private string firstSceneAfterMenu = "LoginFake";
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private GameObject pausePanelToClose;

    private int selectedIndex;
    private Button currentlyHoveredButton;
    private readonly Dictionary<Button, Graphic> buttonLabelGraphic = new Dictionary<Button, Graphic>();

    private void Start()
    {
        EnsureMenuInputState();
        EnsureEventSystemExists();
        AutoConfigureIfNeeded();
        NormalizePanelForUI(optionsPanel);
        NormalizePanelForUI(creditsPanel);
        NormalizePanelForUI(controlsPanel);

        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
        SetMenuOptionsVisible(true);

        if (menuButtons == null || menuButtons.Length == 0)
        {
            Debug.LogWarning("[MainMenuUI] Nenhum botão encontrado para navegação.");
            return;
        }

        if (hoverImage != null)
        {
            hoverImage.raycastTarget = false;
            hoverImage.gameObject.SetActive(false);
        }

        if (selectFirstButtonOnStart)
        {
            selectedIndex = 0;
            SelectCurrentButton();
        }

        ApplyTextHighlight(null);
        BindButtonClickSounds();
    }

    private void EnsureMenuInputState()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void EnsureEventSystemExists()
    {
        if (EventSystem.current != null)
            return;

        var eventSystemGO = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        eventSystemGO.AddComponent<InputSystemUIInputModule>();
#else
        eventSystemGO.AddComponent<StandaloneInputModule>();
#endif
        SceneManager.MoveGameObjectToScene(eventSystemGO, gameObject.scene);
    }

    private void Update()
    {
        if (menuButtons == null || menuButtons.Length == 0)
            return;

        if (AnyPopupOpen())
        {
            UpdateMouseHoverState();
            return;
        }

        bool upPressed = Input.GetKeyDown(KeyCode.UpArrow);
        bool downPressed = Input.GetKeyDown(KeyCode.DownArrow);
        bool submitPressed = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            upPressed |= Keyboard.current.upArrowKey.wasPressedThisFrame;
            downPressed |= Keyboard.current.downArrowKey.wasPressedThisFrame;
            submitPressed |= Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame;
        }
#endif

        if (upPressed)
        {
            selectedIndex = (selectedIndex - 1 + menuButtons.Length) % menuButtons.Length;
            SelectCurrentButton();
        }
        else if (downPressed)
        {
            selectedIndex = (selectedIndex + 1) % menuButtons.Length;
            SelectCurrentButton();
        }

        if (submitPressed)
            menuButtons[selectedIndex].onClick.Invoke();

        UpdateMouseHoverState();
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(firstSceneAfterMenu);
    }

    public void Resume()
    {
        Time.timeScale = 1f;

        if (pausePanelToClose != null)
            pausePanelToClose.SetActive(false);
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void Exit()
    {
        QuitGame();
    }

    public void Options()
    {
        OpenOptions();
    }

    public void Controls()
    {
        OpenControls();
    }

    public void OpenOptions()
    {
        SetPanelState(optionsPanel, true);
    }

    public void CloseOptions()
    {
        SetPanelState(optionsPanel, false);
    }

    public void OpenCredits()
    {
        SetPanelState(creditsPanel, true);
    }

    public void CloseCredits()
    {
        SetPanelState(creditsPanel, false);
    }

    public void OpenControls()
    {
        SetPanelState(controlsPanel, true);
    }

    public void CloseControls()
    {
        SetPanelState(controlsPanel, false);
    }

    public void ReturnFromPopup()
    {
        CloseOptions();
        CloseCredits();
        CloseControls();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SelectCurrentButton()
    {
        var targetButton = menuButtons[selectedIndex];
        if (targetButton == null)
            return;

        EventSystem.current?.SetSelectedGameObject(targetButton.gameObject);
        ApplyTextHighlight(targetButton);
    }

    private void UpdateMouseHoverState()
    {
        if (EventSystem.current == null)
            return;

        var pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        Button hovered = null;
        foreach (var hit in results)
        {
            if (hit.gameObject == null)
                continue;

            var btn = hit.gameObject.GetComponentInParent<Button>();
            if (btn == null)
                continue;

            for (int i = 0; i < menuButtons.Length; i++)
            {
                if (menuButtons[i] == btn)
                {
                    hovered = btn;
                    selectedIndex = i;
                    break;
                }
            }

            if (hovered != null)
                break;
        }

        if (hovered != currentlyHoveredButton)
        {
            currentlyHoveredButton = hovered;

            if (currentlyHoveredButton != null)
            {
                PlayHoverSound();
                EventSystem.current.SetSelectedGameObject(currentlyHoveredButton.gameObject);
                ApplyTextHighlight(currentlyHoveredButton);

                if (hoverImage != null)
                {
                    hoverImage.gameObject.SetActive(true);
                    MoveHoverImageToButton(currentlyHoveredButton);
                }
            }
            else if (hoverImage != null)
            {
                hoverImage.gameObject.SetActive(false);
                ApplyTextHighlight(null);
            }
        }
    }

    private void MoveHoverImageToButton(Button targetButton)
    {
        if (hoverImage == null || targetButton == null)
            return;

        var hoverRect = hoverImage.rectTransform;
        var buttonRect = targetButton.GetComponent<RectTransform>();

        if (hoverRect == null || buttonRect == null)
            return;

        if (hoverRect.parent != buttonRect)
            hoverRect.SetParent(buttonRect, false);

        hoverRect.anchorMin = new Vector2(0.5f, 0.5f);
        hoverRect.anchorMax = new Vector2(0.5f, 0.5f);
        hoverRect.pivot = new Vector2(0.5f, 0.5f);
        hoverRect.anchoredPosition = hoverImageOffset;
        hoverRect.sizeDelta = buttonRect.rect.size + hoverSizePadding;
        hoverRect.SetAsFirstSibling();
    }

    private void AutoConfigureIfNeeded()
    {
        var foundButtons = GetComponentsInChildren<Button>(true)
            .Where(b => b != null && b.gameObject.activeInHierarchy)
            .OrderByDescending(b => b.GetComponent<RectTransform>() != null ? b.GetComponent<RectTransform>().position.y : 0f)
            .ToArray();

        bool shouldRefreshButtons = menuButtons == null || menuButtons.Length == 0;
        if (!shouldRefreshButtons && foundButtons.Length > menuButtons.Length)
            shouldRefreshButtons = true;

        if (shouldRefreshButtons)
        {
            menuButtons = foundButtons;
        }

        if (hoverImage == null)
        {
            var hoverTransform = FindDeepChild(transform.root, "ButtonFX");
            if (hoverTransform != null)
                hoverImage = hoverTransform.GetComponent<Image>();
        }

        if (hoverImage != null)
            hoverImage.raycastTarget = false;

        CacheButtonLabels();
    }

    private Transform FindDeepChild(Transform parent, string childName)
    {
        if (parent == null)
            return null;

        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            var result = FindDeepChild(child, childName);
            if (result != null)
                return result;
        }

        return null;
    }

    private void CacheButtonLabels()
    {
        buttonLabelGraphic.Clear();

        foreach (var button in menuButtons)
        {
            if (button == null)
                continue;

            Graphic label = button.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label == null)
                label = button.GetComponentInChildren<Text>(true);

            if (label != null)
                buttonLabelGraphic[button] = label;
        }
    }

    private void ApplyTextHighlight(Button highlightedButton)
    {
        foreach (var pair in buttonLabelGraphic)
        {
            if (pair.Key == null || pair.Value == null)
                continue;

            pair.Value.color = pair.Key == highlightedButton ? highlightedTextColor : normalTextColor;
        }
    }

    private void SetPanelState(GameObject panel, bool shouldOpen)
    {
        if (panel == null)
        {
            Debug.LogWarning("[MainMenuUI] Painel nao atribuido no Inspector.");
            return;
        }

        if (shouldOpen)
        {
            EnsureParentChainActive(panel.transform);
            PreparePanelForDisplay(panel);
        }

        panel.SetActive(shouldOpen);
        SetMenuOptionsVisible(!AnyPopupOpen());
    }

    private void PreparePanelForDisplay(GameObject panel)
    {
        panel.transform.SetAsLastSibling();
        panel.transform.localScale = Vector3.one;

        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    private void EnsureParentChainActive(Transform child)
    {
        Transform current = child.parent;
        while (current != null)
        {
            if (!current.gameObject.activeSelf)
                current.gameObject.SetActive(true);

            if (current == transform.root)
                break;

            current = current.parent;
        }
    }

    private bool AnyPopupOpen()
    {
        return (optionsPanel != null && optionsPanel.activeSelf)
            || (creditsPanel != null && creditsPanel.activeSelf)
            || (controlsPanel != null && controlsPanel.activeSelf);
    }

    private void SetMenuOptionsVisible(bool isVisible)
    {
        if (menuOptionsRoot != null)
        {
            menuOptionsRoot.SetActive(isVisible);
            return;
        }

        if (menuButtons == null)
            return;

        foreach (var button in menuButtons)
        {
            if (button != null)
                button.gameObject.SetActive(isVisible);
        }
    }

    private void PlayHoverSound()
    {
        if (hoverSfx == null)
            return;

        if (uiAudioSource != null)
        {
            uiAudioSource.PlayOneShot(hoverSfx, hoverVolume);
            return;
        }

        AudioSource.PlayClipAtPoint(hoverSfx, Camera.main != null ? Camera.main.transform.position : Vector3.zero, hoverVolume);
    }

    private void NormalizePanelForUI(GameObject panel)
    {
        if (panel == null)
            return;

        Canvas canvas = GetComponentInParent<Canvas>(true);
        if (canvas == null)
            canvas = Object.FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);

        if (canvas == null)
            return;

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        if (panelRect == null)
            return;

        panelRect.SetParent(canvas.transform, false);
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panelRect.localScale = Vector3.one;
    }

    public void PlayClickSound()
    {
        if (clickSfx == null)
            return;

        if (uiAudioSource != null)
        {
            uiAudioSource.PlayOneShot(clickSfx, clickVolume);
            return;
        }

        AudioSource.PlayClipAtPoint(clickSfx, Camera.main != null ? Camera.main.transform.position : Vector3.zero, clickVolume);
    }

    private void BindButtonClickSounds()
    {
        if (menuButtons == null)
            return;

        foreach (var button in menuButtons)
        {
            if (button == null)
                continue;

            button.onClick.RemoveListener(PlayClickSound);
            button.onClick.AddListener(PlayClickSound);
        }
    }
}

