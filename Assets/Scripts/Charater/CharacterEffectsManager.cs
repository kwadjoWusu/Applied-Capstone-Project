using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    protected CharacterManager character;

    [Header("Visual Effect Settings")]
    [SerializeField] protected float hitEffectScale = 1.0f;
    [SerializeField] protected bool useBloodEffects = true;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void ProcessInstantEffects(InstantCharacterEffect instantCharacterEffect)
    {
        // Check if we got a damage effect (our specialized effect type)
        if (instantCharacterEffect is TakeDamageEffect damageEffect)
        {
            ProcessTakeDamageEffect(damageEffect);
        }
        else
        {
            // Otherwise use default processing
            instantCharacterEffect.ProcessEffect(character);
        }
    }

    protected virtual void ProcessTakeDamageEffect(TakeDamageEffect damageEffect)
    {
        // Base implementation - customize in derived classes
        damageEffect.ProcessEffect(character);
    }

}