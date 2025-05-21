using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCharacterEffectsManager : MonoBehaviour
{
    public static WorldCharacterEffectsManager instance;
    
    [Header("Damage Effects")]
    public TakeDamageEffect takeDamageEffect;
    
    [Header("Visual Effects")]
    public GameObject bloodEffectPrefab;
    
    [Header("Effect Settings")]
    [SerializeField] List<InstantCharacterEffect> instantEffects;
    
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
        
        GenerateEffectIDs();
        
        // Initialize take damage effect with blood prefab
        if (takeDamageEffect != null && bloodEffectPrefab != null)
        {
            takeDamageEffect.bloodEffectPrefab = bloodEffectPrefab;
        }
    }
    
    private void GenerateEffectIDs()
    {
        for (int i = 0; i < instantEffects.Count; i++)
        {
            instantEffects[i].instantEffectID = i;
        }
    }
    
    /// <summary>
    /// Creates a damage effect at the specified position
    /// </summary>
    public TakeDamageEffect CreateDamageEffect(Vector3 contactPoint, float damage, CharacterManager attacker = null)
    {
        if (takeDamageEffect == null)
        {
            Debug.LogError("[WorldCharacterEffectsManager] Take Damage Effect is not assigned!");
            return null;
        }
        
        // Clone the ScriptableObject using Instantiate
        TakeDamageEffect effect = Instantiate(takeDamageEffect);
        
        // Set up effect properties
        effect.contactPoint = contactPoint;
        effect.physicalDamage = damage;
        effect.characterCausingDamage = attacker;
        effect.bloodEffectPrefab = bloodEffectPrefab;
        
        return effect;
    }
}