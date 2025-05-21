using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")]
    public CharacterManager characterCausingDamage; // When calculating damage this is used to check for attackers damage modifiers, effects etc.

    [Header("Hit Effects")]
    public bool applyCameraShake = true;
    public bool applyHitStop = true;
    public bool spawnBloodEffect = true;

    protected override void Awake()
    {
        base.Awake();

        // Set the character owner from the character causing damage if not already set
        if (characterOwner == null && characterCausingDamage != null)
        {
            characterOwner = characterCausingDamage;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        // Don't process if collider is disabled
        if (!damageCollider.enabled)
            return;

        // Don't damage self or owner
        if (other.transform.root == characterOwner.transform)
        {
            Debug.Log($"[MeleeWeaponDamageCollider] Ignoring self-hit: {other.name}", this);
            return;
        }

        // Prevent hitting the same target multiple times in one attack
        if (hitTargets.Contains(other.gameObject))
        {
            Debug.Log($"[MeleeWeaponDamageCollider] Already hit target: {other.name}", this);
            return;
        }

        // Check if we hit a player first
        PlayerManager player = other.GetComponent<PlayerManager>();
        if (player == null)
        {
            player = other.transform.root.GetComponent<PlayerManager>();
        }

        // If we found a player, apply damage
        if (player != null)
        {
            Debug.Log($"[MeleeWeaponDamageCollider] HIT PLAYER: {other.name} with {physicalDamage} damage", this);
            hitTargets.Add(other.gameObject);

            // Get hit position for effects
            Vector3 hitPosition = other.ClosestPoint(transform.position);

            // Create and apply hit effect
            ApplyHitEffectsToPlayer(player, hitPosition);

            // Apply damage to player
            ApplyDamageToPlayer(player);

            return;
        }

        // If not player, try to get EnemyManager
        EnemyManager enemy = other.GetComponent<EnemyManager>();
        if (enemy == null)
        {
            enemy = other.transform.root.GetComponent<EnemyManager>();
        }

        // If we found an enemy, apply damage
        if (enemy != null)
        {
            Debug.Log($"[MeleeWeaponDamageCollider] HIT ENEMY: {other.name} with {physicalDamage} damage", this);
            hitTargets.Add(other.gameObject);

            // Get hit position for effects
            Vector3 hitPosition = other.ClosestPoint(transform.position);

            // Create and apply hit effect - using new method for enemies
            ApplyHitEffectsToEnemy(enemy, hitPosition);

            // Apply damage to enemy
            enemy.TakeDamage(physicalDamage);
        }
        else
        {
            Debug.Log($"[MeleeWeaponDamageCollider] Hit non-character: {other.name}", this);

            // Create visual effect at hit point even if not hitting a character
            if (spawnBloodEffect && WorldCharacterEffectsManager.instance != null)
            {
                Vector3 hitPosition = other.ClosestPoint(transform.position);
                TakeDamageEffect effect = WorldCharacterEffectsManager.instance.CreateDamageEffect(hitPosition, 0, characterCausingDamage);
                if (effect != null)
                {
                    effect.ProcessEffect(null); // Process with null to only get visual effects
                }
            }
        }
    }

    // New method to apply damage to player
    private void ApplyDamageToPlayer(PlayerManager player)
    {
        if (player == null || player.playerNetworkManager == null)
            return;

        // Apply damage to player's health
        if (player.IsOwner)
        {
            player.playerNetworkManager.currHealth.Value -= Mathf.RoundToInt(physicalDamage);
            Debug.Log($"[MeleeWeaponDamageCollider] Applied {physicalDamage} damage to player. New health: {player.playerNetworkManager.currHealth.Value}", this);
        }
    }

    // Modified method to apply hit effects to player (uses CharacterManager)
    private void ApplyHitEffectsToPlayer(PlayerManager player, Vector3 hitPosition)
    {
        if (WorldCharacterEffectsManager.instance == null)
        {
            Debug.LogWarning("[MeleeWeaponDamageCollider] WorldCharacterEffectsManager or takeDamageEffect not found");

        }
        if (WorldCharacterEffectsManager.instance.takeDamageEffect == null)
        {
            Debug.LogWarning("[MeleeWeaponDamageCollider] WorldCharacterEffectsManager or takeDamageEffect not found");

        }

        // Create a damage effect
        TakeDamageEffect effect = WorldCharacterEffectsManager.instance.CreateDamageEffect(hitPosition, physicalDamage, characterCausingDamage);
        if (effect == null)
            return;

        // Configure hit effect settings
        effect.contactPoint = hitPosition;

        // Adjust effect settings based on our properties
        if (!applyCameraShake)
        {
            effect.cameraShakeDuration = 0;
            effect.cameraShakeMagnitude = 0;
        }

        if (!applyHitStop)
        {
            effect.hitStopDuration = 0;
        }

        // Process the effect on the target - player is a CharacterManager
        if (player.characterEffectsManager != null)
        {
            player.characterEffectsManager.ProcessInstantEffects(effect);
        }
        else
        {
            // Fallback if the target doesn't have an effects manager
            effect.ProcessEffect(player);
        }
    }

    // New method to apply hit effects to enemy
    private void ApplyHitEffectsToEnemy(EnemyManager enemy, Vector3 hitPosition)
    {
        if (WorldCharacterEffectsManager.instance == null || WorldCharacterEffectsManager.instance.takeDamageEffect == null)
        {
            Debug.LogWarning("[MeleeWeaponDamageCollider] WorldCharacterEffectsManager or takeDamageEffect not found");
            
        }

        // Create a damage effect
        TakeDamageEffect effect = WorldCharacterEffectsManager.instance.CreateDamageEffect(hitPosition, physicalDamage, characterCausingDamage);
        if (effect == null)
            return;

        // Configure hit effect settings
        effect.contactPoint = hitPosition;

        // Adjust effect settings based on our properties
        if (!applyCameraShake)
        {
            effect.cameraShakeDuration = 0;
            effect.cameraShakeMagnitude = 0;
        }

        if (!applyHitStop)
        {
            effect.hitStopDuration = 0;
        }

        // Process the effect directly on enemy
        // Since EnemyManager isn't a CharacterManager, we handle it differently
        if (enemy.GetComponent<EnemyEffectsManager>() != null)
        {
            enemy.GetComponent<EnemyEffectsManager>().ProcessInstantEffects(effect);
        }
        else
        {
            // Process with null to only get visual effects since enemy isn't a CharacterManager
            effect.ProcessEffect(null);
        }
    }
}