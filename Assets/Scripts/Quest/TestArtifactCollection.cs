using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TestArtifactCollection : MonoBehaviour
{
    public string artifactIdToCollect;

    void Update()
    {
        // Press A to collect artifact
        if (Input.GetKeyDown(KeyCode.A))
        {
            QuestManager.Instance.CollectArtifact(artifactIdToCollect);
            Debug.Log($"Collected artifact: {artifactIdToCollect}");
        }
    }
}