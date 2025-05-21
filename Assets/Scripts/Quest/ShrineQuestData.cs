using UnityEngine;
using System;

// Scriptable Object for shrine data (shared across scenes)
[CreateAssetMenu(fileName = "New Shrine Quest", menuName = "Quests/Shrine Quest")]
public class ShrineQuestData : ScriptableObject
{
    public string questId;
    public string lockedMessage = "Locked";
    public string missingArtifactMessage = "You need a cultural artifact to restore this shrine";
    public string readyMessage = "Press [E] to restore shrine";
    public string restoredMessage = "Restored Shrine!";

    // Reference to required artifact
    public string requiredArtifactId;
    [TextArea(2, 5)]
    public string artifactDescription;

    // Runtime state (persistent between scene loads)
    [NonSerialized] public QuestState currentState = QuestState.Locked;

    public enum QuestState { Locked, MissingArtifact, Ready, Restored }

    // Reset the runtime state when the game starts
    private void OnEnable()
    {
        // Only reset state in editor, not during gameplay
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            currentState = QuestState.Locked;
        }
        #endif
    }
}