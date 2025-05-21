using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant effects/Take Stamina Damage")]
public class TakeStaminaDamageEffect : InstantCharacterEffect
{
    public float staminaDamage;
    public override void ProcessEffect(CharacterManager character)
    {
        CalculateStaminaDamage(character);
    }

    private void CalculateStaminaDamage(CharacterManager character)
    {
        if (character.IsOwner)
        {
            Debug.Log("Taking stamina damage" + staminaDamage + " Stamina Damage");
            character.characterNetworkManager.currStamina.Value -= staminaDamage;
        }
    }
}
