using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Mirror;
using System.Threading;
using System.Threading.Tasks;
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

        public TaskCompletionSource<bool> PlayerConnectionResponseTask = new TaskCompletionSource<bool>();

        [TargetRpc]
        public void TargetReceive(ServerMessage msg)
        {
            Debug.Log("Player joined");

            if (msg.LocalPlayerId < 0)
            {
                Debug.Log("invalid local player id connected");
                PlayerConnectionResponseTask.SetResult(false);
                return;
            }

            if (msg.ServerPlayerId < 0)
            {
                Debug.Log("invalid server player id received");
                PlayerConnectionResponseTask.SetResult(false);
                return;
            }

            Debug.Log("Assigned server id with success: " + msg.ServerPlayerId);
            LocalPlayerManager.Instance.Players[msg.LocalPlayerId]._serverId = msg.ServerPlayerId;
            LocalPlayerManager.Instance.Players[msg.LocalPlayerId]._characterSlot = CharacterSelect.Instance._slots[msg.ServerPlayerId];


            PlayerConnectionResponseTask.SetResult(true);
        }

        [Command]
        public void CmdTryChangeState_CharacterSelectSlot(GameObject obj, CharacterSelectSlot.State target)
        {            
            CharacterSelectSlot slot;
            if ((slot = obj.GetComponent<CharacterSelectSlot>()) != null) slot.RpcTryChangeState(target);          
        }

        public void TryChangeState_Game(Game.State transition, params object[] args)
        {
            CmdTryChangeState_Game(transition, args);
        }

        [Command]
        public void CmdTryChangeState_Game(Game.State transition, object[] args)
        {
            RpcTryChangeState_Game(transition, args);
        }

        [ClientRpc]
        public void RpcTryChangeState_Game(Game.State transition, object[] args)
        {            
            Game.Instance.TryChangeState(transition, args);
        }

        [Command]
        public void CmdScroll_CharacterSelectSlot(GameObject obj, bool scroll)
        {
            CharacterSelectSlot slot;
            if ((slot = obj.GetComponent<CharacterSelectSlot>()) != null) slot.RpcScroll(scroll);
        }
    }
}
