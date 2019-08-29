
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
    public class Player : MonoBehaviour, Schema.IPlayerActions
    {
        [SerializeField]
        public Schema _schema;

        // TODO: Rework ? replace by mult action map

        public Schema.PlayerActions Actions { get { return _schema.Player; } }

        [SerializeField]
        private Objects.Characters.Character _character;

        public void Awake()
        {
            _schema = new Schema();
            Enable();
        }

        public void RegisterCharacter(Objects.Characters.Character character)
        {
            _character = character;
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


        public void OnJump(UnityInput.InputAction.CallbackContext context)
        {

        }

        // TODO: Simulate LeftStick continuous axis with WASD
        public void OnAxesLeft(UnityInput.InputAction.CallbackContext context)
        {
            var val = context.ReadValue<Vector2>();

            _character.Controller.AxesLeft = Vector2.ClampMagnitude(val, 1);
        }


        public void OnAxesRight(UnityInput.InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();

            //value.x = Utils.Mathf.Normalize(value.x, -500, 500, 0);
            //value.y = Utils.Mathf.Normalize(value.y, -500, 500, 0);

            _character.Controller.AxesRight = value;
            
        }

        public void OnCursorMove(UnityInput.InputAction.CallbackContext context)
        {
            Vector3 value = context.ReadValue<Vector2>();



        }

        public void OnCursorClick(UnityInput.InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();

        }

        public void OnCursorHold(UnityInput.InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();

        }

        public void OnInventorySwap(UnityInput.InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnInventoryToggle(UnityInput.InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnInventoryMovement(UnityInput.InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnAction1(UnityInput.InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnAction2(UnityInput.InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}