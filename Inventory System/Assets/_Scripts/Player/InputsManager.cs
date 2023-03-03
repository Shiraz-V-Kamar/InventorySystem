using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;

public class InputsManager : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 Move;
    public Vector2 Look;
    public bool IsJump;
    public bool IsSprint;

    [Header("Inputs for Gun Mechanic")]
    public bool AimPressed;
    public bool ShootPressed;
    public bool ReloadPressed;

    [Header("Inventory interactions")]
    public bool InteractPressed;
    public bool OpenInventoryPressed;
    public bool UseItemPressed;
    public bool DropItemPressed;

    public bool Slot1, Slot2, Slot3, Slot4, Slot5, Slot6, Slot7;
    
    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
   /* public bool cursorLocked = true;*/
    public bool cursorInputForLook = true;

    public bool PausePressed;
    
    public static InputsManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void OnPauseGame(InputValue value)
    {
        OnPauseGame(value.isPressed);
    }

    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);

    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }


    public void OnInteract(InputValue value)
    {
        InteractionPressed(value.isPressed);
    }
    public void OnUseItem(InputValue value)
    {
        UseItem(value.isPressed);

    }

    public void OnDropItem(InputValue value)
    {
        DropItem(value.isPressed);
    }

    public void OnOpenInventory(InputValue value)
    {
        OpenInventory(value.isPressed);
    }

    public void OnSlot1(InputValue value)
    {
        Slot1Pressed(value.isPressed);
    }

    public void OnSlot2(InputValue value)
    {
        Slot2Pressed(value.isPressed);
    }
    public void OnSlot3(InputValue value)
    {
        Slot3Pressed(value.isPressed);
    }

    public void OnSlot4(InputValue value)
    {
        Slot4Pressed(value.isPressed);
    }

    public void OnSlot5(InputValue value)
    {
        Slot5Pressed(value.isPressed);
    }

    public void OnSlot6(InputValue value)
    {
        Slot6Pressed(value.isPressed);
    }

    public void OnSlot7(InputValue value)
    {
        Slot7Pressed(value.isPressed);
    }

    public void OnShoot(InputValue value)
    {
        OnShootPressed(value.isPressed);
    }

    public void OnAim(InputValue value)
    {
        OnAimPressed(value.isPressed);
    }

    public void OnReload(InputValue value)
    {
        OnReloadPressed(value.isPressed);
    }



    // Passing Values into Public fields
    private void OnPauseGame(bool isPressed)
    {
        PausePressed = isPressed;
    }
private void OnReloadPressed(bool isPressed)
    {
        ReloadPressed = isPressed;
    }
    private void OnAimPressed(bool isPressed)
    {
        AimPressed = isPressed;
    }
    private void OnShootPressed(bool isPressed)
    {
        ShootPressed = isPressed;
    }
    private void OpenInventory(bool isPressed)
    {
        OpenInventoryPressed = isPressed;
    }
    private void DropItem(bool isPressed)
    {
        DropItemPressed = isPressed;
    }
    private void UseItem(bool isPressed)
    {
        UseItemPressed = isPressed;
    }
    private void Slot1Pressed(bool isPressed)
    {
        Slot1 = isPressed;
    }
    private void Slot2Pressed(bool isPressed)
    {
        Slot2 = isPressed;
    }
    private void Slot3Pressed(bool isPressed)
    {
        Slot3 = isPressed;
    }
    private void Slot4Pressed(bool isPressed)
    {
        Slot4 = isPressed;
    }
    private void Slot5Pressed(bool isPressed)
    {
        Slot5 = isPressed;
    }
    private void Slot6Pressed(bool isPressed)
    {
        Slot6 = isPressed;
    }

    private void Slot7Pressed(bool isPressed)
    {
        Slot7 = isPressed;
    }
   
    private void InteractionPressed(bool interact)
    {
        InteractPressed = interact;
    }

    public void MoveInput(Vector2 newMoveDirection)
    {
        Move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        Look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        IsJump = newJumpState;

    }

    public void SprintInput(bool newSprintState)
    {
        IsSprint = newSprintState;

    }

    #region CursorLock 
    /*    private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }*/
    #endregion
}

