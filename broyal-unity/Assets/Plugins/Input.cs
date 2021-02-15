// GENERATED AUTOMATICALLY FROM 'Assets/StaticAssets/InputConfigs/x.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMaster : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""x"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""42c6c801-8883-4534-896c-a642f7a97628"",
            ""actions"": [
                {
                    ""name"": ""MainAction"",
                    ""type"": ""Button"",
                    ""id"": ""3f8af9a2-8123-48db-afdc-f4f19bd74c09"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""263a9be3-7d22-44c3-86f2-236def1a1b21"",
                    ""expectedControlType"": ""Dpad"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""StartAction"",
                    ""type"": ""Value"",
                    ""id"": ""60f6d7f9-9ce2-4f1d-904e-5e1033ddf415"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AttackDirection"",
                    ""type"": ""Value"",
                    ""id"": ""1a0cf9b0-d1f2-4db6-b957-a75746de6355"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""FirstAction"",
                    ""type"": ""Button"",
                    ""id"": ""5e704a8f-edaf-4625-bcbd-9e8bdd35be3b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SecondAction"",
                    ""type"": ""Button"",
                    ""id"": ""87092658-0882-4c56-a4d1-aca4dfaec7ae"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ThirdAction"",
                    ""type"": ""Button"",
                    ""id"": ""a43b1b28-071c-4243-b0f7-04943bedda7c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c7aa5788-7aa6-457b-b619-f4686eefa98e"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MainAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""f3cce505-e27e-49ce-a383-4b942bde5b1c"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Up"",
                    ""id"": ""c21b4c1a-6bed-48a8-afe1-24b1c4874fdc"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Down"",
                    ""id"": ""f95f1238-56ce-4572-bf83-83343726b052"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Left"",
                    ""id"": ""4343fb60-a3a5-4d29-9f8a-d73b5b4aa99f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Right"",
                    ""id"": ""4bec3c2e-b0d2-4cdb-9c67-c97741fbfb90"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""11fbaf2b-7042-46f9-a80c-e4ceb749d8ad"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": ""NormalizeVector2"",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""b662c9bc-97bb-4c67-82c5-83d02c902aa2"",
                    ""path"": ""<Joystick>/stick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Default"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""adf011ca-3a36-405f-8e54-d2ae892c62df"",
                    ""path"": ""<Joystick>/stick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Default"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""549c2d33-5402-48bb-a401-bd7ee4ee4fcb"",
                    ""path"": ""<Joystick>/stick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Default"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""d02be0da-35e6-42be-be47-789edefef3bc"",
                    ""path"": ""<Joystick>/stick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Default"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""2eef21dd-f117-4dee-8668-ad73d268e1a8"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AttackDirection"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""cb0918ce-c658-4e5e-be38-f66bce8f6b39"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Default"",
                    ""action"": ""AttackDirection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f8c33ce0-c592-4b89-b9e6-c62a1ab6e16e"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Default"",
                    ""action"": ""AttackDirection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8b1edba1-afa5-4ba6-abb2-b73e011dd926"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Default"",
                    ""action"": ""AttackDirection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b0629554-d369-493c-8160-5de7b7f7bd64"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Default"",
                    ""action"": ""AttackDirection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""14ed112a-3940-4c36-a6e3-a5477c1ee4d6"",
                    ""path"": ""FuckinInput"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Default"",
                    ""action"": ""StartAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0725e0e3-a160-4d81-9651-8477b1b4a570"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""FirstAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aecbb0cb-40d0-4733-a7da-83ab594deabc"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SecondAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""68bcfff3-66a3-419c-b3b1-b25458d10419"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThirdAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Default"",
            ""bindingGroup"": ""Default"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": true,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": true,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": true,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Joystick>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_MainAction = m_Player.FindAction("MainAction", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_StartAction = m_Player.FindAction("StartAction", throwIfNotFound: true);
        m_Player_AttackDirection = m_Player.FindAction("AttackDirection", throwIfNotFound: true);
        m_Player_FirstAction = m_Player.FindAction("FirstAction", throwIfNotFound: true);
        m_Player_SecondAction = m_Player.FindAction("SecondAction", throwIfNotFound: true);
        m_Player_ThirdAction = m_Player.FindAction("ThirdAction", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_MainAction;
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_StartAction;
    private readonly InputAction m_Player_AttackDirection;
    private readonly InputAction m_Player_FirstAction;
    private readonly InputAction m_Player_SecondAction;
    private readonly InputAction m_Player_ThirdAction;
    public struct PlayerActions
    {
        private @InputMaster m_Wrapper;
        public PlayerActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @MainAction => m_Wrapper.m_Player_MainAction;
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @StartAction => m_Wrapper.m_Player_StartAction;
        public InputAction @AttackDirection => m_Wrapper.m_Player_AttackDirection;
        public InputAction @FirstAction => m_Wrapper.m_Player_FirstAction;
        public InputAction @SecondAction => m_Wrapper.m_Player_SecondAction;
        public InputAction @ThirdAction => m_Wrapper.m_Player_ThirdAction;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @MainAction.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMainAction;
                @MainAction.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMainAction;
                @MainAction.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMainAction;
                @Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @StartAction.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnStartAction;
                @StartAction.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnStartAction;
                @StartAction.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnStartAction;
                @AttackDirection.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttackDirection;
                @AttackDirection.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttackDirection;
                @AttackDirection.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttackDirection;
                @FirstAction.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFirstAction;
                @FirstAction.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFirstAction;
                @FirstAction.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFirstAction;
                @SecondAction.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSecondAction;
                @SecondAction.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSecondAction;
                @SecondAction.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSecondAction;
                @ThirdAction.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThirdAction;
                @ThirdAction.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThirdAction;
                @ThirdAction.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThirdAction;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MainAction.started += instance.OnMainAction;
                @MainAction.performed += instance.OnMainAction;
                @MainAction.canceled += instance.OnMainAction;
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @StartAction.started += instance.OnStartAction;
                @StartAction.performed += instance.OnStartAction;
                @StartAction.canceled += instance.OnStartAction;
                @AttackDirection.started += instance.OnAttackDirection;
                @AttackDirection.performed += instance.OnAttackDirection;
                @AttackDirection.canceled += instance.OnAttackDirection;
                @FirstAction.started += instance.OnFirstAction;
                @FirstAction.performed += instance.OnFirstAction;
                @FirstAction.canceled += instance.OnFirstAction;
                @SecondAction.started += instance.OnSecondAction;
                @SecondAction.performed += instance.OnSecondAction;
                @SecondAction.canceled += instance.OnSecondAction;
                @ThirdAction.started += instance.OnThirdAction;
                @ThirdAction.performed += instance.OnThirdAction;
                @ThirdAction.canceled += instance.OnThirdAction;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_DefaultSchemeIndex = -1;
    public InputControlScheme DefaultScheme
    {
        get
        {
            if (m_DefaultSchemeIndex == -1) m_DefaultSchemeIndex = asset.FindControlSchemeIndex("Default");
            return asset.controlSchemes[m_DefaultSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMainAction(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnStartAction(InputAction.CallbackContext context);
        void OnAttackDirection(InputAction.CallbackContext context);
        void OnFirstAction(InputAction.CallbackContext context);
        void OnSecondAction(InputAction.CallbackContext context);
        void OnThirdAction(InputAction.CallbackContext context);
    }
}
