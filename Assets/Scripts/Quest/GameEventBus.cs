using System;

// Event bus to communicate between scenes
public static class GameEventBus
{
    // Define events that can be listened to from any scene
    public static event Action<ShrineQuestData> OnShrineInteractionStarted;
    public static event Action<ShrineQuestData> OnShrineRestored;
    public static event Action<string, string> OnDisplayShrinePrompt;
    public static event Action OnHideShrinePrompt;
    public static event Action<string> OnDisplayNotification;
    public static event Action<string, string, string> OnDisplayDocument;
    public static event Action OnHideDocument;

    // Methods to trigger events
    public static void TriggerShrineInteractionStarted(ShrineQuestData shrine)
    {
        OnShrineInteractionStarted?.Invoke(shrine);
    }

    public static void TriggerShrineRestored(ShrineQuestData shrine)
    {
        OnShrineRestored?.Invoke(shrine);
    }

    public static void TriggerDisplayShrinePrompt(string mainText, string promptText)
    {
        OnDisplayShrinePrompt?.Invoke(mainText, promptText);
    }

    public static void TriggerHideShrinePrompt()
    {
        OnHideShrinePrompt?.Invoke();
    }

    public static void TriggerDisplayNotification(string message)
    {
        OnDisplayNotification?.Invoke(message);
    }

    public static void TriggerDisplayDocument(string title, string content, string artifactId)
    {
        OnDisplayDocument?.Invoke(title, content, artifactId);
    }

    public static void TriggerHideDocument()
    {
        OnHideDocument?.Invoke();
    }
}