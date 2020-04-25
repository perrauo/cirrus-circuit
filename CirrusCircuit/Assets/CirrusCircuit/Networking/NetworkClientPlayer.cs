using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


namespace Cirrus.Circuit.Networking
{
    // Serves to sync the connection
    public class NetworkClientPlayer : NetworkBehaviour
    {
        public Controls.Player Player;

        public int Id = -1;

        public override void OnStartLocalPlayer()
        {
            //Player = 
            Debug.Log("Called on client");
        }

    }
}
