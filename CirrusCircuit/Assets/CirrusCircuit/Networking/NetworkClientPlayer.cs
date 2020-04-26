using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cirrus.Circuit.UI;


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
        public void CmdTryChangeState_CharacterSelectSlot(GameObject obj, CharacterSelectSlot.State target)
        {            
            CharacterSelectSlot slot;
            if ((slot = obj.GetComponent<CharacterSelectSlot>()) != null) slot.RpcTryChangeState(target);          
        }

        [Command]
        public void CmdScroll_CharacterSelectSlot(GameObject obj, bool scroll)
        {
            CharacterSelectSlot slot;
            if ((slot = obj.GetComponent<CharacterSelectSlot>()) != null) slot.RpcScroll(scroll);
        }
    }
}
