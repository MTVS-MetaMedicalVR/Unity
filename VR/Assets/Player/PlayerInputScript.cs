using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputScript : MonoBehaviour
{
    public Vector2 move;
    public Vector2 look;
    public bool sprint;

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }
    
    public void OnSprintInput(InputAction.CallbackContext context)
    {
        sprint = context.action.IsPressed();
    }
}
