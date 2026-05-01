using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MainMenuUI : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject creditsPanel;

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

    [Header("Flow")]
    [SerializeField] private string firstSceneAfterMenu = "LoginFake";

    private int selectedIndex;
    private Button currentlyHoveredButton;
    private readonly Dictionary<Button, Graphic> buttonLabelGraphic = new Dictionary<Button, Graphic>();

    private void Start()
    {
        AutoConfigureIfNeeded();

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
    }

    private void Update()
    {
        if (menuButtons == null || menuButtons.Length == 0)
            return;

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

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
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
        if ((menuButtons == null || menuButtons.Length == 0))
        {
            var foundButtons = GetComponentsInChildren<Button>(true)
                .Where(b => b != null && b.gameObject.activeInHierarchy)
                .OrderByDescending(b => b.GetComponent<RectTransform>() != null ? b.GetComponent<RectTransform>().position.y : 0f)
                .ToArray();

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
}

