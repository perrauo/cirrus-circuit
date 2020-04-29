using UnityEngine;
using System.Collections;
using UnityEngine;
using Inputs = UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Cirrus.Circuit.Controls
{
    public class PlayerManager : BaseSingleton<PlayerManager>
    {
        [SerializeField]
        private Inputs.InputActionAsset _inputActionAsset;

        [SerializeField]
        public string[] Names = { "Red", "Blue", "Yellow", "Green" };

        public const string DefaultName = "Unknown";

        [SerializeField]
        public Color[] Colors = new Color[4];

        public Player[] LocalPlayers;

        public int LocalPlayerCount = 0;
        
        public const int PlayerMax = 4;

        public string GetName(int id)
        {
            string name = DefaultName;
            if (id < PlayerMax) name = Names[id];
            return name;
        }

        public Color GetColor(int id)
        {
            Color color = Color.white;
            if (id < PlayerMax) color = Colors[id];
            return color;
        }

        public override void Awake()
        {
            base.Awake();

            LocalPlayers = new Player[PlayerMax];

            var devices = Inputs.InputDevice.all;

            // TODO: do not assume one player per device?
            // TODO detect when devices connected
            foreach (var device in devices)
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
                                LocalPlayers[LocalPlayerCount] =
                                    new Player(
                                        LocalPlayerCount++,
                                        device,
                                        scheme);
                            }
                        }
                    }
                }

                if (LocalPlayerCount > PlayerMax) break;
            }
        }



        public override void OnValidate()
        {
            base.OnValidate();
        }

        
                


        // TODO
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
            else if (control.device is Inputs.Keyboard)
            {

            }
        }

    }

}
