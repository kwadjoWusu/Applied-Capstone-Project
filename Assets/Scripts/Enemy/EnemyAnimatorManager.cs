using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorManager : MonoBehaviour
{
    EnemyManager enemy;

    int vertical;
    int horizontal;
    int isAttacking;
    int isDead;

    protected virtual void Awake()
    {
        enemy = GetComponent<EnemyManager>();

        // Set up animator hash IDs
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
        isAttacking = Animator.StringToHash("IsAttacking");
        isDead = Animator.StringToHash("IsDead");
    }

    public void UpdateAnimatorMovement(float horizontalValue, float verticalValue)
    {
        float horizontalAmount = horizontalValue;
        float verticalAmount = verticalValue;

        // Update animator parameters with smoothing
        enemy.animator.SetFloat(horizontal, horizontalAmount, 0.1f, Time.deltaTime);
        enemy.animator.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);
    }

    public void UpdateAnimatorState(EnemyAIController.State currentState, bool isMoving)
    {
        // Reset all state booleans
        enemy.animator.SetBool(isAttacking, false);

        switch (currentState)
        {
            case EnemyAIController.State.Patrolling:
                UpdateAnimatorMovement(1.1f, 0.2f);
                // Use movement parameters only
                break;

            case EnemyAIController.State.Chasing:
                UpdateAnimatorMovement(1.1f, 1.5f);
                // Use movement parameters with higher values for running animation
                break;

            case EnemyAIController.State.Attacking:
                enemy.animator.SetBool(isAttacking, true);
                break;
        }
    }

    public virtual void PlayTargetAnimation(string targetAnimation, bool isPerformingAction, bool applyRootMotion = true, bool canRotate = false, bool canMove = false)
    {
        if (enemy.animator == null)
        {
            Debug.LogError("Animator component is missing!", this);
            return;
        }

        enemy.animator.applyRootMotion = applyRootMotion;
        enemy.animator.CrossFade(targetAnimation, 0.2f);

        // Set flags for AI control
        enemy.isPerformingAction = isPerformingAction;
        enemy.canRotate = canRotate;
        enemy.canMove = canMove;
    }

    public void PlayDeathAnimation()
    {
        PlayTargetAnimation("Death", true, true, false, false);
    }

    public void PlayAttackAnimation()
    {
        // Choose from multiple attack animations if available
        PlayTargetAnimation("Attack", true, false, false, false);
    }

    // AnimationEvent at end of attack (no parameters)
    public void ResetActionFlag()
    {
        Debug.Log("[EnemyAnimatorManager] Animation Event: ResetActionFlag", this);

        // allow AI to run its Update() again
        enemy.isPerformingAction = false;
        enemy.canMove = true;
        enemy.canRotate = true;

        // if you want it to immediately resume chasing:
        enemy.enemyAIController.currentState = EnemyAIController.State.Chasing;
    }

}