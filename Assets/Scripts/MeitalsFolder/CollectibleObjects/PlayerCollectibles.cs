using UnityEngine;

public class PlayerCollectibles : MonoBehaviour
{
    [Header("Collectibles")]
    [SerializeField] private int collectedCount = 0;

    public void Collect()
    {
        collectedCount++;
        Debug.Log("Collected items: " + collectedCount);
    }

    public int GetCollectedCount()
    {
        return collectedCount;
    }
}