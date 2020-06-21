using Cirrus.Circuit.Controls;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;
//using Cirrus.Circuit.World.Objects.Characters.Actions;
//using Cirrus.Circuit.Controls;
using UnityEngine.InputSystem;
//using Cirrus.Circuit.Playable;

using Inputs = UnityEngine.InputSystem;
using Cirrus.Circuit.Networking;

// Controls Navmesh Navigation

// TODO control inv and Hotbar through the inventory user the the inv directly



namespace Cirrus.Circuit.Controls
{
    [System.Serializable]
    public class Player : 
        ActionMap.IPlayerActions, 
        ActionMap.IPlayerCharacterActions
    {
        [SerializeField]
        public PlayerSession _session;

        public delegate void OnReady(Player ctrl);

        public OnReady OnReadyHandler;

        private ActionMap _actionMap;

        private InputDevice _device;

        private InputControlScheme _scheme;
        
        [SerializeField]
        public World.Objects.Characters.Character _character;        

        [SerializeField]
        public UI.CharacterSelectSlot _characterSlot;

        public Vector2 AxisLeft => _actionMap.Player.AxesLeft.ReadValue<Vector2>();        

        [SerializeField]
        public int _localId = 0;

        public int LocalId => _localId;

        public int ServerId => 
            _session == null || _session.gameObject == null ? 
                -1 : 
            _session.ServerId;

        public Player(int localId, InputDevice device, InputControlScheme scheme)
        {

            _localId = localId;
            _device = device;
            //_user = Inputs.Users.InputUser.CreateUserWithoutPairedDevices();
            //Inputs.Users.InputUser.PerformPairingWithDevice(_device, _user);

            // Each player gets a separate action setup. This makes the state of actions and bindings
            // local to each player and also ensures we're not stepping on the action setup used by
            // DemoGame itself for the main menu (where we are not using control schemes and just blindly
            // bind to whatever devices are available locally).
            _scheme = scheme;
            _actionMap = new ActionMap();
            _actionMap.bindingMask = new InputBinding { groups = _scheme.bindingGroup };
            Enable();
        }

        ~Player()
        {
            Enable(false);
        }

        public void Enable(bool enabled = true)
        {
            if (enabled)
            {
                // TODO deactivate non player character during gameplay

                _actionMap.Player.Enable();               
                _actionMap.Player.SetCallbacks(this);

                _actionMap.PlayerCharacter.Enable();
                _actionMap.PlayerCharacter.SetCallbacks(this);
            }
            else
            {                
                _actionMap.Player.SetCallbacks(null);            
                _actionMap.Player.AxesLeft.Disable();
                _actionMap.Player.AxesLeft.Dispose();
                _actionMap.Player.Disable();

                _actionMap.PlayerCharacter.SetCallbacks(null);                
                _actionMap.PlayerCharacter.AxesLeft.Disable();
                _actionMap.PlayerCharacter.AxesLeft.Dispose();
                _actionMap.PlayerCharacter.Disable();

                _actionMap.Disable();

            }
        }

        public bool IsAxesLeft => _actionMap.Player.AxesLeft.phase == InputActionPhase.Performed;


        // TODO: Simulate LeftStick continuous axis with WASD
        public void OnAxesLeft(InputAction.CallbackContext context)
        {            
            var axis = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1);

            // Debug.Log(axis);

            Game.Instance.HandleAxesLeft(this, axis);

            if (GameSession.IsNull) return;

        }

        // Cancel
        public void OnAction0(InputAction.CallbackContext context)
        {
            if (context.performed) return;

            if (context.canceled) return;            

            Game.Instance.HandleAction0(this);
        }

        // Accept
        public void OnAction1(InputAction.CallbackContext context)
        {
            //context.
            if (context.performed) return;

            if (context.canceled) return;

            //if(context.)

            Game.Instance.HandleAction1(this);
        }

        public void OnHold(InputAction.CallbackContext context)
        {
            Game.Instance.HandleHold(this, context);
        }
    }
}