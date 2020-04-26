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
        public static NetworkClientPlayer Instance;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            Instance = this;            
        }

        [Command]
        public void Cmd_CharacterSelectSlot_TryChangeState(GameObject slotObj, UI.CharacterSelectSlot.State target)
        {
            UI.CharacterSelectSlot slot;
            if ((slot = slotObj.GetComponent<UI.CharacterSelectSlot>()) != null) slot.RpcTryChangeState(target);          
        }
    }
}
