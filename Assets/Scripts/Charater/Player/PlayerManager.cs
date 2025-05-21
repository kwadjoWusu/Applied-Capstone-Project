using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : CharacterManager
{
    [Header("Debug Menu")]
    [SerializeField] bool respawnCharacter = false;
    [SerializeField] bool switchRightWeapon = false;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;
    [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;
    [SerializeField] bool simulateHit = false;    // ← new!

    protected override void Awake()
    {
        base.Awake();

        // Do more stuff for the player
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
    }

    protected override void Update()
    {
        base.Update();
        // if we do not own this gameobject we do not control or edit it 
        if (!IsOwner)
            return;
        // handles all the characters movement
        playerLocomotionManager.HandleAllMovement();

        // Regenerate Stamina
        playerStatsManager.RegenerateStamina();

        DebugMenu();
    }

    protected override void LateUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        base.LateUpdate();
        PlayerCamera.instance.HandleAllCameraActions();
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            PlayerCamera.instance.player = this;
            PlayerInputManager.instance.player = this;
            WorldSaveGameManager.singletonInstance.player = this;

            playerNetworkManager.life.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
            playerNetworkManager.fortitude.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;

            // Make sure PlayerUIManager and its components exist before subscribing
            if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIHudManager != null)
            {
                playerNetworkManager.currHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
                playerNetworkManager.currStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
            }
            else
            {
                Debug.LogWarning("PlayerUIManager or its components not found during OnNetworkSpawn", this);
            }
            
            playerNetworkManager.currStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenerationTimer;
        }

        playerNetworkManager.currHealth.OnValueChanged += playerNetworkManager.CheckHP;
        playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
        playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        Debug.Log("ProcessDeathEvent called in PlayerManager", this);

        // Log important component references to check for nulls
        Debug.Log($"playerAnimatorManager: {(playerAnimatorManager != null ? "NOT NULL" : "NULL")}", this);
        Debug.Log($"playerNetworkManager: {(playerNetworkManager != null ? "NOT NULL" : "NULL")}", this);
        Debug.Log($"PlayerUIManager.instance: {(PlayerUIManager.instance != null ? "NOT NULL" : "NULL")}", this);

        // Handle UI death notification safely
        if (IsOwner)
        {
            // Try to find PlayerUIManager if it's null
            if (PlayerUIManager.instance == null)
            {
                // Try to find it in the scene first
                PlayerUIManager uiManager = FindObjectOfType<PlayerUIManager>();
                if (uiManager != null)
                {
                    Debug.Log("Found PlayerUIManager in scene", this);
                }
                else
                {
                    Debug.LogError("PlayerUIManager not found in scene", this);
                }
            }

            // Safe access with null checks at each level
            if (PlayerUIManager.instance != null)
            {
                if (PlayerUIManager.instance.playerUIPopUpManager != null)
                {
                    try
                    {
                        PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
                        Debug.Log("Successfully triggered death popup", this);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Error sending death popup: {e.Message}", this);
                    }
                }
                else
                {
                    Debug.LogError("playerUIPopUpManager is null during death event", this);
                }
            }
            else
            {
                Debug.LogError("PlayerUIManager.instance is null during death event", this);
            }
        }

        // Process the base death event
        yield return StartCoroutine(base.ProcessDeathEvent(manuallySelectDeathAnimation));
    }

    public override void ReviveCharacter()
    {
        base.ReviveCharacter();

        if (IsOwner)
        {
            playerNetworkManager.currHealth.Value = playerNetworkManager.maxHealth.Value;
            playerNetworkManager.currStamina.Value = playerNetworkManager.maxStamina.Value;
            //RESTORE FOCUS POINTS
            // PLAY REBIRTH EFFECTS
            playerAnimatorManager.PlayTargetAnimation("Empty", false);
        }
    }

    public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;

        currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
        currentCharacterData.xPosition = transform.position.x;
        currentCharacterData.yPosition = transform.position.y;
        currentCharacterData.zPosition = transform.position.z;

        currentCharacterData.currentHealth = playerNetworkManager.currHealth.Value;
        currentCharacterData.currentStamina = playerNetworkManager.currStamina.Value;

        currentCharacterData.fortitude = playerNetworkManager.fortitude.Value;
        currentCharacterData.life = playerNetworkManager.life.Value;
    }
    
    public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        playerNetworkManager.characterName.Value = currentCharacterData.characterName;
        Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
        transform.position = myPosition;

        playerNetworkManager.fortitude.Value = currentCharacterData.fortitude;
        playerNetworkManager.life.Value = currentCharacterData.life;

        // this will be moved when saving and loading is added
        playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnLifeLevel(playerNetworkManager.life.Value);
        playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnFortitudeLevel(playerNetworkManager.fortitude.Value);
        playerNetworkManager.currHealth.Value = currentCharacterData.currentHealth;
        playerNetworkManager.currStamina.Value = currentCharacterData.currentStamina;

        // Make sure PlayerUIManager exists before using it
        if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIHudManager != null)
        {
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
        }
        else
        {
            Debug.LogWarning("PlayerUIManager not available during data loading", this);
        }
    }

    public void EnableWeaponCollider()
    {
        Debug.Log("[PlayerManager] EnableWeaponCollider called from animation event", this);

        if (playerEquipmentManager == null)
        {
            Debug.LogError("[PlayerManager] playerEquipmentManager is null!", this);
            return;
        }

        if (playerEquipmentManager.rightWeaponManager == null)
        {
            Debug.LogError("[PlayerManager] rightWeaponManager is null!", this);
            return;
        }

        // Otherwise use the weapon manager's method
        playerEquipmentManager.rightWeaponManager.EnableDamageCollider();
        Debug.Log("[PlayerManager] Called EnableDamageCollider on weapon manager", this);
    }

    public void DisableWeaponCollider()
    {
        Debug.Log("[PlayerManager] DisableWeaponCollider called from animation event", this);

        if (playerEquipmentManager == null || playerEquipmentManager.rightWeaponManager == null)
        {
            return;
        }
        // Otherwise use the weapon manager's method
        playerEquipmentManager.rightWeaponManager.DisableDamageCollider();
    }

    // DEBUG DELETE LATER
    private void DebugMenu()
    {
        // ——— Simulate a directional hit ———
        if (simulateHit)
        {
            Debug.Log("[DebugMenu] simulateHit → running hit code", this);
            simulateHit = false;

            var effect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
            effect.contactPoint = transform.position + transform.forward * 2f;
            effect.characterCausingDamage = this;
            effect.ProcessEffect(this);
        }

        // ——— existing debug options ———
        if (respawnCharacter)
        {
            respawnCharacter = false;
            ReviveCharacter();
        }
        if (switchRightWeapon)
        {
            switchRightWeapon = false;
            playerEquipmentManager.SwitchRightWeapon();
        }
    }
}