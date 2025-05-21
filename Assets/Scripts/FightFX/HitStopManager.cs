using System.Collections;
using UnityEngine;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Play(float duration, float timeScale)
    {
        StartCoroutine(HitStopCoroutine(duration, timeScale));
    }

    private IEnumerator HitStopCoroutine(float duration, float timeScale)
    {
        // Store original time scale
        float originalTimeScale = Time.timeScale;

        // Set time scale to create hit stop effect
        Time.timeScale = timeScale;

        // Wait for the duration in real-time (not affected by timeScale)
        yield return new WaitForSecondsRealtime(duration * 0.01f);

        // Restore original time scale
        Time.timeScale = originalTimeScale;
    }
}