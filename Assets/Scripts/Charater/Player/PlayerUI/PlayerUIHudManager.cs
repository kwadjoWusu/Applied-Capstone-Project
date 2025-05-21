using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIHudManager : MonoBehaviour
{
    [SerializeField] UI_StatueBar staminaBar;
    [SerializeField] UI_StatueBar healthBar;

    public void RefreshHUD()
    {
        healthBar.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(false);
        staminaBar.gameObject.SetActive(true);




    }


    public void SetNewStaminaValue(float oldValue, float newValue)
    {

        staminaBar.SetStat(Mathf.RoundToInt(newValue));
    }

    public void SetMaxStaminaValue(int maxStamina)
    {
        staminaBar.SetMaxStat(maxStamina);
    }

    public void SetNewHealthValue(int oldValue, int newValue)
    {

        healthBar.SetStat(newValue);
    }

    public void SetMaxHealthValue(int maxHealth)
    {
        healthBar.SetMaxStat(maxHealth);
    }


}
