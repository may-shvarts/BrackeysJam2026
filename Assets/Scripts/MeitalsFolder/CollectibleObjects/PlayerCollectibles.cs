using System;
using UnityEngine;

public class PlayerCollectibles : MonoBehaviour
{
    private const int TOTAL_COLLECTIBLES = 5;

    private int collectedCount = 0;

    private void OnEnable()
    {
        EventManagement.RestartGame += ResetCollectibles;
    }

    private void OnDisable()
    {
        EventManagement.RestartGame -= ResetCollectibles;
    }

    public void Collect()
    {
        EventManagement.OnItemCollected?.Invoke();
        collectedCount++;

        Debug.Log("Collected items: " + collectedCount);

        // First collected item
        if (collectedCount == 1)
        {
            EventManagement.OnFirstCollectedItem?.Invoke();
        }

        // Last collected item (all collected)
        if (collectedCount == TOTAL_COLLECTIBLES)
        {
            EventManagement.OnLastCollectedItem?.Invoke();
        }
    }

    public int GetCollectedCount()
    {
        return collectedCount;
    }

    private void ResetCollectibles()
    {
        collectedCount = 0;
    }
}