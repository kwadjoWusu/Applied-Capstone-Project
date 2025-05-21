using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyAIController : MonoBehaviour
{
    public enum State { Patrolling, Chasing, Attacking }
    public State currentState = State.Patrolling;

    [Header("References")]
    public NavMeshAgent agent;                         // assign in inspector or fetched at Start()
    public Transform player;                           // will look up by tag
    public Animator animator;                          // Reference to the animator
    private EnemyAnimatorManager animatorManager;      // Reference to your custom animator manager

    [Header("Detection")]
    public float detectionRadius = 10f;
    [Range(0, 360)] public float viewAngle = 90f;

    [Header("Attack")]
    public float attackRange = 2f;
    public float timeBetweenAttacks = 1f;
    private float attackTimer;

    [Header("Patrol")]
    public float wanderRadius = 5f;
    public float wanderTimer = 3f;
    private float wanderTimerCounter;
    private Vector3 startPosition;

    // Movement tracking for animations
    private float currentVelocity;
    private float targetVelocity;

    // Status flags
    public bool isPerformingAction = false;
    public bool isDead = false;
    public bool canMove = true;
    public bool canRotate = true;

    void Awake()
    {
        // Find and assign components if not already assigned
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        animatorManager = GetComponent<EnemyAnimatorManager>();
    }

    void Start()
    {
        // Find player if not already assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("Found player at: " + player.position);
            }
            else
            {
                Debug.LogWarning("No player found with 'Player' tag!");
            }
        }

        // Make sure agent is properly configured
        if (agent != null)
        {
            agent.stoppingDistance = attackRange * 0.8f; // Set stopping distance less than attack range
            agent.autoBraking = true;                    // Enable braking
            Debug.Log($"NavMeshAgent initialized. Speed: {agent.speed}, StoppingDistance: {agent.stoppingDistance}");
        }
        else
        {
            Debug.LogError("NavMeshAgent component missing on enemy!");
        }

        startPosition = transform.position;
        wanderTimerCounter = wanderTimer;
        attackTimer = 0f; // Initialize attack timer

        Debug.Log($"Enemy initialized at {startPosition}");
    }

    void Update()
    {
        // Early returns to prevent null reference errors
        if (isDead) return;
        if (agent == null) return;
        if (animator == null) return;

        // Debug.Log("EnemyAIController Update - Line 67");

        // Handle action state
        if (isPerformingAction)
        {
            // Ensure agent stops moving when performing an action
            if (agent.enabled) agent.velocity = Vector3.zero;
            UpdateAnimationsBasedOnMovement();
            return;
        }

        // Force check for NavMesh issues
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("Enemy is not on NavMesh! Attempting to place on NavMesh...");
            TryPlaceOnNavMesh();
            return;
        }

        // State machine
        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                if (CanSeePlayer())
                {
                    Debug.Log("Spotted player! Changing to Chase state");
                    currentState = State.Chasing;
                }
                break;

            case State.Chasing:
                Chase();
                break;

            case State.Attacking:
                Attack();
                break;
        }

        // Update animations based on AI state and movement
        UpdateAnimationsBasedOnMovement();
    }

    // New method to update animations based on movement
    private void UpdateAnimationsBasedOnMovement()
    {
        if (animator == null) return;

        // Calculate velocity for animation blending
        float velocity = 0;

        if (agent != null && agent.enabled && !isPerformingAction)
        {
            velocity = agent.velocity.magnitude / agent.speed;
        }

        // Smooth velocity for animation blending
        targetVelocity = velocity;
        currentVelocity = Mathf.Lerp(currentVelocity, targetVelocity, Time.deltaTime * 8f);

        // Update animator parameters
        float horizontalValue = 0; // Typically only used for strafing
        float verticalValue = currentVelocity;

        // If chasing, use higher animation values (like sprinting)
        if (currentState == State.Chasing && currentVelocity > 0.1f)
        {
            verticalValue = Mathf.Min(2.0f, verticalValue * 1.5f);
        }

        // Update animator movement values
        if (animatorManager != null)
        {
            animatorManager.UpdateAnimatorMovement(horizontalValue, verticalValue);
            animatorManager.UpdateAnimatorState(currentState, currentVelocity > 0.1f);
        }
        else
        {
            // Fallback if we don't have an animator manager
            animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
            animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
            animator.SetBool("IsAttacking", currentState == State.Attacking);
        }
    }

    // Helper method to try placing the enemy on the NavMesh if it's not
    private void TryPlaceOnNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            Debug.Log("Successfully placed enemy on NavMesh");
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;
        var dir = (player.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > detectionRadius) return false;
        if (Vector3.Angle(transform.forward, dir) > viewAngle * 0.5f) return false;

        // line-of-sight check
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, out hit, detectionRadius))
            return hit.collider.CompareTag("Player");

        return false;
    }

    void Patrol()
    {
        if (!canMove) return;
        if (agent == null) return;

        // Make sure agent is active
        agent.isStopped = false;
        agent.updatePosition = true;
        agent.updateRotation = true;

        wanderTimerCounter += Time.deltaTime;
        if (wanderTimerCounter >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(startPosition, wanderRadius, -1);

            // Check if position is valid before setting destination
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(newPos);
            }
            wanderTimerCounter = 0f;
        }
    }

    void Chase()
    {
        if (!canMove) return;
        if (player == null) { currentState = State.Patrolling; return; }
        if (agent == null) return;

        // Make sure agent is enabled and configured properly
        if (!agent.enabled)
        {
            Debug.LogWarning("Agent is disabled!");
            return;
        }

        // Force agent to be active
        agent.isStopped = false;
        agent.updatePosition = true;
        agent.updateRotation = true;

        // Check if destination can be set
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            Debug.LogWarning("Enemy not on NavMesh!");
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            Debug.Log("Switching to Attack state");
            currentState = State.Attacking;
        }
        else if (!CanSeePlayer() && dist > detectionRadius)
        {
            Debug.Log("Lost player, returning to patrol");
            currentState = State.Patrolling;
            agent.ResetPath();
        }
    }

    void Attack()
    {
        if (player == null) { currentState = State.Patrolling; return; }
        if (agent == null) return;

        // Explicitly stop moving
        agent.isStopped = true;
        agent.ResetPath();

        // Face the player
        if (canRotate)
        {
            Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                              Quaternion.LookRotation(lookPos - transform.position),
                                              agent.angularSpeed * Time.deltaTime);
        }

        attackTimer += Time.deltaTime;

        // Only attempt to attack if not already performing an action
        if (attackTimer >= timeBetweenAttacks && !isPerformingAction)
        {
            Debug.Log("Playing attack animation");

            // Use the animator manager if available
            if (animatorManager != null)
            {
                animatorManager.PlayAttackAnimation();
            }
            else
            {
                // Fallback to direct method
                PlayTargetAnimation("Attack_01", true);
            }

            attackTimer = 0f;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        // If player moves out of attack range, go back to chasing
        if (dist > attackRange + 0.5f) // Added small buffer to prevent oscillation 
        {
            Debug.Log("Player out of attack range, resuming chase");
            currentState = State.Chasing;
        }
    }

    // Helper method to play animations (similar to PlayerAnimatorManager)
    public void PlayTargetAnimation(string targetAnimation, bool setPerformingAction, bool applyRootMotion = false)
    {
        if (animator == null) return;

        animator.applyRootMotion = applyRootMotion;
        animator.CrossFade(targetAnimation, 0.2f);
        isPerformingAction = setPerformingAction;
    }

    // Method for animation events when attack hits
    public void OnAttackHit()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange)
        {
            // Here you would call a method on the player to take damage
            // Example:
            // player.GetComponent<PlayerManager>().TakeDamage(20f);
            Debug.Log("Enemy attack hit player!");
        }
    }

    // Method to handle damage
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        // Handle health here or through a health component
        // If this enemy has a health property, reduce it
        // health -= amount;

        // If health <= 0, die
        // if (health <= 0) Die();

        // Play hit animation
        PlayTargetAnimation("Damage_Front", true);

        // If hit while patrolling, start chasing
        if (currentState == State.Patrolling)
        {
            currentState = State.Chasing;
        }
    }

    // Method to handle death
    public void Die()
    {
        isDead = true;

        if (agent != null)
            agent.enabled = false;

        PlayTargetAnimation("Death", true, true);

        // Disable colliders
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }

        // Invoke death event if needed
        // OnEnemyDeath?.Invoke();

        Destroy(gameObject, 3f);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDir = UnityEngine.Random.insideUnitSphere * dist + origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDir, out navHit, dist, layermask);
        return navHit.position;
    }

    void OnDrawGizmosSelected()
    {
        // Detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        // FOV lines
        Vector3 left = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward * detectionRadius;
        Vector3 right = Quaternion.Euler(0, viewAngle * 0.5f, 0) * transform.forward * detectionRadius;
        Gizmos.DrawLine(transform.position, transform.position + left);
        Gizmos.DrawLine(transform.position, transform.position + right);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}