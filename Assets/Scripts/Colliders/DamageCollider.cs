using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour 
{
    [Header("Damage Stats")]
    public float physicalDamage = 10f;
    public float magicDamage = 0f;
    public float natureDamage = 0f;
    public float poiseDamage = 30f;
    
    [Header("Collider")]
    protected Collider damageCollider;
    
    [Header("References")]
    public CharacterManager characterOwner;
    
    // Using object instead of CharacterManager to track hit targets
    protected List<GameObject> hitTargets = new List<GameObject>();
    
    protected virtual void Awake()
    {
        damageCollider = GetComponent<Collider>();
        DisableDamageCollider();
        
        if (damageCollider == null)
        {
            Debug.LogError("No collider found on damage collider object!", this);
        }
    }
    
    public virtual void EnableDamageCollider()
    {
        if (damageCollider != null)
        {
            damageCollider.enabled = true;
            hitTargets.Clear(); // Clear hit targets when enabling
            Debug.Log($"[DamageCollider] Enabled on {gameObject.name}", this);
        }
        else
        {
            Debug.LogError("No collider found on damage collider!", this);
        }
    }
    
    public virtual void DisableDamageCollider()
    {
        if (damageCollider != null)
        {
            damageCollider.enabled = false;
            hitTargets.Clear();
            Debug.Log($"[DamageCollider] Disabled on {gameObject.name}", this);
        }
    }
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        // Basic collision logic is handled in derived classes
        Debug.Log($"[DamageCollider] Base trigger hit: {other.name}", this);
    }
}