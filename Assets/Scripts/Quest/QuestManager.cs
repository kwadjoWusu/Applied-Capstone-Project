using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton manager to persist quest data between scenes
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [SerializeField] private List<ShrineQuestData> allShrineQuests = new List<ShrineQuestData>();

    // Track collected artifacts
    [SerializeField] private List<string> collectedArtifacts = new List<string>();

    void Awake()
    {
        // Simple singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[QuestManager] Instance initialized and set to DontDestroyOnLoad");
        }
        else
        {
            Debug.Log("[QuestManager] Duplicate instance found, destroying");
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        // Listen for restored shrines
        GameEventBus.OnShrineRestored += HandleShrineRestored;
    }

    void OnDisable()
    {
        GameEventBus.OnShrineRestored -= HandleShrineRestored;
    }

    private void HandleShrineRestored(ShrineQuestData shrine)
    {
        // Save progress or perform other game-wide actions when shrines are restored
        Debug.Log($"[QuestManager] Shrine {shrine.questId} restored!");

        // Check if all shrines are restored
        CheckAllShrinesRestored();
    }

    private void CheckAllShrinesRestored()
    {
        bool allRestored = true;
        foreach (var shrine in allShrineQuests)
        {
            if (shrine.currentState != ShrineQuestData.QuestState.Restored)
            {
                allRestored = false;
                break;
            }
        }

        if (allRestored)
        {
            Debug.Log("[QuestManager] All shrines restored! Trigger game completion event!");
            // Trigger game completion here
            GameEventBus.TriggerDisplayNotification("All shrines restored! You have completed your quest!");
        }
    }

    // Public methods to access quest data
    public ShrineQuestData GetShrineQuest(string questId)
    {
        return allShrineQuests.Find(q => q.questId == questId);
    }

    public void UnlockShrine(string questId)
    {
        var shrine = GetShrineQuest(questId);
        if (shrine != null && shrine.currentState == ShrineQuestData.QuestState.Locked)
        {
            shrine.currentState = ShrineQuestData.QuestState.MissingArtifact;
            Debug.Log($"[QuestManager] Unlocked shrine: {questId}");

            // Notify all shrine instances to update
            foreach (var shrineInst in FindObjectsOfType<ShrineQuest>())
            {
                if (shrineInst.questData.questId == shrine.questId)
                {
                    Debug.Log($"[QuestManager] Notifying shrine instance to unlock: {questId}");
                    shrineInst.UpdateFromData(shrine);
                }
            }
        }
    }

    // Collect an artifact and update relevant shrines
    public void CollectArtifact(string artifactId)
    {
        Debug.Log($"[QuestManager] Attempting to collect artifact: {artifactId}");

        if (!collectedArtifacts.Contains(artifactId))
        {
            // Add the artifact to collected list
            collectedArtifacts.Add(artifactId);
            Debug.Log($"[QuestManager] Successfully collected artifact: {artifactId}");
            Debug.Log($"[QuestManager] Total artifacts: {collectedArtifacts.Count}");

            // Log all collected artifacts
            Debug.Log($"[QuestManager] All collected artifacts: {string.Join(", ", collectedArtifacts)}");

            // First update all shrine data objects that require this artifact
            foreach (var shrine in allShrineQuests)
            {
                Debug.Log($"[QuestManager] Checking shrine data {shrine.questId}, requires: {shrine.requiredArtifactId}, state: {shrine.currentState}");

                if (shrine.requiredArtifactId == artifactId)
                {
                    // Update based on current state
                    if (shrine.currentState == ShrineQuestData.QuestState.Locked)
                    {
                        Debug.Log($"[QuestManager] Unlocking shrine {shrine.questId} (was locked)");
                        shrine.currentState = ShrineQuestData.QuestState.Ready;
                    }
                    else if (shrine.currentState == ShrineQuestData.QuestState.MissingArtifact)
                    {
                        Debug.Log($"[QuestManager] Shrine {shrine.questId} is now ready!");
                        shrine.currentState = ShrineQuestData.QuestState.Ready;
                    }

                    // Update active shrine instances in the scene
                    UpdateShrineInstances(shrine);
                }
            }
        }
        else
        {
            Debug.Log($"[QuestManager] Artifact already collected: {artifactId}");
        }
    }

    // Helper method to find and update shrine instances in the scene
    private void UpdateShrineInstances(ShrineQuestData shrineData)
    {
        var shrineInstances = FindObjectsOfType<ShrineQuest>();
        Debug.Log($"[QuestManager] Looking for shrine instances to update. Found: {shrineInstances.Length}");

        bool shrineUpdated = false;
        foreach (var shrineInst in shrineInstances)
        {
            if (shrineInst.questData.questId == shrineData.questId)
            {
                Debug.Log($"[QuestManager] Found matching shrine instance: {shrineData.questId}, updating to state: {shrineData.currentState}");
                shrineInst.UpdateFromData(shrineData);
                shrineUpdated = true;
            }
        }

        // Send notification if any shrine was updated to Ready state
        if (shrineUpdated && shrineData.currentState == ShrineQuestData.QuestState.Ready)
        {
            GameEventBus.TriggerDisplayNotification($"Shrine {shrineData.questId} can now be restored!");
        }
    }

    // Method to check if player has a specific artifact
    public bool HasArtifact(string artifactId)
    {
        return collectedArtifacts.Contains(artifactId);
    }

    // Return all collected artifacts
    public List<string> GetCollectedArtifacts()
    {
        return new List<string>(collectedArtifacts);
    }

    // For testing purposes
    public void ResetProgress()
    {
        collectedArtifacts.Clear();

        foreach (var shrine in allShrineQuests)
        {
            shrine.currentState = ShrineQuestData.QuestState.Locked;
        }

        // Update all shrine instances in the scene
        foreach (var shrineInst in FindObjectsOfType<ShrineQuest>())
        {
            shrineInst.UpdateFromData(shrineInst.questData);
        }

        Debug.Log("[QuestManager] Progress reset complete");
    }
}