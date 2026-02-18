using UnityEngine;

public class PlayerCollectibles : MonoBehaviour
{
    private const int TOTAL_COLLECTIBLES = 5;

    private int collectedCount = 0;

    public void Collect()
    {
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
}