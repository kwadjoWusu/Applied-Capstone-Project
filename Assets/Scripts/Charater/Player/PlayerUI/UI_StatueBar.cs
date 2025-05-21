using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatueBar : MonoBehaviour
{
    private Slider slider; // this represents how much stamina we have
    private RectTransform rectTransform;
    // variable to scale the bar depending on stat (higher stat = longer bar across screen)
    [Header("Bar Options")]
    [SerializeField] protected bool scaleBarLengthWithStat = true;
    [SerializeField] protected float widthScaleMultiplier = 1;
    // secondry bar behind the first to act as a polish (yellow bar that shows how much an actions takes awy form the current stat)

    protected virtual void Awake(){

        slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void SetStat(int newValue){ // sets the current value when it updates 
        slider.value = newValue;
    }

    public virtual void SetMaxStat(int maxValue){ // sets the max value of the slider
        slider.maxValue = maxValue;
        slider.value = maxValue;

        if (scaleBarLengthWithStat){
            rectTransform.sizeDelta = new Vector2(widthScaleMultiplier * maxValue, rectTransform.sizeDelta.y);
            // Reset the positions of the bars
            PlayerUIManager.instance.playerUIHudManager.RefreshHUD();
        }
            

    }



}
