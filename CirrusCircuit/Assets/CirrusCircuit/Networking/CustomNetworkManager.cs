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

        public virtual bool TryPlayerJoin(Player player)
        {
            return false;
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
            _conn.Send(new CreateNetworkPlayerMessage());
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            _conn = conn;
        }

        public override bool TryPlayerJoin(Player player)
        {
            _conn.Send(new CreateClientPlayerMessage {
                Name = player.Name,
                Id = player.Id
            });
            return true;
        }
    }

    public class NetworkManagerServerHandler : NetworkManagerHandler
    {
        private List<NetworkPlayer> _networkPlayers = new List<NetworkPlayer>();
        private List<ClientPlayer> _clientPlayers = new List<ClientPlayer>();

        public NetworkManagerServerHandler(CustomNetworkManager net) : base(net)
        {
            NetworkServer.RegisterHandler<CreateNetworkPlayerMessage>(OnNetworkPlayerCreateMessage);
            NetworkServer.RegisterHandler<CreateClientPlayerMessage>(OnClientPlayerCreateMessage);
        }

        private bool TryCreatePlayer(NetworkConnection conn, out NetworkBehaviour player) 
        {
            player = null;

            if (_net.NetworkPlayerTemplate.gameObject.GetComponent<NetworkBehaviour>() == null) return false;

            player = _net.NetworkPlayerTemplate.gameObject.Create().GetComponent<NetworkBehaviour>();

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

            if (obj.netIdentity.AssignClientAuthority(conn)) return true;

            return false;
        }

        public override bool TryPlayerJoin(Player player)
        {
            return false;
            //return TryCreatePlayer(
            //    NetworkServer.localConnection, 
            //    _net.ClientPlayerTemplate, 
            //    out NetworkBehaviour clientPlayer);            
        }

        public void OnClientPlayerCreateMessage(NetworkConnection conn, CreateClientPlayerMessage message)
        {
            Debug.Log("On network player created");
            if (TryCreateNetworkObject(
                conn,
                _net.ClientPlayerTemplate,
                out NetworkBehaviour player))
            {
                var clientPlayer = (ClientPlayer)player;
                clientPlayer.Id = message.Id;
                Debug.Log(clientPlayer.Id);

                _clientPlayers.Add(clientPlayer);
            }
        }

        public void OnNetworkPlayerCreateMessage(NetworkConnection conn, CreateNetworkPlayerMessage message)
        {
            Debug.Log("On network player created");
            if (TryCreatePlayer(conn, out NetworkBehaviour player))
            {
                _networkPlayers.Add((NetworkPlayer)player);
            }
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

        [SerializeField]
        private ClientPlayer _clientPlayerTemplate;
        public ClientPlayer ClientPlayerTemplate => _clientPlayerTemplate;

        [SerializeField]
        private NetworkPlayer _networkPlayerTemplate;
        public NetworkPlayer NetworkPlayerTemplate => _networkPlayerTemplate;

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
            //StartServer();
            StartHost();
            return true;            
        }

        public bool TryPlayerJoin(Player player)
        {
            return _handler.TryPlayerJoin(player);
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
