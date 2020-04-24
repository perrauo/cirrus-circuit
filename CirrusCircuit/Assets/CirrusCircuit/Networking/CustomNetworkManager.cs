using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cirrus.Circuit.Controls;
using Cirrus.Utils;
using Mirror;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Cirrus.Circuit.Networking
{
    public class NetworkManagerHandler
    {
        protected CustomNetworkManager _net;

        public NetworkManagerHandler(CustomNetworkManager net)
        {
            _net = net;
        }

        public virtual void OnClientConnect(NetworkConnection conn)
        {

        }

        public virtual void OnClientDisconnect(NetworkConnection conn)
        {

        }

        public virtual void OnClientError(NetworkConnection conn, int errorCode)
        {

        }

        public virtual void OnStartClient()
        {

        }

        public virtual void OnStopClient()
        {

        }
    }

    public class NetworkManagerClientHandler : NetworkManagerHandler
    {
        public NetworkConnection _conn;

        public NetworkManagerClientHandler(CustomNetworkManager net) : base(net)
        {
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            _conn = conn;
        }


        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            _conn = conn;
        }



        public bool TryClientPlayerJoin(Player player)
        {
            //player.

            _conn.Send(new CreateNetworkPlayerMessage());
            return true;
        }

    }

    public class NetworkManagerServerHandler : NetworkManagerHandler
    {
        public NetworkManagerServerHandler(CustomNetworkManager net) : base(net)
        {
            NetworkServer.RegisterHandler<CreateNetworkPlayerMessage>(OnNetworkPlayerCreateMessage);
        }

        private bool TryCreatePlayer(NetworkConnection conn, out NetworkPlayer player) // for this connection
        {
            player = null;

            if (_net.playerPrefab.GetComponent<NetworkPlayer>()) return false;

            player = _net.playerPrefab.gameObject.Create(_net.transform).GetComponent<NetworkPlayer>();

            if (NetworkServer.AddPlayerForConnection(conn, player.gameObject)) return true;

            return false;
        }

        public void OnNetworkPlayerCreateMessage(NetworkConnection conn, CreateNetworkPlayerMessage message)
        {
            Debug.Log("On network player created");
        }

    }


    public class CustomNetworkManager : NetworkManager
    {
        private TelepathyTransport Transport => (TelepathyTransport) transport;
        private NetworkManagerHandler _handler;

        public bool IsServer => _handler is NetworkManagerServerHandler;
        public NetworkManagerClientHandler Client => IsServer ? null : (NetworkManagerClientHandler)_handler;
        public NetworkManagerServerHandler Server => IsServer ? (NetworkManagerServerHandler)_handler : null;

        public static CustomNetworkManager Instance => (CustomNetworkManager) singleton;


        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            _handler.OnClientConnect(conn);

        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            _handler.OnClientDisconnect(conn);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            base.OnClientError(conn, errorCode);
            _handler.OnClientError(conn, errorCode);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            _handler.OnStartClient();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            _handler.OnStopClient();
        }

        public override void Awake()
        {
            base.Awake();
        }

        public bool TryServerHost(string port)
        {
            _handler = null;            
            ushort res = ushort.TryParse(port, out res) ? res: NetworkUtils.DefaultUserPort;

            _handler = new NetworkManagerServerHandler(this);
            Transport.port = res;
            StartServer();
            return true;            
        }

        // 25.1.149.130:4040

        public bool TryClientJoin(string hostAddress)
        {
            _handler = null;
            if (NetworkUtils.TryParseAddress(hostAddress, out IPAddress adrs, out ushort port))
            {
                _handler = new NetworkManagerClientHandler(this);
                Transport.port = port;
                StartClient(NetworkUtils.ToUri(adrs, TelepathyTransport.Scheme));
                return true;
            }

            return false;
        }

    }
}
