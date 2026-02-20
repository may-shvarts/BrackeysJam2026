using System;
using UnityEngine;

public static class EventManagement
{
    // The spawn point for the current floor (updated by the elevator)
    public static Vector3 CurrentFloorSpawnPoint { get; set; }
    // The floor the elevator is currently resting on (or just arrived at).
    // Doors read this to know whether an event is relevant to them.
    public static int CurrentFloor { get; set; } = 0;
    public static int MaxFloor { get; set; } = 0;
    
    // Player walked into the elevator cabin. Fires with the floor it's resting on.
    public static Action<int> OnElevatorEnter;

    // Player walked out of the elevator cabin. Fires with the floor it's resting on.
    public static Action<int> OnElevatorExit;

    // Doors on the DEPARTURE floor should close. int = departure floor.
    public static Action<int> OnElevatorPrepareToMove;

    // Doors on the ARRIVAL floor should open. int = arrival floor.
    public static Action<int> OnElevatorArrived;
    
    //Freezing and unfreezing player's movement while up the elevator
    public static Action OnPlayerFreeze;
    public static Action OnPlayerUnfreeze;
    
    public static Action OnFirstCollectedItem;
    public static Action OnLastCollectedItem;
    
    //Player died
    public static Action OnPlayerDied;
    
    public static Action OnWin;
    
    public static Action<int> OnLivesChanged;

    public static Action RestartGame;
    
    //Gas wheel interaction
    public static Action OnGasWheelEnter;
    public static Action OnGasWheelExit;
    public static Action OnGasWheelActivated;
    
    //Light switch interactions
    public static Action OnLightHoverEnter;
    public static Action OnLightHoverExit;
    public static Action OnLightInteracted;
}