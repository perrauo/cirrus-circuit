using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Mirror;
using UnityEngine;


namespace Cirrus.Circuit.Networking
{
    // Serves to sync the connection
    public class ClientConnectionPlayer : NetworkBehaviour
    {
        public static ClientConnectionPlayer Instance;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();           
            Instance = this;                          
        }

        [TargetRpc]
        public void TargetOnServerPlayerMessage(ServerPlayerMessage msg)
        {
            Debug.Log("Player joined");

            if (msg.LocalPlayerId < 0)
            {
                Debug.Log("invalid local player id connected");
                return;
            }

            if (msg.ServerPlayerId < 0)
            {
                Debug.Log("invalid server player id received");
                return;
            }

            Debug.Log("Assigned server id with success: " + msg.ServerPlayerId);
            LocalPlayerManager.Instance.Players[msg.LocalPlayerId]._serverId = msg.ServerPlayerId;
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
