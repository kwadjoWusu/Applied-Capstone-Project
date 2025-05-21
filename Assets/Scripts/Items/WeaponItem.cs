using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item
{
    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Requirements")]

    [Header("Weapon Base Damage")]
    public int physicalDamage = 0;
    public int magicDamage = 0;

    public int natureDamage = 0;


    [Header("Weapon Base Actions")]


    // IT WEAPON GUARD ABSORPTIONS (BLOCKING POWER)

    // WEAPON MODIFIERS
    //LIGHT ATTACK MODIFIER HEAVY ATTACK MODIFIER
    //CRITICAL DAMAGE MODIFIER
    //ECT

    [Header("Stamina Costs")]
    public int baseStaminaCost = 20;

    //RUNNING ATTACK STAMINA COST MODIFIER
    //LIGHT ATTACK STAMINA COST MODIFIER
    //HEAVY ATTACK STAMINA COST MODIFIER ECT

    // ITEM BASED ACTIONS (RB, RT, LB, LT)
    // ASH OF WAR

    //Bloacking Sounds


}
