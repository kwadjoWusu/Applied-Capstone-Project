using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Take Damage Effect")]
public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Damage Values")]
    public float physicalDamage = 10f;
    public float magicDamage = 0f;
    public float natureDamage = 0f;
    public float poiseDamage = 30f;

    public float hitAnimationDuration;

    [Header("Effect Settings")]
    public Vector3 contactPoint; // Where the hit occurred
    public CharacterManager characterCausingDamage; // Who caused the damage
    public GameObject bloodEffectPrefab; // Reference to blood effect prefab

    [Header("Camera Effects")]
    public float cameraShakeDuration = 0.1f;
    public float cameraShakeMagnitude = 2.0f;
    public float hitStopDuration = 0.05f;
    public float hitStopTimeScale = 0.2f;

    public override void ProcessEffect(CharacterManager character)
    {
        if (character == null)
        {
            // This handles generic visual effects without a specific character target
            SpawnBloodEffect(contactPoint);
            return;
        }

        // Skip processing if character is already dead
        if (character.isDead.Value)
            return;

        // Calculate direction of hit (for directional animations)
        // Direction is based on where the hit came from relative to the character
        Vector3 hitDirection = Vector3.zero;
        if (characterCausingDamage != null)
        {
            hitDirection = character.transform.position - characterCausingDamage.transform.position;
        }
        else if (contactPoint != Vector3.zero)
        {
            hitDirection = character.transform.position - contactPoint;
        }

        // Calculate hit angle for directional animation
        float hitAngle = CalculateHitAngle(character.transform.forward, hitDirection);
        string hitAnimation = HitAnimationMapper.GetHitAnimation(hitAngle);

        Debug.Log($"[TakeDamageEffect] Hit from angle: {hitAngle}, playing animation: {hitAnimation}");

        // Visual effects - spawn blood at contact point
        SpawnBloodEffect(contactPoint);

        // Camera effects - only apply if the player was hit
        PlayerManager playerManager = character as PlayerManager;
        if (playerManager != null && playerManager.IsOwner)
        {
            // Apply camera shake
            if (CameraShakeManager.Instance != null)
            {
                CameraShakeManager.Instance.Shake(cameraShakeDuration, cameraShakeMagnitude);
            }

            // Apply hit stop effect
            if (HitStopManager.Instance != null)
            {
                HitStopManager.Instance.Play(hitStopDuration, hitStopTimeScale);
            }
        }

        // Play hit animation on the character
        character.characterAnimatorManager.PlayTargetAnimation(hitAnimation, true, false, false, false);
    }

    private float CalculateHitAngle(Vector3 characterForward, Vector3 hitDirection)
    {
        // Ignore Y component for more accurate calculation
        hitDirection.y = 0;
        characterForward.y = 0;

        // Calculate the angle between character forward and hit direction
        float angleInDegrees = Vector3.SignedAngle(characterForward, hitDirection, Vector3.up);

        // Convert to 0-360 range
        return (angleInDegrees + 360) % 360;
    }

    private void SpawnBloodEffect(Vector3 position)
    {
        // Only spawn blood if we have a blood prefab and valid position
        if (bloodEffectPrefab != null && position != Vector3.zero)
        {
            GameObject bloodInstance = Object.Instantiate(bloodEffectPrefab, position, Quaternion.identity);

            // Destroy the blood effect after a short time
            Object.Destroy(bloodInstance, 5f);
        }
    }
}