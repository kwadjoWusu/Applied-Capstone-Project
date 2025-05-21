using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectsManager : CharacterEffectsManager
{
    private PlayerManager player;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem bloodSplatterEffect;

    [Header("Hit Feedback Settings")]
    [SerializeField] private float cameraShakeIntensityMultiplier = 0.5f;
    [SerializeField] private float hitStopIntensityMultiplier = 2f;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    public override void ProcessInstantEffects(InstantCharacterEffect instantCharacterEffect)
    {
        // If the effect is a damage effect, process it with our specialized handling
        if (instantCharacterEffect is TakeDamageEffect damageEffect)
        {
            ProcessTakeDamageEffect(damageEffect);
        }
        else
        {
            // Otherwise use the base implementation
            base.ProcessInstantEffects(instantCharacterEffect);
        }
    }

    protected override void ProcessTakeDamageEffect(TakeDamageEffect damageEffect)
    {
        if (player == null || player.isDead.Value)
            return;

        // 1) lock out controls
        player.isPerformingAction = true;
        player.canMove = false;
        player.canRotate = false;

        // 2) play feedback
        HandleHitVisualEffects(damageEffect);
        damageEffect.ProcessEffect(player); // plays the directional hit animation

        // 3) recover after the duration of your hit animation
        float recoverTime = damageEffect.hitAnimationDuration; // you can add this field to your SO
        StartCoroutine(RecoverAfterDelay(player, recoverTime));
    }


    private void HandleHitVisualEffects(TakeDamageEffect damageEffect)
    {
        // Only apply these effects if we're the owner of this player
        if (!player.IsOwner)
            return;

        // Show optional blood splatter effect
        if (bloodSplatterEffect != null)
        {
            bloodSplatterEffect.Play();
        }
    }

    public IEnumerator RecoverAfterDelay(CharacterManager character, float delay)
    {
        yield return new WaitForSeconds(delay);
        character.isPerformingAction = false;
        character.canMove = true;
        character.canRotate = true;
    }
}