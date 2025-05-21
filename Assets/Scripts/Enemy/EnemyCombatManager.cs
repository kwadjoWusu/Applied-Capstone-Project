using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatManager : CharacterCombatManager
{
    private EnemyManager enemyManager;
    
    [Header("Weapon Colliders")]
    public DamageCollider mainWeaponCollider; // Reference to the enemy's weapon collider
    
    [Header("Attack Settings")]
    public float basePhysicalDamage = 10f;
    public float baseMagicDamage = 0f;
    public float baseNatureDamage = 0f;
    
    protected override void Awake()
    {
        base.Awake();
        enemyManager = GetComponent<EnemyManager>();
        
        // Initialize weapon collider if assigned
        if (mainWeaponCollider != null)
        {
            // Set damage values
            mainWeaponCollider.physicalDamage = basePhysicalDamage;
            mainWeaponCollider.magicDamage = baseMagicDamage;
            mainWeaponCollider.natureDamage = baseNatureDamage;
            
            // Fix: Ensure EnemyManager inherits from CharacterManager or get the CharacterManager component
            CharacterManager characterManager = GetComponent<CharacterManager>();
            if (characterManager == null)
            {
                Debug.LogError("[EnemyCombatManager] No CharacterManager component found on this GameObject!", this);
            }
            
            mainWeaponCollider.characterOwner = characterManager;
            
            // If it's a melee weapon collider, set the character causing damage
            if (mainWeaponCollider is MeleeWeaponDamageCollider meleeCollider)
            {
                meleeCollider.characterCausingDamage = characterManager;
                Debug.Log($"[EnemyCombatManager] Set up melee weapon collider with damage: {basePhysicalDamage}", this);
            }
            
            // Make sure it starts disabled
            DisableWeaponCollider();
        }
        else
        {
            Debug.LogWarning("[EnemyCombatManager] No weapon collider assigned to enemy!", this);
        }
    }
    
    // Called via animation events during attack animations
    public void EnableWeaponCollider()
    {
        if (mainWeaponCollider != null)
        {
            Debug.Log("[EnemyCombatManager] Enabling weapon collider", this);
            mainWeaponCollider.EnableDamageCollider();
        }
    }
    
    // Called via animation events during attack animations
    public void DisableWeaponCollider()
    {
        if (mainWeaponCollider != null)
        {
            Debug.Log("[EnemyCombatManager] Disabling weapon collider", this);
            mainWeaponCollider.DisableDamageCollider();
        }
    }
}
