using UnityEngine;
using System.Collections;
using UnityEngine.Events;

// The shrine object in the world scene
public class ShrineQuest : MonoBehaviour
{
    [Header("Quest Settings")]
    public ShrineQuestData questData;
    public KeyCode interactKey = KeyCode.I;
    public float interactRadius = 3f;

    [Header("VFX & Animation")]
    public ParticleSystem restoreEffect;
    public Animator shrineAnimator;
    public float fadeInOutDuration = 0.5f;

    [Header("Events")]
    public UnityEvent OnShrineRestored;

    private Transform player;
    private bool playerInRange = false;

    void Start()
    {
        // Try to find player
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go != null) player = go.transform;

        // Check if QuestManager has this artifact already
        if (QuestManager.Instance != null && questData != null)
        {
            // Sync with QuestManager if artifact is already collected
            if (questData.currentState == ShrineQuestData.QuestState.MissingArtifact &&
                QuestManager.Instance.HasArtifact(questData.requiredArtifactId))
            {
                Debug.Log($"[ShrineQuest] Start: Found we already have the required artifact: {questData.requiredArtifactId}");
                questData.currentState = ShrineQuestData.QuestState.Ready;
            }
        }

        // Update visual state based on quest data
        UpdateShrineVisuals();

        Debug.Log($"[ShrineQuest] Shrine {questData.questId} initialized in state: {questData.currentState}");
    }

    void Update()
    {
        if (player == null) return;

        // Check player distance
        float distance = Vector3.Distance(transform.position, player.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= interactRadius;

        // Handle entering/exiting range
        if (playerInRange && !wasInRange)
        {
            ShowPrompt();
        }
        else if (!playerInRange && wasInRange)
        {
            HidePrompt();
        }

        // Handle interaction
        if (playerInRange &&
            questData.currentState == ShrineQuestData.QuestState.Ready &&
            Input.GetKeyDown(interactKey))
        {
            RestoreShrine();
        }
    }

    private void ShowPrompt()
    {
        if (questData == null) return;

        string mainText = "";
        string promptText = "";

        switch (questData.currentState)
        {
            case ShrineQuestData.QuestState.Locked:
                mainText = questData.lockedMessage;
                break;

            case ShrineQuestData.QuestState.MissingArtifact:
                mainText = questData.missingArtifactMessage;
                if (!string.IsNullOrEmpty(questData.artifactDescription))
                {
                    mainText += $"\n{questData.artifactDescription}";
                }
                break;

            case ShrineQuestData.QuestState.Ready:
                mainText = "Shrine is ready to be restored";
                promptText = questData.readyMessage.Replace("[E]", interactKey.ToString());
                break;

            case ShrineQuestData.QuestState.Restored:
                mainText = questData.restoredMessage;
                break;
        }

        // Send event to UI controller
        GameEventBus.TriggerDisplayShrinePrompt(mainText, promptText);
    }

    private void HidePrompt()
    {
        // Send event to UI controller
        GameEventBus.TriggerHideShrinePrompt();
    }

    public void UnlockShrine()
    {
        Debug.Log($"[ShrineQuest] UnlockShrine called on {questData.questId}, current state: {questData.currentState}");

        if (questData.currentState == ShrineQuestData.QuestState.Locked)
        {
            questData.currentState = ShrineQuestData.QuestState.MissingArtifact;
            UpdateShrineVisuals();

            Debug.Log($"[ShrineQuest] Shrine {questData.questId} unlocked and needs artifact: {questData.requiredArtifactId}");

            // Update UI if player is in range
            if (playerInRange)
                ShowPrompt();
        }
    }

    // Update this shrine with data from QuestManager
    public void UpdateFromData(ShrineQuestData updatedData)
    {
        Debug.Log($"[ShrineQuest] UpdateFromData: Updating shrine {questData.questId} to state: {updatedData.currentState}");

        // Make sure we're working with the same shrine
        if (questData.questId == updatedData.questId)
        {
            // Store previous state to detect changes
            var previousState = questData.currentState;
            
            // Update state
            questData.currentState = updatedData.currentState;
            
            // Update visuals
            UpdateShrineVisuals();

            // Show visual effect if shrine became ready
            if (previousState != ShrineQuestData.QuestState.Ready && 
                questData.currentState == ShrineQuestData.QuestState.Ready)
            {
                // Optional: play a small effect to show shrine is now active
                if (restoreEffect != null)
                {
                    var smallEffect = Instantiate(restoreEffect, transform.position, Quaternion.identity);
                    smallEffect.transform.localScale = smallEffect.transform.localScale * 0.5f; // Smaller effect than full restoration
                    
                    // Make it a different color or adjust parameters to distinguish from restoration effect
                    var mainModule = smallEffect.main;
                    if (mainModule.startColor.mode == ParticleSystemGradientMode.Color)
                    {
                        mainModule.startColor = new Color(0.2f, 0.8f, 1f); // Blue-ish glow instead of regular effect color
                    }
                }
            }

            // Update UI if player is in range
            if (playerInRange)
                ShowPrompt();
        }
    }

    private void RestoreShrine()
    {
        Debug.Log($"[ShrineQuest] RestoreShrine: Restoring shrine {questData.questId}");
        questData.currentState = ShrineQuestData.QuestState.Restored;
        UpdateShrineVisuals();

        // Show restored prompt
        ShowPrompt();

        // Play VFX if available
        if (restoreEffect != null)
            Instantiate(restoreEffect, transform.position, Quaternion.identity);

        // Play animation if available
        if (shrineAnimator != null)
            shrineAnimator.SetTrigger("Restore");

        // Notify the event bus about the restoration
        GameEventBus.TriggerShrineInteractionStarted(questData);

        // Start the blackout sequence
        StartCoroutine(BlackoutSequence());
    }

    private IEnumerator BlackoutSequence()
    {
        // Request screen fade (will be handled by UI controller)
        GameEventBus.TriggerShrineInteractionStarted(questData);

        // Wait for animation time
        yield return new WaitForSeconds(fadeInOutDuration);

        // Trigger local and global events
        OnShrineRestored?.Invoke();
        GameEventBus.TriggerShrineRestored(questData);

        // Wait a bit before fading back
        yield return new WaitForSeconds(0.5f);
    }

    private void UpdateShrineVisuals()
    {
        // Update visual appearance based on quest state
        if (shrineAnimator != null)
        {
            shrineAnimator.SetBool("IsLocked", questData.currentState == ShrineQuestData.QuestState.Locked);
            shrineAnimator.SetBool("IsRestored", questData.currentState == ShrineQuestData.QuestState.Restored);
            shrineAnimator.SetBool("HasArtifact", questData.currentState == ShrineQuestData.QuestState.Ready);
        }
    }

    // Visual debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}