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
    //[System.Serializable]
    public class Player : ActionMap.IPlayerActions
    {
        private float _score = 0;

        public float Score
        {
            get => _score;
            set => _score = value < 0 ? 0 : value;
        }

        public delegate void OnReady(Player ctrl);

        public OnReady OnReadyHandler;

        private ActionMap _actionMap;

        private Inputs.InputDevice _device;

        private Inputs.InputControlScheme _scheme;

        // TODO: Rework ? replace by mult action map

        public World.Objects.Characters.Character _character;

        public World.Objects.Characters.CharacterAsset _characterResource;

        public UI.CharacterSelectSlot _characterSlot;      

        public UI.Player PlayerDisplay = null;

        public Color _color;

        public Color Color => _color;

        public string _name;

        public string Name => _name;        

        public int _number = 0;

        public int Number => _number;

        public int _assignedNumber = 0;

        public Vector2 AxisLeft => _actionMap.Player.AxesLeft.ReadValue<Vector2>();

        public Player(int number, string name, Color color, Inputs.InputDevice device, Inputs.InputControlScheme scheme)
        {
            _name = name;
            _color = color;
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
            if(!context.performed)
                Game.Instance.HandleAction0(this);
        }

        // Accept
        public void OnAction1(UnityInput.InputAction.CallbackContext context)
        {
            //context.
            if(!context.performed)
                Game.Instance.HandleAction1(this);
        }
    }
}