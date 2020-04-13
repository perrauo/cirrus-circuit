// GENERATED AUTOMATICALLY FROM 'Assets/Controls/ActionMap.inputactions'

using System.Collections;
using System.Collections.Generic;
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
                    ""type"": ""Value"",
                    ""id"": ""149cc932-d56a-4338-b185-94fd9471a705"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Action.0"",
                    ""type"": ""Button"",
                    ""id"": ""b8f36fdc-aeae-47f1-ae24-22c832a43bd6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Action.1"",
                    ""type"": ""Button"",
                    ""id"": ""22b556f9-b24e-489e-a3be-593ad29a0efb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
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
                    ""isPartOfComposite"": false
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
                    ""isPartOfComposite"": true
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
                    ""isPartOfComposite"": true
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
                    ""isPartOfComposite"": true
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
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard.Arrows"",
                    ""id"": ""c0a6a62a-d82f-4ab9-bbb9-78ced9b3dcbe"",
                    ""path"": ""2DVector"",
                    ""interactions"": ""Hold(duration=10)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
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
                    ""isPartOfComposite"": true
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
                    ""isPartOfComposite"": true
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
                    ""isPartOfComposite"": true
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
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard.WASD"",
                    ""id"": ""57c9370e-1587-4b31-8cc9-fb84501c7c10"",
                    ""path"": ""2DVector"",
                    ""interactions"": ""Hold(duration=10)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ff4e9725-2d41-43d9-b76d-0f24c8432991"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.WASD"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""91fd13e9-e63e-47a9-a46f-b902ad6fb402"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.WASD"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""237ce53a-c36d-457c-9677-84f2f4a63451"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.WASD"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b0387e29-09ca-4905-b6c6-a417afbfea83"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.WASD"",
                    ""action"": ""Axes.Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""50c15d99-5709-4325-82b5-69a4e7134a9f"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Keyboard.WASD"",
                    ""action"": ""Action.0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""73023437-1b7a-4363-8403-798f573f0772"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Gamepad.Joystick;Gamepad.Dpad"",
                    ""action"": ""Action.0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b2d0cb2d-27cb-4a17-ab00-4bc2c1559246"",
                    ""path"": ""<Keyboard>/backspace"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.Arrow;Keyboard.Arrows"",
                    ""action"": ""Action.0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e5e7eae2-8067-4c27-bf78-6215e5135083"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Gamepad.Joystick;Gamepad.Dpad"",
                    ""action"": ""Action.1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""091dc393-1d52-4102-b97a-8ec75f95f792"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.Arrows"",
                    ""action"": ""Action.1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9880bad7-328f-4e28-b655-0e344621fe3a"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard.WASD"",
                    ""action"": ""Action.1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard.Arrows"",
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
        private readonly InputAction m_Player_AxesLeft;
        private readonly InputAction m_Player_Action0;
        private readonly InputAction m_Player_Action1;
        public struct PlayerActions
        {
            private ActionMap m_Wrapper;
            public PlayerActions(ActionMap wrapper) { m_Wrapper = wrapper; }
            public InputAction @AxesLeft => m_Wrapper.m_Player_AxesLeft;
            public InputAction @Action0 => m_Wrapper.m_Player_Action0;
            public InputAction @Action1 => m_Wrapper.m_Player_Action1;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
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
        public PlayerActions @Player => new PlayerActions(this);
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
