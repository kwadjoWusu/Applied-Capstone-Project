using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffectsManager : CharacterEffectsManager
{
    private EnemyManager enemy;
    private EnemyAnimatorManager eAM;
    private EnemyAIController eAI;

    [Header("Enemy Hit Effect Settings")]
    [SerializeField] private float hitAnimationDuration = 0.5f;
    [SerializeField] private float damageFeedbackDelay = 0.2f;

    // How intense should the camera shake be on enemy hit?
    [Header("Hit Feedback Settings")]
    [SerializeField] private float cameraShakeMultiplier = 1f;
    [SerializeField] private float hitStopMultiplier = 1f;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<EnemyManager>();
        eAM   = GetComponent<EnemyAnimatorManager>();
        eAI   = GetComponent<EnemyAIController>();
    }

    protected override void ProcessTakeDamageEffect(TakeDamageEffect damageEffect)
    {
        if (enemy == null || enemy.isDead)
            return;

        // 1) Calculate damage
        float totalDamage = damageEffect.physicalDamage
                          + damageEffect.magicDamage
                          + damageEffect.natureDamage;

        Debug.Log($"[EnemyEffectsManager] Taking damage: {totalDamage}", this);

        // 2) Compute hit direction & animation
        Vector3 hitDir = damageEffect.characterCausingDamage != null
            ? transform.position - damageEffect.characterCausingDamage.transform.position
            : transform.position - damageEffect.contactPoint;

        float angle = CalculateHitAngle(transform.forward, hitDir);
        string hitAnim = HitAnimationMapper.GetHitAnimation(angle);
        Debug.Log($"[EnemyEffectsManager] Hit from angle: {angle}, playing: {hitAnim}", this);

        // 3) OPTIONAL: spawn blood prefab
        if (useBloodEffects && damageEffect.bloodEffectPrefab != null)
        {
            Vector3 spawnPos = damageEffect.contactPoint == Vector3.zero
                             ? transform.position + transform.forward * 0.5f
                             : damageEffect.contactPoint;

            var blood = Instantiate(damageEffect.bloodEffectPrefab, spawnPos, Quaternion.identity);
            Destroy(blood, 5f);
        }

        // 4) **Trigger camera shake** (scaled by your multiplier)
        if (CameraShakeManager.Instance != null)
        {
            float shakeDur    = damageEffect.cameraShakeDuration * cameraShakeMultiplier;
            float shakeMag    = damageEffect.cameraShakeMagnitude * cameraShakeMultiplier;
            CameraShakeManager.Instance.Shake(shakeDur, shakeMag);
        }

        // 5) **Trigger hit stop** (scaled by your multiplier)
        if (HitStopManager.Instance != null)
        {
            float stopDur = damageEffect.hitStopDuration * hitStopMultiplier;
            HitStopManager.Instance.Play(stopDur, damageEffect.hitStopTimeScale);
        }

        // 6) Play your damage animation + delay actual health subtraction
        if (eAM != null)
        {
            StartCoroutine(DelayedDamageApplication(totalDamage, damageFeedbackDelay));
            eAM.PlayTargetAnimation(hitAnim, true, false, false, false);

            // if we were patrolling, switch to chasing
            if (eAI != null && eAI.currentState == EnemyAIController.State.Patrolling)
                eAI.currentState = EnemyAIController.State.Chasing;
        }
        else
        {
            // Fallback
            enemy.TakeDamage(totalDamage);
        }
    }

    private float CalculateHitAngle(Vector3 forward, Vector3 dir)
    {
        dir.y = forward.y = 0;
        float signed = Vector3.SignedAngle(forward, dir, Vector3.up);
        return (signed + 360f) % 360f;
    }

    private IEnumerator DelayedDamageApplication(float damage, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (enemy != null && !enemy.isDead)
            enemy.TakeDamage(damage);
    }
}
