using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Cirrus.Utils;
using Mirror;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace Cirrus.Circuit.Networking
{

    public class NetworkManagerServerHandler : NetworkManagerHandler
    {
        private Dictionary<int, ClientConnectionPlayer> _connections = new Dictionary<int, ClientConnectionPlayer>();
        private Dictionary<int, List<int>> _players = new Dictionary<int, List<int>>();
        private int _playerCount = 0;


        public NetworkManagerServerHandler(CustomNetworkManager net) : base(net)
        {
            NetworkServer.RegisterHandler<ClientConnectedMessage>(OnClientConnectedMessage);
            NetworkServer.RegisterHandler<ClientPlayerMessage>(OnPlayerJoinMessage);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            // If local connection            
            if (NetworkServer.localConnection.connectionId == conn.connectionId)
            {
                if (TryCreateClient(NetworkServer.localConnection, out NetworkBehaviour player))
                {
                    _connections.Add(conn.connectionId, (ClientConnectionPlayer)player);
                }
            }
        }

        private bool TryCreateClient(NetworkConnection conn, out NetworkBehaviour player)
        {
            player = null;

            if (NetworkingLibrary.Instance.ClientConnectionPlayer.gameObject.GetComponent<NetworkBehaviour>() == null) return false;

            player = NetworkingLibrary.Instance.ClientConnectionPlayer.gameObject.Create().GetComponent<NetworkBehaviour>();

            if (NetworkServer.AddPlayerForConnection(conn, player.gameObject)) return true;

            return false;
        }

        public bool TryCreateNetworkObject(
            NetworkConnection conn,
            GameObject template,
            out NetworkIdentity obj,
            bool clientAuthority = true)
        {
            obj = null;

            if (template.GetComponent<NetworkIdentity>() == null) return false;

            obj = template.gameObject.Create().GetComponent<NetworkIdentity>();

            NetworkServer.Spawn(obj.gameObject, clientAuthority ? conn : null);

            return true;
        }

        public bool TryDestroyNetworkObject(
            GameObject obj)            
        {
            if (obj.GetComponent<NetworkIdentity>() == null) return false;

            NetworkServer.Destroy(obj.gameObject);
            GameObject.Destroy(obj);
           
            return true;
        }


        public bool DoTryPlayerJoin(NetworkConnection conn, int localPlayerId)
        {
            if (_playerCount == PlayerManager.Max) return false;

            List<int> connectionPlayers = null;
            if (_players.TryGetValue(conn.connectionId, out connectionPlayers))
            {
                if (
                    connectionPlayers == null ||
                    connectionPlayers.Contains(localPlayerId)) return false;                
            }
            else
            {
                connectionPlayers = new List<int>();
                _players.Add(conn.connectionId, connectionPlayers);
            }

            if (_connections.TryGetValue(conn.connectionId, out ClientConnectionPlayer clientConnection))
            {
                if (TryCreateNetworkObject(
                    conn, 
                    NetworkingLibrary.Instance.PlayerSession.gameObject, 
                    out NetworkIdentity sessionObj))
                {                    
                    PlayerSession session;
                    if ((session = sessionObj.GetComponent<PlayerSession>()) != null)
                    {
                        session._serverId = _playerCount++;
                        session._color = PlayerManager.Instance.GetColor(session._serverId);
                        session._name = PlayerManager.Instance.GetName(session._serverId);                        

                        session._localId = localPlayerId;
                        connectionPlayers.Add(localPlayerId);

                        session.netIdentity.AssignClientAuthority(conn);
                        CharacterSelect.Instance.AssignAuthority(conn, session._serverId);

                        Debug.Log("Server player ID: " + session._serverId);
                        Debug.Log("Local player ID: " + localPlayerId);
                        return true;
                    }
                    else TryDestroyNetworkObject(sessionObj.gameObject);
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
            if (TryCreateClient(conn, out NetworkBehaviour client))
            {
                _connections.Add(conn.connectionId, (ClientConnectionPlayer)client);
            }
        }
    }

}
