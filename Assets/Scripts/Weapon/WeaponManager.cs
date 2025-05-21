using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour 
{
    [SerializeField] MeleeWeaponDamageCollider meleeDamageCollider;
    
    private void Awake()
    {
        // Try to find the damage collider if not assigned
        if (meleeDamageCollider == null)
        {
            meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
            
            // Log warning if still not found
            if (meleeDamageCollider == null)
            {
                Debug.LogWarning($"No MeleeWeaponDamageCollider found on weapon {gameObject.name}", this);
            }
            else
            {
                Debug.Log($"[WeaponManager] Found damage collider on {meleeDamageCollider.gameObject.name}", this);
            }
        }
    }
    
    public void SetWeaponDamage(CharacterManager characterWieldingWeapon, WeaponItem weapon)
    {
        if (meleeDamageCollider != null)
        {
            // Set both the character causing damage and the owner
            meleeDamageCollider.characterCausingDamage = characterWieldingWeapon;
            meleeDamageCollider.characterOwner = characterWieldingWeapon;
            
            // Set damage values from weapon
            meleeDamageCollider.physicalDamage = weapon.physicalDamage;
            meleeDamageCollider.magicDamage = weapon.magicDamage;
            meleeDamageCollider.natureDamage = weapon.natureDamage;
            
            Debug.Log($"[WeaponManager] Set {characterWieldingWeapon.name}'s weapon damage to {weapon.physicalDamage}", this);
        }
        else
        {
            Debug.LogError($"[WeaponManager] Cannot set weapon damage - no damage collider on {gameObject.name}", this);
        }
    }
    
    // Helper method to expose the EnableDamageCollider function directly
    public void EnableDamageCollider()
    {
        if (meleeDamageCollider != null)
        {
            meleeDamageCollider.EnableDamageCollider();
        }
        else
        {
            Debug.LogError("Cannot enable damage collider - none found!", this);
        }
    }
    
    // Helper method to expose the DisableDamageCollider function directly
    public void DisableDamageCollider()
    {
        if (meleeDamageCollider != null)
        {
            meleeDamageCollider.DisableDamageCollider();
        }
    }
}