//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Scripts/CapybaraInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @CapybaraInputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @CapybaraInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""CapybaraInputActions"",
    ""maps"": [
        {
            ""name"": ""Basic"",
            ""id"": ""b64489e5-bdc1-42f0-a09e-6d0c7dcad465"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""35d28390-5c48-4e30-a6e3-d502a6f7ee63"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Fart"",
                    ""type"": ""Button"",
                    ""id"": ""e65bb070-d1dc-4587-942c-7b72f2e588d9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""279dc97f-6a90-438e-bbbc-1de8360fbc2e"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""01a9c937-3d0a-4135-8221-00955ab85bfc"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""658a5c2b-1d0b-49b1-9bfa-baaf3c7ba176"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Basic
        m_Basic = asset.FindActionMap("Basic", throwIfNotFound: true);
        m_Basic_Move = m_Basic.FindAction("Move", throwIfNotFound: true);
        m_Basic_Fart = m_Basic.FindAction("Fart", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Basic
    private readonly InputActionMap m_Basic;
    private List<IBasicActions> m_BasicActionsCallbackInterfaces = new List<IBasicActions>();
    private readonly InputAction m_Basic_Move;
    private readonly InputAction m_Basic_Fart;
    public struct BasicActions
    {
        private @CapybaraInputActions m_Wrapper;
        public BasicActions(@CapybaraInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Basic_Move;
        public InputAction @Fart => m_Wrapper.m_Basic_Fart;
        public InputActionMap Get() { return m_Wrapper.m_Basic; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BasicActions set) { return set.Get(); }
        public void AddCallbacks(IBasicActions instance)
        {
            if (instance == null || m_Wrapper.m_BasicActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_BasicActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Fart.started += instance.OnFart;
            @Fart.performed += instance.OnFart;
            @Fart.canceled += instance.OnFart;
        }

        private void UnregisterCallbacks(IBasicActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Fart.started -= instance.OnFart;
            @Fart.performed -= instance.OnFart;
            @Fart.canceled -= instance.OnFart;
        }

        public void RemoveCallbacks(IBasicActions instance)
        {
            if (m_Wrapper.m_BasicActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IBasicActions instance)
        {
            foreach (var item in m_Wrapper.m_BasicActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_BasicActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public BasicActions @Basic => new BasicActions(this);
    public interface IBasicActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnFart(InputAction.CallbackContext context);
    }
}