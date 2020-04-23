using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirror;

namespace Cirrus.Circuit.Networking
{
    public class CustomNetworkManager : NetworkManager
    {
        public Events.Event<NetworkConnection> OnClientConnectHandler;
        public Events.Event<NetworkConnection> OnClientDisconnectHandler;
        public Events.Event<NetworkConnection, int> OnClientErrorHandler;
        public Events.Event OnStartClientHandler;
        public Events.Event OnStopClientHandler;

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            OnClientConnectHandler?.Invoke(conn);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            OnClientDisconnectHandler?.Invoke(conn);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            base.OnClientError(conn, errorCode);
            OnClientErrorHandler?.Invoke(conn, errorCode);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            OnStartClientHandler?.Invoke();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            OnStopClientHandler?.Invoke();
        }
    }
}
