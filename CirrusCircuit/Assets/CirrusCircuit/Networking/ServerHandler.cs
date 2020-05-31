using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Cirrus;
using Mirror;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using Cirrus.MirrorExt;
using Cirrus.Circuit.World;

namespace Cirrus.Circuit.Networking
{

    public class ServerHandler : NetworkManagerHandler
    {
        private Dictionary<int, CommandClient> _connections = new Dictionary<int, CommandClient>();
        private Dictionary<int, List<int>> _players = new Dictionary<int, List<int>>();        

        public ServerHandler(CustomNetworkManager net) : base(net)
        {
            NetworkServer.RegisterHandler<ClientConnectedMessage>(OnClientConnectedMessage);
            NetworkServer.RegisterHandler<ClientPlayerMessage>(OnClientPlayerMessage);
        }

        public override void Stop()
        {
            _net.StopHost();
            ServerUtils.DestroyNetworkObject(GameSession.Instance.gameObject);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            // If local connection            
            if (NetworkServer.localConnection.connectionId == conn.connectionId)
            {
                if (ServerUtils.CreatePlayer(
                    NetworkServer.localConnection, 
                    NetworkingLibrary.Instance.ClientConnectionPlayer.gameObject,
                    out NetworkBehaviour player))
                {
                    _connections.Add(conn.connectionId, (CommandClient)player);
                }
            }
        }

        public bool GetConnection(int connectionId, out CommandClient client)
        {
            return _connections.TryGetValue(connectionId, out client);
        }

        public bool LeavePlayer(NetworkConnection conn, int serverPlayerId)
        {
            PlayerSession leavingPlayer = null;
            foreach (var player in GameSession.Instance.Players)
            {
                if (player == null) continue;
                if (player.ServerId == serverPlayerId) leavingPlayer = player;
            }

            if (leavingPlayer == null) return false;

            if (!PlayerManager.Instance.GetPlayer(leavingPlayer.LocalId, out Player localPlayer)) return false;

            if (!_players.TryGetValue(conn.connectionId, out List<int> playersPerConnection)) return false;

            if (playersPerConnection == null) return false;

            GameSession.Instance.Cmd_RemovePlayer(leavingPlayer);                
                
            playersPerConnection.Remove(leavingPlayer.LocalId);            

            localPlayer._characterSlot.RemoveAuthority();

            localPlayer._characterSlot = null;
            localPlayer._session = null;

            NetworkServer.Destroy(leavingPlayer.gameObject);
            leavingPlayer.gameObject.Destroy();

            GameSession.Instance.PlayerCount--;
            GameSession.Instance.CharacterSelectOpenCount =
                GameSession.Instance.CharacterSelectOpenCount == 0 ?
                0 :
                GameSession.Instance.CharacterSelectOpenCount - 1;

            return true;
        }


        public bool JoinPlayer(
            NetworkConnection conn, 
            int localPlayerId)
        {
            if (GameSession.Instance.PlayerCount == PlayerManager.PlayerMax) return false;

            List<int> playersPerConnection = null;
            if (_players.TryGetValue(conn.connectionId, out playersPerConnection))
            {
                if (
                    playersPerConnection == null ||
                    playersPerConnection.Contains(localPlayerId)) return false;                
            }
            else
            {
                playersPerConnection = new List<int>();
                _players.Add(conn.connectionId, playersPerConnection);
            }

            if (_connections.TryGetValue(
                conn.connectionId, 
                out CommandClient clientConnection))
            {
                PlayerSession playerSession = NetworkingLibrary.Instance.PlayerSession.Create();
                playerSession._connectionId = conn.connectionId;
                playerSession._serverId = GameSession.Instance.PlayerCount++;
                playerSession._color = PlayerManager.Instance.GetColor(playerSession._serverId);
                playerSession._name = PlayerManager.Instance.GetName(playerSession._serverId);
                playerSession._localId = localPlayerId;
                playersPerConnection.Add(localPlayerId);

                NetworkServer.Spawn(playerSession.gameObject);
                playerSession.netIdentity.AssignClientAuthority(conn);       
                GameSession.Instance._players.Add(playerSession.gameObject);
                playerSession.netIdentity.AssignClientAuthority(conn);

                CharacterSelectInterface
                    .Instance
                    .SetAuthority(
                        conn, 
                        playerSession.ServerId);

                return true;                    
            }

            return false;
        }

        public override bool RequestJoinPlayer(int localPlayerId)
        {
            // Debug.Log("On network player created");
            return JoinPlayer(
                NetworkServer.localConnection,
                localPlayerId);
        }

        public override bool RequestPlayerLeave(int localPlayerId)
        {
            return LeavePlayer(
                NetworkServer.localConnection,
                localPlayerId);
        }

        public void OnClientPlayerMessage(NetworkConnection conn, ClientPlayerMessage message)
        {
            switch (message.Type)
            {
                case ClientPlayerMessageType.Join:
                        JoinPlayer(
                            conn,
                            message.LocalPlayerId);
                    break;

                case ClientPlayerMessageType.Leave:
                    LeavePlayer(
                        conn,
                        message.LocalPlayerId);
                    break;
            }
        }

        public void OnClientConnectedMessage(NetworkConnection conn, ClientConnectedMessage message)
        {
            if (ServerUtils.CreatePlayer(
                conn, 
                NetworkingLibrary.Instance.ClientConnectionPlayer.gameObject, 
                out NetworkBehaviour client))
            {
                _connections.Add(conn.connectionId, (CommandClient)client);
            }
        }
    }

}
