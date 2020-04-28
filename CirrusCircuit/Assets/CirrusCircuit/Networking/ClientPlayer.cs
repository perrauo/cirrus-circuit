using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Mirror;
using UnityEngine;


namespace Cirrus.Circuit.Networking
{
    // Serves to sync the connection
    public class ClientPlayer : NetworkBehaviour
    {
        public static ClientPlayer _Instance;

        public static ClientPlayer Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = FindObjectOfType<ClientPlayer>();
                return _Instance;
            }
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
        public void Cmd_CharacterSelectSlot_TryChangeState(GameObject obj, CharacterSelectSlot.State target)
        {            
            CharacterSelectSlot slot;

            Debug.Log("RPC SELECT OUTER CMD");
            if ((slot = obj.GetComponent<CharacterSelectSlot>()) != null)
            {
                Debug.Log("RPC SELECT INNER CMD");
                slot.Rpc_TryChangeState(target);
            }
        }

        [Command]
        public void Cmd_CharacterSelectSlot_Scroll(GameObject obj, bool scroll)
        {
            CharacterSelectSlot slot;
            if ((slot = obj.GetComponent<CharacterSelectSlot>()) != null) slot.Rpc_Scroll(scroll);
        }


        #endregion

        #region Player Session

        [Command]
        public void Cmd_PlayerSession_SetCharacterId(GameObject obj, int characterId)
        {
            PlayerSession session;
            if ((session = obj.GetComponent<PlayerSession>()) != null) session._characterId = characterId;
        }

        [Command]
        public void Cmd_PlayerSession_SetScore(GameObject obj, float score)
        {
            PlayerSession session;
            if ((session = obj.GetComponent<PlayerSession>()) != null) session._score = score;
        }

        #endregion

        #region Game Session

        [Command]
        public void Cmd_GameSession_SelectLevel(GameObject obj, int step)
        {
            GameSession session;
            if ((session = obj.GetComponent<GameSession>()) != null) session.Rpc_SelectLevel(step);
        }
       

        [Command]
        public void Cmd_GameSession_TryChangeState(
            GameObject obj, 
            GameSession.State transition, 
            object[] args)
        {
            GameSession session;
            if ((session = obj.GetComponent<GameSession>()) != null) session.Rpc_TryChangeState(transition, args);
        }


        #endregion
    }
}
