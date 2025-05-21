using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : CharacterEquipmentManager
{

    PlayerManager player;
    public WeaponModelInstantiationSlot rightHandSlot;
    public WeaponModelInstantiationSlot leftHandSlot;

    [SerializeField] public WeaponManager rightWeaponManager;
    [SerializeField] public WeaponManager leftWeaponManager;

    public GameObject rightHandWeaponModel;
    public GameObject leftHandWeaponModel;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
        //GET OUR SLOTS
        InitializeWeaponSlots();
    }

    override protected void Start()
    {
        base.Start();
        LoadWeaponsOnBothHands();

    }
    private void InitializeWeaponSlots()
    {
        WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();
        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot.weaponSlot == WeaponModelSlot.RightHand)
            {
                rightHandSlot = weaponSlot;

            }
            else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHand)
            {
                leftHandSlot = weaponSlot;

            }

        }

    }

    public void LoadWeaponsOnBothHands()
    {
        LoadRightWeapon();
        LoadLeftWeapon();

    }
    // Right Weapon

    public void LoadRightWeapon()
    {
        if (player.playerInventoryManager.currentRightHandWeapon != null)
        {
            rightHandSlot.UnloadWeapon();
            rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);
            rightHandSlot.LoadWeapon(rightHandWeaponModel);
            rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
            rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
            // ASSIGN WEAPONS DAMAGE, TO ITS COLLIDER
        }

    }
    public void SwitchRightWeapon()
    {
        if (!player.IsOwner)
            return;
        player.playerAnimatorManager.PlayTargetAnimation("Swap_Right_Weapon_01", false, true, true, true);

        WeaponItem selectedWeapon = null;
        player.playerInventoryManager.rightHandWeaponIndex += 1;
        if (player.playerInventoryManager.rightHandWeaponIndex < 0 || player.playerInventoryManager.rightHandWeaponIndex > 2)
        {
            player.playerInventoryManager.rightHandWeaponIndex = 0;
            float weaponCount = 0;
            WeaponItem firstWeapon = null;
            int firstWeaponPosition = 0;
            for (int i = 0; i < player.playerInventoryManager.weaponsInRightHandSlots.Length; i++)
            {
                if (player.playerInventoryManager.weaponsInRightHandSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    weaponCount += 1;
                    if (firstWeapon = null)
                    {

                        firstWeapon = player.playerInventoryManager.weaponsInRightHandSlots[i];
                        firstWeaponPosition = 1;
                    }
                }

            }
            if (weaponCount <= 1)
            {
                player.playerInventoryManager.rightHandWeaponIndex = -1;
                selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;
                player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;

            }
            else
            {
                player.playerInventoryManager.rightHandWeaponIndex = firstWeaponPosition;
                player.playerNetworkManager.currentRightHandWeaponID.Value = firstWeapon.itemID;
            }
            return;
        }

        foreach (WeaponItem weapon in player.playerInventoryManager.weaponsInRightHandSlots)
        {
            // if the next potential weapon does not equal the unarmed weapon
            if (player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
            {
                selectedWeapon = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex];

                player.playerNetworkManager.currentRightHandWeaponID.Value = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID;
                return;
            }
        }
        if (selectedWeapon == null && player.playerInventoryManager.rightHandWeaponIndex <= 2)
        {
            SwitchRightWeapon();
        }
    }





    // Left Weapon

    public void LoadLeftWeapon()
    {
        if (player.playerInventoryManager.currentLeftHandWeapon != null)
        {
            leftHandSlot.UnloadWeapon();
            leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);
            leftHandSlot.LoadWeapon(leftHandWeaponModel);
            leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
            leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);

            // ASSIGN WEAPONS DAMAGE, TO ITS COLLIDER


        }

    }

    public void SwitchLeftWeapon()
    {

    }



}
