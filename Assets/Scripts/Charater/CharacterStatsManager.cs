using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Stamina Regeneration")]
    [SerializeField] float staminaRegenerationAmount = 2;
    private float staminaRegenerationTimer = 0;

    private float staminaTickTimer = 0;
    [SerializeField] float staminaRegenerationDelay = 2; //this is a time after an action you have to wait for you stamina to go up

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }
    protected virtual void Start() {
        
    }
    public int CalculateHealthBasedOnLifeLevel(int life)
    {

        float health = 0;

        // create an equation for how you want your stamina to be calculated
        health = life * 15;

        return Mathf.RoundToInt(health);

    }
    public int CalculateStaminaBasedOnFortitudeLevel(int fortitude)
    {

        float stamina = 0;

        // create an equation for how you want your stamina to be calculated
        stamina = fortitude * 10;

        return Mathf.RoundToInt(stamina);

    }

    public virtual void RegenerateStamina()
    {
        // only owners can edit their network varaibles
        if (!character.IsOwner)
        {
            return;
        }
        if (character.characterNetworkManager.isSprinting.Value)
        {
            return;
        }
        if (character.isPerformingAction)
        {
            return;
        }

        staminaRegenerationTimer += Time.deltaTime;

        if (staminaRegenerationTimer >= staminaRegenerationDelay)
        {
            if (character.characterNetworkManager.currStamina.Value < character.characterNetworkManager.maxStamina.Value)
            {
                staminaTickTimer += Time.deltaTime;

                if (staminaTickTimer >= 0.1)
                {
                    staminaTickTimer = 0;
                    character.characterNetworkManager.currStamina.Value += staminaRegenerationAmount;
                }
            }
        }


    }

    public virtual void ResetStaminaRegenerationTimer(float previousStaminaAmount, float newStaminaAmount)
    {
        //we only want to reset the regeneration if the action used stamina
        // we dont want to reset the regeneration if we are already regenerating stamina
        if (newStaminaAmount < previousStaminaAmount)
        {
            staminaRegenerationTimer = 0;
        }
    }
}
