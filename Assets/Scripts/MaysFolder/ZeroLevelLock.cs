using System;
using UnityEngine;

public class ZeroLevelLock : MonoBehaviour
{
    private void OnEnable()
    {
        EventManagement.OnFirstCollectedItem += OpenFirstLock;
    }
    private void OnDisable()
    {
        EventManagement.OnFirstCollectedItem += OpenFirstLock;
    }

    //TODO: the lock reappears after the game is restarted for some reason
    private void OpenFirstLock()
    {
        gameObject.SetActive(false);
    }

}
