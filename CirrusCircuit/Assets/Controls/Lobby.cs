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

        public int _joinedCount = 0;

        private const int _playerMax = 8;

        private List<Objects.Characters.Character> _characters;

        public void Awake()
        {
            Controllers = new Controller[_playerMax];
            _characters = new List<Objects.Characters.Character>();
        }


        public bool TryJoin(Controller controller)
        {
            if (controller.Character == null)
            {
                if (_joinedCount >= Game.Instance.CurrentLevel.CharacterCount)
                    return false;

                controller.Character = Game.Instance.CurrentLevel.Characters[_joinedCount];
                _joinedCount++;
                return true;
            }
            else return false;
        }

        public bool TryLeave(Controller controller)
        {
            if (controller.Character != null)
            {
                if (_joinedCount >= Game.Instance.CurrentLevel.CharacterCount)
                    return false;

                controller.Character = Game.Instance.CurrentLevel.Characters[_joinedCount];

                _joinedCount--;

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
                                Controllers[ControllerCount] = new Controller(device, scheme);
                                ControllerCount++;
                            }
                        }                        
                    }
                }

                if (ControllerCount > _playerMax || ControllerCount > Game.Instance.CurrentLevel.CharacterCount) break;
            }

            Inputs.Users.InputUser.onUnpairedDeviceUsed += OnUnpairedInputDeviceUsed;
        }

        public void OnApplicationQuit()
        {
            Controllers = null;
        }

        private void OnUserChange(
            Inputs.Users.InputUser user, 
            Inputs.Users.InputUserChange change,
            Inputs.InputDevice device)
        {
            switch (change)
            {
                case Inputs.Users.InputUserChange.Added:

                    break;

                // A player has switched accounts. This will only happen on platforms that have user account
                // management (PS4, Xbox, Switch). On PS4, for example, this can happen at any time by the
                // player pressing the PS4 button and switching accounts. We simply update the information
                // we display for the player's active user account.
                case Inputs.Users.InputUserChange.AccountChanged:

                    break;

                // If the user has canceled account selection, we remove the user if there's no devices
                // already paired to it. This usually happens when a player initiates a join on a device on
                // Xbox or Switch, has the account picker come up, but then cancels instead of making an
                // account selection. In this case, we want to cancel the join.
                // NOTE: We are only adding DemoPlayerControllers once device pairing is complete
                case Inputs.Users.InputUserChange.AccountSelectionCanceled:

                    break;

                // An InputUser gained a new device. If we're in the lobby and don't yet have a player
                // for the user, it means a new player has joined. We don't join players until they have
                // a device paired to them which is why we ignore InputUserChange.Added and only react
                // to InputUserChange.DevicePaired instead.
                case Inputs.Users.InputUserChange.DevicePaired:

                    break;

                // Some player ran out of battery or unplugged a wired device.
                case Inputs.Users.InputUserChange.DeviceLost:

                    break;


                // Some player has customized controls or had previously customized controls loaded.
                case Inputs.Users.InputUserChange.BindingsChanged:

                    break;

            }
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
