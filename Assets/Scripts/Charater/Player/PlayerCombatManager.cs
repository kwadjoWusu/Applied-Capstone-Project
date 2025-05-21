using System.Collections;
using UnityEngine;

/// <summary>
/// Handles player combat: light and heavy attack combos,
/// plus jumping and rolling attack variations.
/// Ensures sword is equipped and manages weapon visibility.
/// </summary>
public class PlayerCombatManager : CharacterCombatManager
{
    private PlayerManager player;
    private PlayerEquipmentManager equipmentManager;

    [Header("Light Attack Combo Settings")]
    public string[] lightAttackAnimations = { "Light_Attack_01", "Light_Attack_02" };
    public float comboInputWindow = 2.5f;

    [Header("Heavy Attack Combo Settings")]
    public string[] heavyAttackAnimations = { "Heavy_Attack_01" };
    public float heavyComboInputWindow = 2.5f;

    [Header("Jump Attack Settings")]
    public string[] jumpAttackAnimations = { "Jump_Attack_01" };
    public float jumpComboInputWindow = 1f;
    public float jumpAttackStaminaCost = 25f;

    [Header("Attack Costs")]
    public float lightAttackStaminaCost = 20f;
    public float heavyAttackStaminaCost = 35f;

    [Header("Weapon Visibility")]
    public float weaponHideDelay = 3f;

    private Coroutine hideWeaponCoroutine;
    private int lightComboIndex = 0;
    private float lastLightAttackTime = -Mathf.Infinity;
    private int heavyComboIndex = 0;
    private float lastHeavyAttackTime = -Mathf.Infinity;
    private int jumpComboIndex = 0;
    private float lastJumpAttackTime = -Mathf.Infinity;

    private bool queueLightAttack = false;
    private bool queueHeavyAttack = false;
    private bool queueJumpAttack = false;
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
        equipmentManager = GetComponent<PlayerEquipmentManager>();
        SetWeaponActive(false);
    }

    void Update()
    {
        if (!player.isPerformingAction)
        {
            if (queueJumpAttack)
            {
                queueJumpAttack = false;
                PerformJumpAttack();
                return;
            }
            if (queueLightAttack)
            {
                queueLightAttack = false;
                PerformLightAttack();
                return;
            }
            if (queueHeavyAttack)
            {
                queueHeavyAttack = false;
                PerformHeavyAttack();
            }
        }
    }

    public bool HasSwordEquipped()
    {
        var currentWeapon = player.playerInventoryManager.currentRightHandWeapon;
        return currentWeapon != null &&
               currentWeapon.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID;
    }

    private void SetWeaponActive(bool active)
    {
        var model = equipmentManager.rightHandWeaponModel;
        if (model != null)
            model.SetActive(active);
    }

    private void ShowWeaponTemporarily()
    {
        SetWeaponActive(true);
        if (hideWeaponCoroutine != null)
            StopCoroutine(hideWeaponCoroutine);
        hideWeaponCoroutine = StartCoroutine(HideWeaponAfterDelay());
    }

    private IEnumerator HideWeaponAfterDelay()
    {
        yield return new WaitForSeconds(weaponHideDelay);
        SetWeaponActive(false);
    }

    // Queue handlers
    public void QueueLightAttack() => queueLightAttack = true;
    public void QueueHeavyAttack() => queueHeavyAttack = true;
    public void QueueJumpAttack() => queueJumpAttack = true;

    public void PerformLightAttack()
    {
        if (player.isPerformingAction || !HasSwordEquipped()) return;
        var net = player.playerNetworkManager;
        if (net.currStamina.Value < lightAttackStaminaCost) return;
        net.currStamina.Value -= lightAttackStaminaCost;
        net.currStamina.OnValueChanged += player.playerStatsManager.ResetStaminaRegenerationTimer;
        float dt = Time.time - lastLightAttackTime;
        lightComboIndex = dt <= comboInputWindow ? (lightComboIndex + 1) % lightAttackAnimations.Length : 0;
        lastLightAttackTime = Time.time;
        ShowWeaponTemporarily();
        player.playerAnimatorManager.PlayTargetAnimation(
            lightAttackAnimations[lightComboIndex], true, true, false, false);
    }

    public void PerformHeavyAttack()
    {
        if (player.isPerformingAction || !HasSwordEquipped()) return;
        var net = player.playerNetworkManager;
        if (net.currStamina.Value < heavyAttackStaminaCost) return;
        net.currStamina.Value -= heavyAttackStaminaCost;
        net.currStamina.OnValueChanged += player.playerStatsManager.ResetStaminaRegenerationTimer;
        float dt = Time.time - lastHeavyAttackTime;
        heavyComboIndex = dt <= heavyComboInputWindow ? (heavyComboIndex + 1) % heavyAttackAnimations.Length : 0;
        lastHeavyAttackTime = Time.time;
        ShowWeaponTemporarily();
        player.playerAnimatorManager.PlayTargetAnimation(
            heavyAttackAnimations[heavyComboIndex], true, true, false, false);
    }

    public void PerformJumpAttack()
    {
        if (!player.isJumping || player.isPerformingAction || !HasSwordEquipped()) return;
        var net = player.playerNetworkManager;
        if (net.currStamina.Value < jumpAttackStaminaCost) return;
        net.currStamina.Value -= jumpAttackStaminaCost;
        net.currStamina.OnValueChanged += player.playerStatsManager.ResetStaminaRegenerationTimer;
        float dt = Time.time - lastJumpAttackTime;
        jumpComboIndex = dt <= jumpComboInputWindow ? (jumpComboIndex + 1) % jumpAttackAnimations.Length : 0;
        lastJumpAttackTime = Time.time;
        ShowWeaponTemporarily();
        player.playerAnimatorManager.PlayTargetAnimation(
            jumpAttackAnimations[jumpComboIndex], true, true, false, false);
    }
}
