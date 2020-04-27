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

            if (_net.NetworkClientTemplate.gameObject.GetComponent<NetworkBehaviour>() == null) return false;

            player = _net.NetworkClientTemplate.gameObject.Create().GetComponent<NetworkBehaviour>();

            if (NetworkServer.AddPlayerForConnection(conn, player.gameObject)) return true;

            return false;
        }

        private bool TryCreateNetworkObject(
            NetworkConnection conn,
            NetworkBehaviour template,
            out NetworkBehaviour obj,
            bool clientAuthority = true)
        {
            obj = null;

            if (template.gameObject.GetComponent<NetworkBehaviour>() == null) return false;

            obj = template.gameObject.Create().GetComponent<NetworkBehaviour>();

            NetworkServer.Spawn(obj.gameObject);

            return true;
        }


        public bool DoTryPlayerJoin(NetworkConnection conn, int localPlayerId)
        {
            var response = new ServerMessage()
            {
                Id = ServerMessageId.ServerId
            };

            if (_playerCount == 4) response.Id = ServerMessageId.Failure;

            List<int> connectionPlayers = null;
            if (_players.TryGetValue(conn.connectionId, out connectionPlayers))
            {
                if (
                    connectionPlayers == null ||
                    connectionPlayers.Contains(localPlayerId))
                {
                    response.Id = ServerMessageId.Failure;
                }
            }
            else
            {
                connectionPlayers = new List<int>();
                _players.Add(conn.connectionId, connectionPlayers);
            }

            if (_connections.TryGetValue(conn.connectionId, out ClientConnectionPlayer clientConnection))
            {
                if (response.Id != ServerMessageId.Failure)
                {
                    connectionPlayers.Add(localPlayerId);
                    response.ServerPlayerId = _playerCount++;
                    response.LocalPlayerId = localPlayerId;
                    CharacterSelect.Instance.AssignAuthority(conn, response.ServerPlayerId);

                    Debug.Log("Server player ID: " + response.ServerPlayerId);
                    Debug.Log("Local player ID: " + response.LocalPlayerId);
                }

                clientConnection.TargetReceive(response);
            }

            return response.Id < ServerMessageId.Failure;
        }

        public override bool TryPlayerJoin(int localPlayerId)
        {
            // Debug.Log("On network player created");
            return DoTryPlayerJoin(NetworkServer.localConnection, localPlayerId);
        }

        public override bool TryPlayerLeave(int localPlayerId)
        {
            // Debug.Log("On network player created");
            return DoTryPlayerJoin(NetworkServer.localConnection, localPlayerId);
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
