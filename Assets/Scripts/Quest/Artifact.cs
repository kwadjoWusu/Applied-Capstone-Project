using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Artifact collectible class with improved collection handling
public class Artifact : MonoBehaviour
{
    public string artifactId;
    public string artifactName;
    [TextArea(3, 10)]
    public string description;
    public GameObject visualModel;
    public AudioClip collectSound;
    public float collectEffectDuration = 1.0f;
    public ParticleSystem collectEffect;

    [Header("Clue Document")]
    public bool hasDocument = false;
    public string documentTitle;
    [TextArea(5, 20)]
    public string documentContent;

    private bool isBeingCollected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isBeingCollected)
        {
            StartCoroutine(CollectArtifact());
        }
    }

    private IEnumerator CollectArtifact()
    {
        isBeingCollected = true;
        Debug.Log($"[Artifact] Player collecting artifact: {artifactId} - {artifactName}");

        // Play collect effect if available
        if (collectEffect != null)
        {
            collectEffect.Play();
        }

        // Play collect sound if available
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Show notification
        GameEventBus.TriggerDisplayNotification($"Collected: {artifactName}");
        
        // Wait for effect duration
        yield return new WaitForSeconds(collectEffectDuration * 0.5f);
        
        // Disable visuals but keep object alive for document display
        if (visualModel != null)
        {
            visualModel.SetActive(false);
        }
        
        // Collect the artifact through the quest manager
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.CollectArtifact(artifactId);
        }
        else
        {
            Debug.LogError("[Artifact] QuestManager instance is null!");
        }

        // Show document if available - with slight delay for better UX
        yield return new WaitForSeconds(collectEffectDuration * 0.5f);
        
        if (hasDocument)
        {
            GameEventBus.TriggerDisplayDocument(documentTitle, documentContent, artifactId);
            
            // Wait for document to be closed before destroying
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0));
            
            // Hide document
            GameEventBus.TriggerHideDocument();
        }

        // Destroy the collectible
        Destroy(gameObject);
    }
}