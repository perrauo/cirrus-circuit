using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Mirror;
using UnityEngine;


namespace Cirrus.Circuit.Networking
{
    // Serves to sync the connection
    public class ClientPlayer : NetworkBehaviour
    {
        public static ClientPlayer _instance;

        public static ClientPlayer Instance
        {
            get
            {
                if (_instance == null)
                {
                    foreach (var player in FindObjectsOfType<ClientPlayer>())
                    {
                        if (player.hasAuthority)
                        {
                            _instance = player;
                            break;
                        }
                    }
                }

                return _instance;
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


        // TODO Character Select session
        [Command]
        public void Cmd_GameSession_SetCharacterSelectReadyCount(GameObject obj, int count)
        {
            GameSession session;
            if ((session = obj.GetComponent<GameSession>()) != null) session._characterSelectReadyCount = count;
        }

        [Command]
        public void Cmd_GameSession_SetCharacterSelectOpenCount(GameObject obj, int count)
        {
            GameSession session;
            if ((session = obj.GetComponent<GameSession>()) != null) session._characterSelectOpenCount = count;
        }


        [Command]
        public void Cmd_GameSession_SetSelectedLevelIndex(GameObject obj, int index)
        {            
            GameSession session;
            if ((session = obj.GetComponent<GameSession>()) != null) session._selectedLevelIndex = index;
        }

        //[Command]
        //public void Cmd_GameSession_TryChangeState_2(
        //    GameObject obj,
        //    GameSession.State transition)
        //{
        //    GameSession session;
        //    if ((session = obj.GetComponent<GameSession>()) != null) session.Rpc_TryChangeState_2(transition);
        //}
        //


        #endregion

        #region Game


        [Command]
        public void Cmd_Game_SelectLevel(int step)
        {
            //Debug.Log("CMD GAME SELECT LEVEL");

            Rpc_Game_SelectLevel(step);
        }


        [Command]
        public void Cmd_Game_TryChangeState(
            Game.State transition,
            bool transitionEffect)
        {
            Rpc_Game_TryChangeState(transition, transitionEffect);
        }

        [ClientRpc]
        public void Rpc_Game_TryChangeState(Game.State transition, bool transitionEffect)
        {
            Game.Instance._TryChangeState(transition, transitionEffect);
        }

        [ClientRpc]
        public void Rpc_Game_SelectLevel(int step)
        {
            Game.Instance._SelectLevel(step);
        }

        #endregion
    }
}
