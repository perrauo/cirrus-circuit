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
    // Serves for individual control on player
    public class ClientPlayer : NetworkBehaviour
    {
        public Controls.Player Player;

        public int Id = -1;

        public override void OnStartLocalPlayer()
        {
            //Player = 
        }

        public void Awake()
        {
            
        }
    }
}
