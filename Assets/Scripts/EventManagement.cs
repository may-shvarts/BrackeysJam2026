using System;
using UnityEngine;

public abstract class EventManagement
{
    //Triggering the elevator door movement
    public static Action OnElevatorEnter;
    
    //Triggering the change between floors
    public static Action OnFloorChange;
}
