// GENERATED AUTOMATICALLY FROM 'Assets/Controls/Schema.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Cirrus.GemCircuit.Controls
{
    public class Schema : IInputActionCollection
    {
        private InputActionAsset asset;
        public Schema()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Schema"",
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
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Axes.Right"",
                    ""id"": ""14148ecb-7fa7-4452-8af8-a1172024886b"",
                    ""expectedControlLayout"": ""Axis"",
                    ""continuous"": true,
                    ""passThrough"": true,
                    ""initialStateCheck"": true,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Jump"",
                    ""id"": ""b8f36fdc-aeae-47f1-ae24-22c832a43bd6"",
                    ""expectedControlLayout"": ""Button"",
                    ""continuous"": false,
                    ""passThrough"": true,
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
                    ""name"": """",
                    ""id"": ""974c9817-cb20-41cf-a1ac-5bbd2a70924a"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""c5c305e5-7130-4171-8c98-c8cb50291045"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""85140fb2-423a-4653-8656-90aa442e23db"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Axes.Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""basedOn"": """",
            ""bindingGroup"": ""Keyboard"",
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
            ""name"": ""Keyboard.WASD"",
            ""basedOn"": """",
            ""bindingGroup"": ""Keyboard.WASD"",
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
        }
    ]
}");
            // Player
            m_Player = asset.GetActionMap("Player");
            m_Player_AxesLeft = m_Player.GetAction("Axes.Left");
            m_Player_AxesRight = m_Player.GetAction("Axes.Right");
            m_Player_Jump = m_Player.GetAction("Jump");
        }

        ~Schema()
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
        private InputAction m_Player_AxesRight;
        private InputAction m_Player_Jump;
        public struct PlayerActions
        {
            private Schema m_Wrapper;
            public PlayerActions(Schema wrapper) { m_Wrapper = wrapper; }
            public InputAction @AxesLeft { get { return m_Wrapper.m_Player_AxesLeft; } }
            public InputAction @AxesRight { get { return m_Wrapper.m_Player_AxesRight; } }
            public InputAction @Jump { get { return m_Wrapper.m_Player_Jump; } }
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
                    AxesRight.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAxesRight;
                    AxesRight.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAxesRight;
                    AxesRight.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAxesRight;
                    Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                    Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                    Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                }
                m_Wrapper.m_PlayerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    AxesLeft.started += instance.OnAxesLeft;
                    AxesLeft.performed += instance.OnAxesLeft;
                    AxesLeft.canceled += instance.OnAxesLeft;
                    AxesRight.started += instance.OnAxesRight;
                    AxesRight.performed += instance.OnAxesRight;
                    AxesRight.canceled += instance.OnAxesRight;
                    Jump.started += instance.OnJump;
                    Jump.performed += instance.OnJump;
                    Jump.canceled += instance.OnJump;
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
        private int m_KeyboardSchemeIndex = -1;
        public InputControlScheme KeyboardScheme
        {
            get
            {
                if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.GetControlSchemeIndex("Keyboard");
                return asset.controlSchemes[m_KeyboardSchemeIndex];
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
        private int m_GamepadSchemeIndex = -1;
        public InputControlScheme GamepadScheme
        {
            get
            {
                if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.GetControlSchemeIndex("Gamepad");
                return asset.controlSchemes[m_GamepadSchemeIndex];
            }
        }
        public interface IPlayerActions
        {
            void OnAxesLeft(InputAction.CallbackContext context);
            void OnAxesRight(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
        }
    }
}
