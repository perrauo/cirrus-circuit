using UnityEngine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Cirrus.Circuit.Controls
{
    public class PlayerManager : BaseSingleton<PlayerManager>
    {
        [SerializeField]
        private InputActionAsset _inputActionAsset;
       
        public static readonly string[] Names = { "Red", "Blue", "Yellow", "Green" };

        private static readonly Dictionary<int, string> ErrorCodes
            = new Dictionary<int, string>
            {
                { 1, "Error One" },
                { 2, "Error Two" }
            };

        public const string DefaultName = "Unknown";

        [SerializeField]
        public Color[] Colors = new Color[4];

        [SerializeField]
        public Player[] LocalPlayers;

        public int LocalPlayerCount = 0;
        
        public const int PlayerMax = 4;

        public static bool IsValidPlayerId(int id) => id >= 0 && id < PlayerMax;

        public string GetName(int id)
        {
            int num = 1;
            if (
                ErrorCodes
                .TryGetValue(
                    num,
                    out string value))
            {

            }
            else
            {
                Debug.Log("not found");
            }

            if (ErrorCodes.ContainsKey(1))
            {
                string value = ErrorCodes[1];
            }
            else
            {
                Debug.Log("not found");
            }


            string name = DefaultName;
            if (id < PlayerMax) name = Names[id];
            return name;
        }

        public bool GetPlayer(int localId, out Player player)
        {            
            player = null;
            if (localId < 0 || localId >= PlayerMax) return false;
            player = LocalPlayers[localId];
            return true;
        }

        public Color GetColor(int id)
        {
            return (id >= 0 && id < PlayerMax) ? Colors[id] : Color.white;                       
        }

        public override void Awake()
        {
            base.Awake();

            LocalPlayers = new Player[PlayerMax];

            var devices = InputSystem.devices;

            // TODO: do not assume one player per device?
            // TODO detect when devices connected
            foreach (var device in devices)
            {
                if (device != null)
                {
                    if (device is Keyboard ||
                        device is Gamepad)
                    {
                        //InputActionAsset actions;
                        foreach (InputControlScheme scheme in _inputActionAsset.controlSchemes)
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
        public void OnUnpairedInputDeviceUsed(InputControl control)
        {
            if (control.device is Gamepad)
            {
                //for (var i = 0; i < _users.Length; i++)
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
            else if (control.device is Keyboard)
            {

            }
        }

    }

}
