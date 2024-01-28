using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : Singleton<InputController>
{
    private CapybaraInputActions inputActions;
    private Dictionary<IControllable, Dictionary<InputControllerAction, Action>> inputActionMap = new Dictionary<IControllable, Dictionary<InputControllerAction, Action>>();
    private Dictionary<InputControllerAction, bool> inputActionEnabledMap = new Dictionary<InputControllerAction, bool>(){
        {InputControllerAction.Fart, true},
        {InputControllerAction.Interact, true},
        {InputControllerAction.Jump, true},
        {InputControllerAction.Move, true}
    };

    private IControllable currentControllable;
    public LastPressedKey ActiveDirectionKey {get; set;} = LastPressedKey.None;
    public bool IsJumping {get; set;} = false;
    public bool IsConsuming {get; set;} = false;
    public Vector2 Movement {get; set;} = Vector2.zero;

    protected override void Awake()
    {
        base.Awake();
        inputActions = new CapybaraInputActions();
        inputActions.Enable();
    }

    private void Start()
    {
        inputActions.Basic.Fart.started += ctx => OnFart();
        inputActions.Basic.Interact.started += ctx => OnInteract();
        inputActions.Basic.Jump.started += ctx => OnJumpPressed();
        inputActions.Basic.Jump.canceled += ctx => OnJumpReleased();
        inputActions.Basic.Consume.started += ctx => OnConsumeStarted();
        inputActions.Basic.Consume.canceled += ctx => OnConsumeReleased();
        inputActions.Basic.Move.performed += ctx => OnMovement(ctx);
    }

    public void ToggleInputActionState(InputControllerAction inputControllerAction, bool isEnabled)
    {
        inputActionEnabledMap[inputControllerAction] = isEnabled;
    }

    public void RegisterControllableActionHandler(IControllable controllable, InputControllerAction inputControllerAction, Action handler)
    {
        if (!inputActionMap.ContainsKey(controllable))
        {
            inputActionMap.Add(controllable, new Dictionary<InputControllerAction, Action>());
        }
        inputActionMap[controllable].Add(inputControllerAction, handler);
    }

    private void OnFart()
    {
        if (!inputActionEnabledMap[InputControllerAction.Fart])
        {
            return;
        }

        if (currentControllable != null && inputActionMap.ContainsKey(currentControllable))
        {
            inputActionMap[currentControllable][InputControllerAction.Fart]?.Invoke();
        }
    }

    private void OnInteract()
    {
        if (!inputActionEnabledMap[InputControllerAction.Interact])
        {
            return;
        }

        if (currentControllable != null && inputActionMap.ContainsKey(currentControllable))
        {
            inputActionMap[currentControllable][InputControllerAction.Interact]?.Invoke();
        }
    }


    public void SetCurrentControllable(IControllable controllable)
    {
        currentControllable = controllable;
    }

    public IControllable GetCurrentControllable()
    {
        return currentControllable;
    }

    private void OnJumpPressed()
    {
        if (!inputActionEnabledMap[InputControllerAction.Jump])
        {
            return;
        }
        IsJumping = true;
    }

    private void OnJumpReleased()
    {
        if (!inputActionEnabledMap[InputControllerAction.Jump])
        {
            return;
        }
        IsJumping = false;
    }

    private void OnConsumeStarted()
    {
        IsConsuming = true;
    }

    private void OnConsumeReleased()
    {
        IsConsuming = false;
    }

    private void OnMovement(InputAction.CallbackContext ctx)
    {
        if (!inputActionEnabledMap[InputControllerAction.Move])
        {
            return;
        }

        Movement = ctx.ReadValue<Vector2>();
        if (currentControllable != null && inputActionMap.ContainsKey(currentControllable))
        {
            inputActionMap[currentControllable][InputControllerAction.Move]?.Invoke();
        }
    }
}
