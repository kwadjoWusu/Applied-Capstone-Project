using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;

public class PlayerUIPopUpManager : MonoBehaviour
{
    [Header("YOU DIED Pop Up")]
    [SerializeField] GameObject youDiedPopUpGameObject;
    [SerializeField] TextMeshProUGUI youDiedPopUpBackgroundText;
    [SerializeField] CanvasGroup youDiedPopUpCanvasGroup;
    // Allows us to set the alpha to fade over time

    public void SendYouDiedPopUp()
    {
        youDiedPopUpGameObject.SetActive(true);
        youDiedPopUpBackgroundText.characterSpacing = 0;

        // Start directly with fading in, no need for separate Wait()
        StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 4f));
        StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedPopUpCanvasGroup, 2, 4f));
    }
    private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration)
    {
        if (duration > 0)
        {
            canvas.alpha = 0;
            float timer = 0;
            yield return null;

            while (timer < duration)
            {

                timer = timer + Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 1, duration * Time.deltaTime);
                yield return null;
            }
        }
        canvas.alpha = 1;
        yield return null;

    }

    private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvas, float duration, float delay)
    {
        if (duration > 0)
        {

            while (delay > 0)
            {
                delay = delay - Time.deltaTime;
                yield return null;
            }
            canvas.alpha = 1;
            float timer = 0;
            yield return null;

            while (timer < duration)
            {

                timer = timer + Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
                yield return null;

            }
        }
        canvas.alpha = 0;
        yield return null;
    }

}