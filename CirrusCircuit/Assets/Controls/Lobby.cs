using UnityEngine;
using System.Collections;
using UnityEngine;
using Inputs = UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Cirrus.Circuit.Controls
{
    public enum PlayerNumber
    {
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight
    }

    public class Lobby : MonoBehaviour
    {
        [SerializeField]
        private Inputs.InputActionAsset _inputActionAsset;

        [SerializeField]
        public Color[] Colors = new Color[4];

        public Controller[] Controllers;

        public int ControllerCount = 0;

        public int JoinedCount = 0;

        private const int _playerMax = 8;

        public List<Objects.Characters.Character> Characters;

        public void Awake()
        {
            Controllers = new Controller[_playerMax];
            Characters = new List<Objects.Characters.Character>();
        }

        public bool TryJoin(Controller controller)
        {
            if (controller.Character == null)
            {
                if (JoinedCount >= Game.Instance.CurrentLevel.CharacterCount)
                    return false;

                controller.Character = Game.Instance.CurrentLevel.Characters[JoinedCount];
                JoinedCount++;
                return true;
            }
            else return false;
        }

        public bool TryLeave(Controller controller)
        {
            if (controller.Character != null)
            {
                if (JoinedCount >= Game.Instance.CurrentLevel.CharacterCount)
                    return false;

                controller.Character = Game.Instance.CurrentLevel.Characters[JoinedCount];

                JoinedCount--;

                return true;
            }
            else return false;
        }


        public void OnValidate()
        {
        }

        // Update is called once per frame
        public void Start()
        {
            var devices = Inputs.InputDevice.all;
            
            // TODO: do not assume one player per device?
            foreach(var device in devices)
            {
                if (device != null)
                {
                    if (device is Inputs.Keyboard ||
                        device is Inputs.Gamepad)
                    {
                        //InputActionAsset actions;
                        foreach (Inputs.InputControlScheme scheme in _inputActionAsset.controlSchemes)
                        {
                            if (scheme.SupportsDevice(device))
                            {
                                Controllers[ControllerCount] = new Controller(ControllerCount, device, scheme);
                                ControllerCount++;
                            }
                        }                        
                    }
                }

                if (ControllerCount > _playerMax || ControllerCount > Game.Instance.CurrentLevel.CharacterCount) break;
            }

            //Inputs.Users.InputUser.onUnpairedDeviceUsed += OnUnpairedInputDeviceUsed;
        }
        

        public void OnUnpairedInputDeviceUsed(Inputs.InputControl control)
        {
            if (control.device is Inputs.Gamepad)
            {
                //for(var i = 0; i < _users.Length; i++)
                //{
                //    // find a user without a paired device
                //    if (_users[i].pairedDevices.Count == 0)
                //    {
                //        // pair the new Gamepad device to that user
                //        _users[i] = InputUser.PerformPairingWithDevice(control.device, _users[i]);
                //        return;
                //    }
                //}
            }
            else if(control.device is Inputs.Keyboard)
            {

            }
        }

    }

}
