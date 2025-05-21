using System.Collections;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance { get; private set; }
    
    private Transform cameraTransform;
    private Vector3 originalPosition;
    
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
        
        // Find the main camera if available
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }
    
    public void Shake(float duration, float magnitude)
    {
        if (cameraTransform != null)
        {
            StartCoroutine(ShakeCoroutine(duration, magnitude));
        }
    }
    
    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        originalPosition = cameraTransform.localPosition;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude * 0.01f;
            float y = Random.Range(-1f, 1f) * magnitude * 0.01f;
            
            cameraTransform.localPosition = originalPosition + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        cameraTransform.localPosition = originalPosition;
    }
}