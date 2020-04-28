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
        public void TargetReceiveResponse(ServerResponseMessage response)
        {
            switch (response.Id)
            {
                case ServerMessageId.ServerId:
                    return;

                default: return;
            }

        }

        #region Character Select Slot

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

        #endregion

        #region Player Session

        [Command]
        public void CmdSetCharacterId_PlayerSession(GameObject obj, int characterId)
        {
            PlayerSession session;
            if ((session = obj.GetComponent<PlayerSession>()) != null) session._characterId = characterId;
        }

        [Command]
        public void CmdSetScore_PlayerSession(GameObject obj, float score)
        {
            PlayerSession session;
            if ((session = obj.GetComponent<PlayerSession>()) != null) session._score = score;
        }

        #endregion

        #region Game

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

        #endregion
    }
}
