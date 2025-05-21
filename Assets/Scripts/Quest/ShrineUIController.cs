using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

// UI Controller for Shrine prompts and blackout effects
public class ShrineUIController : MonoBehaviour
{
    public static ShrineUIController instance;

    [SerializeField] private CanvasGroup uiCanvasGroup;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private CanvasGroup blackoutCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Notification")]
    [SerializeField] private CanvasGroup notificationGroup;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float notificationDuration = 3f;

    [Header("Document View")]
    [SerializeField] private CanvasGroup documentGroup;
    [SerializeField] private TextMeshProUGUI documentTitleText;
    [SerializeField] private TextMeshProUGUI documentContentText;
    [SerializeField] private Button closeButton;

    private Coroutine fadeCoroutine;
    private Coroutine notificationCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        // Subscribe to events
        GameEventBus.OnDisplayShrinePrompt += HandleDisplayPrompt;
        GameEventBus.OnHideShrinePrompt += HandleHidePrompt;
        GameEventBus.OnShrineInteractionStarted += HandleShrineInteraction;
        GameEventBus.OnShrineRestored += HandleShrineRestored;
        GameEventBus.OnDisplayNotification += HandleDisplayNotification;
        GameEventBus.OnDisplayDocument += HandleDisplayDocument;
        GameEventBus.OnHideDocument += HandleHideDocument;

        // Initialize UI
        if (uiCanvasGroup) uiCanvasGroup.alpha = 0f;
        if (blackoutCanvasGroup)
        {
            blackoutCanvasGroup.alpha = 0f;
            blackoutCanvasGroup.blocksRaycasts = false;
        }
        if (notificationGroup)
        {
            notificationGroup.alpha = 0f;
            notificationGroup.blocksRaycasts = false;
        }
        if (documentGroup)
        {
            documentGroup.alpha = 0f;
            documentGroup.blocksRaycasts = false;
            documentGroup.interactable = false;
        }

        // Set up close button event listener
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseDocument);
            Debug.Log("Close button listener attached in OnEnable");
        }
        else
        {
            Debug.LogError("Close button reference is not assigned");
        }
    }

    void Start()
    {
        // Double check button setup in Start
        if (closeButton == null)
        {
            // Try to find it if not assigned
            closeButton = documentGroup.GetComponentInChildren<Button>();
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(CloseDocument);
                Debug.Log("Found and attached close button in Start");
            }
        }
    }

    void OnDisable()
    {
        // Unsubscribe from events
        GameEventBus.OnDisplayShrinePrompt -= HandleDisplayPrompt;
        GameEventBus.OnHideShrinePrompt -= HandleHidePrompt;
        GameEventBus.OnShrineInteractionStarted -= HandleShrineInteraction;
        GameEventBus.OnShrineRestored -= HandleShrineRestored;
        GameEventBus.OnDisplayNotification -= HandleDisplayNotification;
        GameEventBus.OnDisplayDocument -= HandleDisplayDocument;
        GameEventBus.OnHideDocument -= HandleHideDocument;
    }

    void Update()
    {
        // Add keyboard escape support for document
        if (documentGroup != null && documentGroup.alpha > 0 && Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed - closing document");
            CloseDocument();
        }
    }

    private void HandleDisplayPrompt(string main, string prompt)
    {
        if (uiCanvasGroup == null) return;

        uiCanvasGroup.alpha = 1f;

        if (stateText != null)
            stateText.text = main;

        if (promptText != null)
            promptText.text = prompt;
    }

    private void HandleHidePrompt()
    {
        if (uiCanvasGroup != null)
            uiCanvasGroup.alpha = 0f;
    }

    private void HandleDisplayNotification(string message)
    {
        if (notificationGroup == null || notificationText == null) return;

        // Cancel any running notification
        if (notificationCoroutine != null)
            StopCoroutine(notificationCoroutine);

        // Start new notification
        notificationCoroutine = StartCoroutine(ShowNotification(message));
    }

    private IEnumerator ShowNotification(string message)
    {
        notificationText.text = message;
        notificationGroup.alpha = 1f;
        notificationGroup.blocksRaycasts = true;
        notificationGroup.interactable = true;

        yield return new WaitForSeconds(notificationDuration);

        // Fade out
        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            notificationGroup.alpha = Mathf.Clamp01(1f - (t / 0.5f));
            yield return null;
        }

        notificationGroup.alpha = 0f;
        notificationGroup.blocksRaycasts = false;
        notificationGroup.interactable = false;
        notificationCoroutine = null;
    }

    private void HandleDisplayDocument(string title, string content, string artifactId)
    {
        if (documentGroup == null) return;

        // Set text
        if (documentTitleText != null)
            documentTitleText.text = title;

        if (documentContentText != null)
            documentContentText.text = content;

        // Show document UI
        documentGroup.alpha = 1f;
        documentGroup.blocksRaycasts = true;
        documentGroup.interactable = true;

        // Make sure button is set up
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseDocument);
            closeButton.Select(); // Select the button for navigation
            Debug.Log("Close button selected and listener attached");
        }

        // Pause game
        Time.timeScale = 0f;
    }

    private void HandleHideDocument()
    {
        if (documentGroup == null) return;

        documentGroup.alpha = 0f;
        documentGroup.blocksRaycasts = false;
        documentGroup.interactable = false;

        // Resume game 
        Time.timeScale = 1f;

        Debug.Log("Document UI hidden");
    }

    // Button callback for closing document
    public void CloseDocument()
    {
        Debug.Log("Close document button clicked!");
        HandleHideDocument();
    }

    private void HandleShrineInteraction(ShrineQuestData shrine)
    {
        // Start screen fade
        if (blackoutCanvasGroup != null && fadeCoroutine == null)
            fadeCoroutine = StartCoroutine(FadeToBlack());
    }

    private void HandleShrineRestored(ShrineQuestData shrine)
    {
        // Start fade back in
        if (blackoutCanvasGroup != null && fadeCoroutine == null)
            fadeCoroutine = StartCoroutine(FadeFromBlack());
    }

    private IEnumerator FadeToBlack()
    {
        float t = 0f;
        blackoutCanvasGroup.blocksRaycasts = true;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            blackoutCanvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        blackoutCanvasGroup.alpha = 1f;
        fadeCoroutine = null;
    }

    private IEnumerator FadeFromBlack()
    {
        // Start with small delay
        yield return new WaitForSeconds(0.5f);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            blackoutCanvasGroup.alpha = Mathf.Clamp01(1f - (t / fadeDuration));
            yield return null;
        }

        blackoutCanvasGroup.alpha = 0f;
        blackoutCanvasGroup.blocksRaycasts = false;
        fadeCoroutine = null;
    }
}