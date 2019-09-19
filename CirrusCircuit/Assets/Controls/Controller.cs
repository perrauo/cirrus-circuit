using Cirrus.Circuit.Controls;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;
//using Cirrus.Circuit.World.Objects.Characters.Actions;
//using Cirrus.Circuit.Controls;
using UnityInput = UnityEngine.InputSystem;
//using Cirrus.Circuit.Playable;

using Inputs = UnityEngine.InputSystem;

// Controls Navmesh Navigation

// TODO control inv and Hotbar through the inventory user the the inv directly



namespace Cirrus.Circuit.Controls
{
    public class Controller : ActionMap.IPlayerActions
    {
        public float Score { get; set; }

        public delegate void OnReady(Controller ctrl);

        public OnReady OnReadyHandler;

        private ActionMap _actionMap;

        private Inputs.InputDevice _device;

        private Inputs.InputControlScheme _scheme;

        // TODO: Rework ? replace by mult action map

        public World.Objects.Character _character;

        public World.Objects.Characters.Resource _characterResource;

        public UI.CharacterSelectSlot _characterSlot;      

        public UI.Player PlayerDisplay = null;

        private int _number = 0;

        public int Number
        {
            get
            {
                return _number;
            }
        }

        public Controller(int number, Inputs.InputDevice device, Inputs.InputControlScheme scheme)
        {
            _number = number;
            _device = device;
            //_user = Inputs.Users.InputUser.CreateUserWithoutPairedDevices();
            //Inputs.Users.InputUser.PerformPairingWithDevice(_device, _user);

            // Each player gets a separate action setup. This makes the state of actions and bindings
            // local to each player and also ensures we're not stepping on the action setup used by
            // DemoGame itself for the main menu (where we are not using control schemes and just blindly
            // bind to whatever devices are available locally).
            _scheme = scheme;
            _actionMap = new ActionMap();
            _actionMap.bindingMask = new Inputs.InputBinding { groups = _scheme.bindingGroup }; 
            //_user.AssociateActionsWithUser(_actionMap);
            Enable();
        }

        ~Controller()
        {
            Enable(false);
            //_user.UnpairDevicesAndRemoveUser();
        }

        public void Enable(bool enabled = true)
        {
            if (enabled)
            {
                _actionMap.Player.Enable();
                _actionMap.Player.SetCallbacks(this);
            }
            else
            {
                //_actionMap.Player.SetCallbacks(null);
                _actionMap.Player.Disable();
                _actionMap.Disable();
                _actionMap.Player.AxesLeft.Disable();
                _actionMap.Player.AxesLeft.Dispose();
                //_actionMap.
                //_actionMap.
            }
        }

        // TODO: Simulate LeftStick continuous axis with WASD
        public void OnAxesLeft(UnityInput.InputAction.CallbackContext context)
        {
            var axis = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1);
            Game.Instance.HandleAxesLeft(this, axis);
        }

        // Cancel
        public void OnAction0(UnityInput.InputAction.CallbackContext context)
        {
            Game.Instance.HandleAction0(this);
        }

        // Accept
        public void OnAction1(UnityInput.InputAction.CallbackContext context)
        {
            Debug.Log(context);
            Game.Instance.HandleAction1(this);
        }
    }
}