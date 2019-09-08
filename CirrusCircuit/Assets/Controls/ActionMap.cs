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
                    ""passThrough"": true,
                    ""initialStateCheck"": true,
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
                    ""expectedControlLayout"": """",
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
                    ""name"": ""KeyBoard.WASD"",
                    ""id"": ""2d7ae6a8-9bed-41c3-865d-685413787d31"",
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
                    ""id"": ""8cf182db-55dc-4945-afc6-5369f4dde369"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""Right"",
                    ""id"": ""b7f5620c-4284-402e-8aa3-a784d23bd569"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""Left"",
                    ""id"": ""56da0287-c546-4d00-a1df-c4d2cb520c8e"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""Down"",
                    ""id"": ""e78a1cdf-3f0f-430f-8f9c-7ef9fb065121"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""GamePad.DPad"",
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
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""Down"",
                    ""id"": ""a4dd7588-d13d-4e45-babe-811fdca8129c"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""Left"",
                    ""id"": ""b4cf9297-abcf-4bdd-b7a2-6acd48e9f5a3"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""Right"",
                    ""id"": ""c5cdbc85-7a7f-41d4-8896-7204459da4b6"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""KeyBoard.Arrows"",
                    ""id"": ""64483cd5-1de2-4585-b8c9-37c2b722c732"",
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
                    ""id"": ""8ec7097e-7872-4b58-b556-cd255e4685c2"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""Down"",
                    ""id"": ""ddad3f75-6b34-47c8-a9e7-c02ba4fe328d"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""Left"",
                    ""id"": ""298e68dd-3926-4424-8831-9463ea688187"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""Right"",
                    ""id"": ""4885437d-82f9-4b0b-b074-7a7ce419b1ef"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""36531ca0-2dd3-47a1-87f1-93a9b05b7bd8"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""2D Vector"",
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
                    ""groups"": ""Keyboard.Arrow"",
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
                    ""groups"": ""Keyboard.Arrow"",
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
                    ""groups"": ""Keyboard.Arrow"",
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
                    ""groups"": ""Keyboard.Arrow"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""aac9eb53-c7cd-4349-a748-80c9e1079305"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
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
                    ""groups"": ""Keyboard"",
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
                    ""groups"": ""Keyboard"",
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
                    ""groups"": ""Keyboard"",
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
                    ""groups"": ""Gamepad"",
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
                    ""groups"": ""Keyboard.Arrow"",
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
                    ""groups"": ""Gamepad"",
                    ""action"": ""Action.1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""4525d395-53b7-4e32-ad7e-854a78fc5c1d"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard.Arrow"",
                    ""action"": ""Action.1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""c8a128a6-e3dc-4605-b86a-db21a65389b1"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.WASD"",
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
            ""name"": ""Keyboard.Arrow"",
            ""basedOn"": """",
            ""bindingGroup"": ""Keyboard.Arrow"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""basedOn"": """",
            ""bindingGroup"": ""Gamepad"",
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
            ""devices"": []
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
        private int m_KeyboardArrowSchemeIndex = -1;
        public InputControlScheme KeyboardArrowScheme
        {
            get
            {
                if (m_KeyboardArrowSchemeIndex == -1) m_KeyboardArrowSchemeIndex = asset.GetControlSchemeIndex("Keyboard.Arrow");
                return asset.controlSchemes[m_KeyboardArrowSchemeIndex];
            }
        }
        private int m_GamepadSchemeIndex = -1;
        public InputControlScheme GamepadScheme
        {
            get
            {
                if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.GetControlSchemeIndex("Gamepad");
                return asset.controlSchemes[m_GamepadSchemeIndex];
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
