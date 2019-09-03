using UnityEngine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Inputs = UnityEngine.InputSystem;


namespace Cirrus.Gembalaya.Controls
{
    public class Lobby : MonoBehaviour
    {
        [SerializeField]
        public Controller[] Players;

        public const int MaxPlayers = 4;

        public void Awake()
        {
            Players = new Controller[MaxPlayers];
        }


        // Update is called once per frame
        public void Start()
        {
                       
            
            Inputs.Users.InputUser.onUnpairedDeviceUsed += OnUnpairedInputDeviceUsed;
            //Inputs.Users.InputUser.
        }

        public void OnUnpairedInputDeviceUsed(InputControl control)
        {
            if (control.device is Gamepad)
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
        }

    }

}
