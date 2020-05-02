using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Cirrus.Utils;
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
            NetworkServer.RegisterHandler<ClientPlayerMessage>(OnPlayerJoinMessage);
        }

        public override void Stop()
        {
            _net.StopHost();
            ServerUtils.TryDestroyNetworkObject(GameSession.Instance.gameObject);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            // If local connection            
            if (NetworkServer.localConnection.connectionId == conn.connectionId)
            {
                if (ServerUtils.TryCreatePlayer(
                    NetworkServer.localConnection, 
                    NetworkingLibrary.Instance.ClientConnectionPlayer.gameObject,
                    out NetworkBehaviour player))
                {
                    _connections.Add(conn.connectionId, (CommandClient)player);
                }
            }
        }

        public bool TryGetConnection(int connectionId, out CommandClient client)
        {
            return _connections.TryGetValue(connectionId, out client);
        }


        public bool DoTryPlayerJoin(NetworkConnection conn, int localPlayerId)
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

            if (_connections.TryGetValue(conn.connectionId, out CommandClient clientConnection))
            {
                if (ServerUtils.TryCreateNetworkObject(
                    conn, 
                    NetworkingLibrary.Instance.PlayerSession.gameObject, 
                    out NetworkIdentity sessionObj,
                    false))
                {                    
                    PlayerSession session;
                    if ((session = sessionObj.GetComponent<PlayerSession>()) != null)
                    {
                        session._connectionId = conn.connectionId;
                        session._serverId = GameSession.Instance.PlayerCount++;
                        session._color = PlayerManager.Instance.GetColor(session._serverId);
                        session._name = PlayerManager.Instance.GetName(session._serverId);                        
                        session._localId = localPlayerId;                        
                        session.netIdentity.AssignClientAuthority(conn);
                        
                        playersPerConnection.Add(localPlayerId);

                        GameSession.Instance._players.Add(session.gameObject);
                        CharacterSelect.Instance.AssignAuthority(conn, session._serverId);

                        //Debug.Log("Server player ID: " + session._serverId);
                        //Debug.Log("Local player ID: " + localPlayerId);
                        return true;
                    }
                    else ServerUtils.TryDestroyNetworkObject(sessionObj.gameObject);
                }
            }

            return false;
        }

        public override bool RequestPlayerJoin(int localPlayerId)
        {
            // Debug.Log("On network player created");
            return DoTryPlayerJoin(NetworkServer.localConnection, localPlayerId);
        }

        public override bool RequestPlayerLeave(int localPlayerId)
        {
            return false;
            //// Debug.Log("On network player created");
            //return DoTryPlayerJoin(NetworkServer.localConnection, localPlayerId);
        }

        public void OnPlayerJoinMessage(NetworkConnection conn, ClientPlayerMessage message)
        {
            DoTryPlayerJoin(conn, message.LocalPlayerId);
        }

        public void OnClientConnectedMessage(NetworkConnection conn, ClientConnectedMessage message)
        {
            if (ServerUtils.TryCreatePlayer(
                conn, 
                NetworkingLibrary.Instance.ClientConnectionPlayer.gameObject, 
                out NetworkBehaviour client))
            {
                _connections.Add(conn.connectionId, (CommandClient)client);
            }
        }
    }

}
