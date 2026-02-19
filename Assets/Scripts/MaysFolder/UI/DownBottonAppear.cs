using System;
using UnityEngine;
using UnityEngine.UI;

public class DownBottonAppear : MonoBehaviour
{
    private bool _playerOnElevator = false;
    private bool _elevatorMoving = false;
    private bool _collectedFirstItem = false;
    private bool _collectedLastItem = false;
    private int _currentFloor = 0;
    private int _maxFloor = 4;

    private String DOWN_BOTTON_TAG = "DownBotton"; 
    private String UP_BOTTON_TAG = "UpBotton"; 
    private Image _uiImage;
    void Start()
    {
        _uiImage = GetComponent<Image>();
        _currentFloor = EventManagement.CurrentFloor;
        UpdateVisibility(); 
    }

    private void OnEnable()
    {
        EventManagement.OnElevatorEnter += HandleElevatorEnter;
        EventManagement.OnElevatorExit += HandleElevatorExit;
        EventManagement.OnElevatorPrepareToMove += HandleElevatorPrepareToMove;
        EventManagement.OnElevatorArrived += HandleElevatorArrived;
        EventManagement.OnFirstCollectedItem += HandleFirstCollectedItem;
        EventManagement.OnLastCollectedItem += HandleLastCollectedItem;
        EventManagement.RestartGame += HandleRestart;
    }

    private void OnDisable()
    {
        EventManagement.OnElevatorEnter -= HandleElevatorEnter;
        EventManagement.OnElevatorExit -= HandleElevatorExit;
        EventManagement.OnElevatorPrepareToMove -= HandleElevatorPrepareToMove;
        EventManagement.OnElevatorArrived -= HandleElevatorArrived;
        EventManagement.OnFirstCollectedItem -= HandleFirstCollectedItem;
        EventManagement.OnLastCollectedItem -= HandleLastCollectedItem;
        EventManagement.RestartGame -= HandleRestart;
    }
    private void HandleFirstCollectedItem()
    {
        _collectedFirstItem = true;
        UpdateVisibility();
    }
    private void HandleLastCollectedItem()
    {
        _collectedLastItem = true;
        UpdateVisibility();
    }
    private void HandleElevatorEnter(int floor)
    {
        _playerOnElevator = true;
        _currentFloor =  floor;
        UpdateVisibility();
    }

    private void HandleElevatorExit(int floor)
    {
        _playerOnElevator = false;
        _currentFloor =  floor;
        UpdateVisibility();
    }

    private void HandleElevatorPrepareToMove(int floor)
    {
        _elevatorMoving = true;
        _currentFloor =  floor;
        UpdateVisibility();
    }

    private void HandleElevatorArrived(int floor)
    {
        _elevatorMoving = false;
        _currentFloor =  floor;
        UpdateVisibility();
    }


    private void UpdateVisibility()
    {
        if (_playerOnElevator && !_elevatorMoving)
        {
            if (this.gameObject.CompareTag(DOWN_BOTTON_TAG) && _currentFloor > 0)
            {
                ShowObject();
            }

            else if (this.gameObject.CompareTag(UP_BOTTON_TAG) && _currentFloor <= _maxFloor)
            {
                if(_currentFloor != 0 && _currentFloor != _maxFloor)
                    ShowObject();
                else if(_currentFloor == _maxFloor && _collectedLastItem)
                    ShowObject();
                else if(_currentFloor == 0 && _collectedFirstItem)
                    ShowObject();
                else
                    HideObject();
            }
        }
        else
        {
            HideObject();
        }
    }

    private void HideObject()
    {
        if (_uiImage != null && _uiImage.enabled)
        {
            _uiImage.enabled = false;
        }
    }

    private void ShowObject()
    {
        if (_uiImage != null && !_uiImage.enabled)
        {
            _uiImage.enabled = true;
        }
    }
    
    private void HandleRestart()
    {
        _playerOnElevator = false;
        _elevatorMoving = false;
        _collectedFirstItem = false;
        _collectedLastItem = false;
        
        _currentFloor = 0; 

        UpdateVisibility();
    }
}