using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cirrus.Circuit.Controls;
using Cirrus.Utils;
using Mirror;

using System.Collections;
using UnityEngine;
using Mirror;

namespace Cirrus.Circuit.Networking
{
    public class ControlPlayer : NetworkBehaviour
    {
        [SerializeField]
        //[SyncVar]
        public Player Player;

        [SerializeField]
        [SyncVar]
        public int Id = -1;

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();              

            if (Id >= PlayerManager.Instance.Players.Length)
            {
                Debug.Log("Could not find player control");
                return;
            }
            
            Player = PlayerManager.Instance.Players[Id];
            Debug.Log("Assigned player control " + Id);
        }
    }
}
