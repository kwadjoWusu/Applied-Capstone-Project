using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Testing scripts
public class TestShrineUnlock : MonoBehaviour
{
    public string shrineIdToUnlock;
    
    void Update()
    {
        // Press T to test unlocking the shrine
        if (Input.GetKeyDown(KeyCode.T))
        {
            QuestManager.Instance.UnlockShrine(shrineIdToUnlock);
            Debug.Log($"Attempting to unlock shrine: {shrineIdToUnlock}");
        }
    }
}