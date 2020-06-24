//using UnityEngine;
//using System.Collections;
//using UnityEngine.InputSystem;

//namespace Cirrus.Circuit.Controls
//{
//    public class PlayerCharacterActionHandler : ActionMap.IPlayerCharacterActions
//    {
//        private Player _player;

//        public PlayerCharacterActionHandler(Player player)
//        {
//            _player = player;
//        }    

//        public void OnAxesLeft(InputAction.CallbackContext context)
//        {
//            throw new System.NotImplementedException();
//        }

//        public void OnHold(InputAction.CallbackContext context)
//        {
//            Game.Instance.HandleHold(_player, context);
//        }
//    }


//    public class PlayerActionHandler : ActionMap.IPlayerActions
//    {
//        private Player _player;

//        public PlayerActionHandler(Player player)
//        {
//            _player = player;
//        }

//        public void OnAction0(InputAction.CallbackContext context)
//        {
//            throw new System.NotImplementedException();
//        }

//        public void OnAction1(InputAction.CallbackContext context)
//        {
//            throw new System.NotImplementedException();
//        }

//        public void OnAxesLeft(InputAction.CallbackContext context)
//        {
//            throw new System.NotImplementedException();
//        }
//    }
//}