using Cirrus.Gembalaya.Controls;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;
//using Cirrus.Gembalaya.Objects.Characters.Actions;
//using Cirrus.Gembalaya.Controls;
using UnityInput = UnityEngine.InputSystem;
//using Cirrus.Gembalaya.Playable;


// Controls Navmesh Navigation

// TODO control inv and Hotbar through the inventory user the the inv directly



namespace Cirrus.Gembalaya.Controls
{
    public class Controller : Schema.IPlayerActions
    {
        [SerializeField]
        public UnityInput.Users.InputUser _user;

        [SerializeField]
        public Schema _schema;

        // TODO: Rework ? replace by mult action map

        public Schema.PlayerActions Actions { get { return _schema.Player; } }

        [SerializeField]
        private Objects.Characters.Character _character;

        public Controller()
        {
            _user = UnityInput.Users.InputUser.CreateUserWithoutPairedDevices();
            _schema = new Schema();
            _user.AssociateActionsWithUser(_schema);
            Enable();
        }         

        public void Enable(bool enabled = true)
        {
            if (enabled)
            {
                Actions.Enable();
                Actions.SetCallbacks(this);
            }
            else
            {
                Actions.Disable(); ;
                Actions.SetCallbacks(null);
            }
        }

        // TODO: Simulate LeftStick continuous axis with WASD
        public void OnAxesLeft(UnityInput.InputAction.CallbackContext context)
        {
            var val = context.ReadValue<Vector2>();

            var axis = Vector2.ClampMagnitude(val, 1);

            _character.TryMove(axis);
        }


        public void OnAxesRight(UnityInput.InputAction.CallbackContext context)
        {
            //var value = context.ReadValue<Vector2>();

            //_character.Controller.AxesRight = value;

        }

        public void OnJump(UnityInput.InputAction.CallbackContext context)
        {

        }
    }
}