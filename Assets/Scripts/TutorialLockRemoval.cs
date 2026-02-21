using System;
using DG.Tweening;
using UnityEngine;

public class TutorialLockRemoval : MonoBehaviour
{
    [SerializeField] private float lockRemovalDelay = 2f;
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
    {/*
        DOVirtual.DelayedCall(lockRemovalDelay, () =>
        {
            this.gameObject.SetActive(false);
        });*/
        this.gameObject.SetActive(false);
    }
}
