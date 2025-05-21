using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;
public class PlayerNetworkManager : CharacterNetworkManager
{

    PlayerManager player;
    public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [Header("Equipment")]
    public NetworkVariable<int> currentRightHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentLeftHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }
    public void SetNewMaxHealthValue(int oldLife, int newLife)
    {
        maxHealth.Value = player.playerStatsManager.CalculateHealthBasedOnLifeLevel(newLife);
        PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(maxHealth.Value);
        currHealth.Value = maxHealth.Value;
    }

    public void SetNewMaxStaminaValue(int oldFortitude, int newFortitude)
    {
        maxStamina.Value = player.playerStatsManager.CalculateStaminaBasedOnFortitudeLevel(newFortitude);
        PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(maxStamina.Value);

        currStamina.Value = maxStamina.Value;

    }

    public void OnCurrentRightHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));
        player.playerInventoryManager.currentRightHandWeapon = newWeapon;
        player.playerEquipmentManager.LoadRightWeapon();


    }

    public void OnCurrentLeftHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));
        player.playerInventoryManager.currentLeftHandWeapon = newWeapon;
        player.playerEquipmentManager.LoadLeftWeapon();
    }

}
