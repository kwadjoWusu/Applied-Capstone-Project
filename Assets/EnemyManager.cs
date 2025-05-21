// EnemyManager.cs
// EnemyManager.cs
using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public NavMeshAgent navMeshAgent;
    public Transform target;

    [Header("Status")]
    public float health = 400f;    // Fixed the typo from "heatlh" to "health"
    public bool isDead = false;
    public bool isPerformingAction = false;
    public bool canRotate = true;
    public bool canMove = true;

    [HideInInspector] public EnemyAnimatorManager enemyAnimatorManager;
    [HideInInspector] public EnemyAIController enemyAIController;

    public event Action OnEnemyDeath;

    // State tracking to prevent duplicated actions
    private bool isCurrentlyTakingDamage = false;
    private float damageStateTimer = 0f;
    private const float DAMAGE_STATE_DURATION = 1.0f; // Time to stay in damage state

    [Header("Debug")]
    [SerializeField] private bool debugManager = true;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
        enemyAIController = GetComponent<EnemyAIController>();

        if (target == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) target = go.transform;
        }
    }

    protected virtual void Update()
    {
        // Handle damage state timer
        if (isCurrentlyTakingDamage)
        {
            damageStateTimer += Time.deltaTime;
            if (damageStateTimer >= DAMAGE_STATE_DURATION)
            {
                isCurrentlyTakingDamage = false;
                damageStateTimer = 0f;

                // Only reset if we're not dead or performing another action
                if (!isDead && isPerformingAction)
                {
                    isPerformingAction = false;
                    canRotate = true;
                    canMove = true;
                }
            }
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        if (health <= 0) Die();
        else
        {
            if (!isPerformingAction)
                enemyAnimatorManager.PlayTargetAnimation("Hit", true, false, false, false);

            if (enemyAIController.currentState == EnemyAIController.State.Patrolling)
                enemyAIController.currentState = EnemyAIController.State.Chasing;
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        navMeshAgent.enabled = false;
        enemyAnimatorManager.PlayDeathAnimation();

        foreach (var c in GetComponents<Collider>())
            c.enabled = false;

        OnEnemyDeath?.Invoke();
        Destroy(gameObject, 3f);
    }
}
