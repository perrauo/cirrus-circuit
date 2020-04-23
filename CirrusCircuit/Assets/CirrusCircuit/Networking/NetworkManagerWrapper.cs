using Cirrus.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using Mirror;
using System;
using System.Net.Sockets;

namespace Cirrus.Circuit.Networking
{
    // TODO
    public class ClientConnectionError
    {
        public string Message = "Unknown error";   
    }

    public class NetworkManagerWrapper :  BaseSingleton<NetworkManagerWrapper>
    {
        [SerializeField]
        private CustomNetworkManager _net;

        [SerializeField]
        private TelepathyTransport _transport;

        [SerializeField]
        private NetworkPlayerClient _playerClient;
        public NetworkPlayerClient PlayerClient  => _playerClient;

        private bool _isServer = false;

        public bool IsServer => _isServer;

        public override void Awake()
        {
            base.Awake();
            _net.OnClientConnectHandler += OnClientConnected;
            
            //_net.client
        }

        public bool TryServerHost(string port)
        {
            if (ushort.TryParse(port, out ushort res))
            {
                _transport.port = res;
                _net.StartServer();
                _isServer = true;
                return true;
            }

            return false;
        }

        public void OnClientConnected(NetworkConnection conn)
        {

        }

        // 25.1.149.130:4040

        public bool TryClientJoin(string hostAddress)
        {
            _isServer = false;
            if (NetworkUtils.TryParseAddress(hostAddress, out IPAddress adrs, out  ushort port))
            {
                _net.StartClient(NetworkUtils.ToUri(adrs, port));
                return true;
            }

            return false;
            //_manager.Start
        }

        private bool TryCreatePlayer(NetworkConnection conn, out NetworkPlayer player) // for this connection
        {
            player = null;

            if (_net.playerPrefab.GetComponent<NetworkPlayer>()) return false;

            player = _net.playerPrefab.gameObject.Create(transform).GetComponent<NetworkPlayer>();

            if (NetworkServer.AddPlayerForConnection(conn, player.gameObject))
            {
                return true;
            }

            return false;           
        }


    }
}
