using System;
using UnityEngine;

public class TutorialLockRemoval : MonoBehaviour
{
    private void Awake()
    {
        EventManagement.OnFirstCollectedItem += DisableLock;
        EventManagement.RestartGame += EnableLock;
    }

    private void OnDestroy()
    {
        EventManagement.OnFirstCollectedItem -= DisableLock;
        EventManagement.RestartGame -= EnableLock;
    }

    private void EnableLock()
    {
        this.gameObject.SetActive(true);
    }
    private void DisableLock()
    {
        this.gameObject.SetActive(false);
    }
}
