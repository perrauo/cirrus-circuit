// GENERATED AUTOMATICALLY FROM 'Assets/Controls/ActionMap.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Cirrus.Circuit.Controls
{
    public class ActionMap : IInputActionCollection
    {
        private InputActionAsset asset;
        public ActionMap()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""ActionMap"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""aca4cda5-f476-4668-ab26-f77843935d8f"",
            ""actions"": [
                {
                    ""name"": ""Axes.Left"",
                    ""id"": ""149cc932-d56a-4338-b185-94fd9471a705"",
                    ""expectedControlLayout"": ""Axis"",
                    ""continuous"": true,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Action.0"",
                    ""id"": ""b8f36fdc-aeae-47f1-ae24-22c832a43bd6"",
                    ""expectedControlLayout"": ""Button"",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Action.1"",
                    ""id"": ""22b556f9-b24e-489e-a3be-593ad29a0efb"",
                    ""expectedControlLayout"": ""Button"",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Gamepad.Dpad "",
                    ""id"": ""35b4b9fd-490b-46b2-a0b8-2a1e08a73ac4"",
                    ""path"": ""2DVector"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""Up"",
                    ""id"": ""16cba554-6d72-4f81-be75-03b229060bbc"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": "";Gamepad;Gamepad.Joystick.Left;Gamepad.Joystick;Gamepad.Dpad"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""Down"",
                    ""id"": ""a4dd7588-d13d-4e45-babe-811fdca8129c"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": "";Gamepad;Gamepad.Joystick.Left;Gamepad.Joystick;Gamepad.Dpad"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""Left"",
                    ""id"": ""b4cf9297-abcf-4bdd-b7a2-6acd48e9f5a3"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": "";Gamepad;Gamepad.Joystick.Left;Gamepad.Joystick;Gamepad.Dpad"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""Right"",
                    ""id"": ""c5cdbc85-7a7f-41d4-8896-7204459da4b6"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": "";Gamepad;Gamepad.Joystick.Left;Gamepad.Joystick;Gamepad.Dpad"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""Keyboard.Arrows"",
                    ""id"": ""c0a6a62a-d82f-4ab9-bbb9-78ced9b3dcbe"",
                    ""path"": ""2DVector"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""up"",
                    ""id"": ""7dc4d7bc-fb07-4000-8e1d-b005f0c88cf3"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.Arrow;Keyboard.Arrows"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""down"",
                    ""id"": ""25edc75f-7ac9-4ab1-8910-dded7dbe6d19"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.Arrow;Keyboard.Arrows"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""left"",
                    ""id"": ""23ba203e-5bae-4e7f-b8ad-a6799415e57d"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.Arrow;Keyboard.Arrows"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""right"",
                    ""id"": ""7957612c-1769-4379-ac12-38acf3e8760c"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.Arrow;Keyboard.Arrows"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""Keyboard.WASD"",
                    ""id"": ""aac9eb53-c7cd-4349-a748-80c9e1079305"",
                    ""path"": ""2DVector"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""up"",
                    ""id"": ""533e134b-da9d-4ae1-809c-575bda3ce091"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.WASD"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""down"",
                    ""id"": ""05953ed0-d742-4d6e-a6c1-102e8d32d932"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.WASD"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""left"",
                    ""id"": ""26d95925-b9ec-4ab4-81d2-d201a4489dce"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.WASD"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""right"",
                    ""id"": ""0615133a-3f77-4806-8f65-88d1c5d2e66f"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.WASD"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""50c15d99-5709-4325-82b5-69a4e7134a9f"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Keyboard.WASD"",
                    ""action"": ""Action.0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""d6f0bed5-c55d-43cf-9cba-8842591165c8"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Keyboard.Arrows"",
                    ""action"": ""Action.1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""10473732-bb47-4144-aaa0-20942f79a40b"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Keyboard.WASD"",
                    ""action"": ""Action.1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""73023437-1b7a-4363-8403-798f573f0772"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Gamepad.Joystick;Gamepad.Dpad"",
                    ""action"": ""Action.0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""b2d0cb2d-27cb-4a17-ab00-4bc2c1559246"",
                    ""path"": ""<Keyboard>/backspace"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard.Arrow;Keyboard.Arrows"",
                    ""action"": ""Action.0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""4ba46cc7-e8ff-4dd1-a354-c1c492ff7d03"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.WASD"",
                    ""action"": ""Action.0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""e5e7eae2-8067-4c27-bf78-6215e5135083"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Gamepad.Joystick;Gamepad.Dpad"",
                    ""action"": ""Action.1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard.Arrows"",
            ""basedOn"": """",
            ""bindingGroup"": ""Keyboard.Arrows"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad.Dpad"",
            ""basedOn"": """",
            ""bindingGroup"": ""Gamepad.Dpad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard.WASD"",
            ""basedOn"": """",
            ""bindingGroup"": ""Keyboard.WASD"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Player
            m_Player = asset.GetActionMap("Player");
            m_Player_AxesLeft = m_Player.GetAction("Axes.Left");
            m_Player_Action0 = m_Player.GetAction("Action.0");
            m_Player_Action1 = m_Player.GetAction("Action.1");
        }

        ~ActionMap()
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

        public ReadOnlyArray<InputControlScheme> controlSchemes
        {
            get => asset.controlSchemes;
        }

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
        private InputActionMap m_Player;
        private IPlayerActions m_PlayerActionsCallbackInterface;
        private InputAction m_Player_AxesLeft;
        private InputAction m_Player_Action0;
        private InputAction m_Player_Action1;
        public struct PlayerActions
        {
            private ActionMap m_Wrapper;
            public PlayerActions(ActionMap wrapper) { m_Wrapper = wrapper; }
            public InputAction @AxesLeft { get { return m_Wrapper.m_Player_AxesLeft; } }
            public InputAction @Action0 { get { return m_Wrapper.m_Player_Action0; } }
            public InputAction @Action1 { get { return m_Wrapper.m_Player_Action1; } }
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled { get { return Get().enabled; } }
            public InputActionMap Clone() { return Get().Clone(); }
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
                {
                    AxesLeft.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAxesLeft;
                    AxesLeft.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAxesLeft;
                    AxesLeft.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAxesLeft;
                    Action0.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction0;
                    Action0.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction0;
                    Action0.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction0;
                    Action1.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction1;
                    Action1.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction1;
                    Action1.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction1;
                }
                m_Wrapper.m_PlayerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    AxesLeft.started += instance.OnAxesLeft;
                    AxesLeft.performed += instance.OnAxesLeft;
                    AxesLeft.canceled += instance.OnAxesLeft;
                    Action0.started += instance.OnAction0;
                    Action0.performed += instance.OnAction0;
                    Action0.canceled += instance.OnAction0;
                    Action1.started += instance.OnAction1;
                    Action1.performed += instance.OnAction1;
                    Action1.canceled += instance.OnAction1;
                }
            }
        }
        public PlayerActions @Player
        {
            get
            {
                return new PlayerActions(this);
            }
        }
        private int m_KeyboardArrowsSchemeIndex = -1;
        public InputControlScheme KeyboardArrowsScheme
        {
            get
            {
                if (m_KeyboardArrowsSchemeIndex == -1) m_KeyboardArrowsSchemeIndex = asset.GetControlSchemeIndex("Keyboard.Arrows");
                return asset.controlSchemes[m_KeyboardArrowsSchemeIndex];
            }
        }
        private int m_GamepadDpadSchemeIndex = -1;
        public InputControlScheme GamepadDpadScheme
        {
            get
            {
                if (m_GamepadDpadSchemeIndex == -1) m_GamepadDpadSchemeIndex = asset.GetControlSchemeIndex("Gamepad.Dpad");
                return asset.controlSchemes[m_GamepadDpadSchemeIndex];
            }
        }
        private int m_KeyboardWASDSchemeIndex = -1;
        public InputControlScheme KeyboardWASDScheme
        {
            get
            {
                if (m_KeyboardWASDSchemeIndex == -1) m_KeyboardWASDSchemeIndex = asset.GetControlSchemeIndex("Keyboard.WASD");
                return asset.controlSchemes[m_KeyboardWASDSchemeIndex];
            }
        }
        public interface IPlayerActions
        {
            void OnAxesLeft(InputAction.CallbackContext context);
            void OnAction0(InputAction.CallbackContext context);
            void OnAction1(InputAction.CallbackContext context);
        }
    }
}
