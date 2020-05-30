using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Cirrus.MirrorExt;
using Mirror;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace Cirrus.Circuit.Networking
{

    public class ClientHandler : NetworkManagerHandler
    {
        public NetworkConnection _conn;

        public const int ServerResponseTimeout = 10000;

        public override void Stop()
        {
            _net.StopClient();
        }

        public ClientHandler(CustomNetworkManager net) : base(net)
        {

        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            _conn = conn;
            _conn.Send(new ClientConnectedMessage());
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            _conn = conn;
        }

        public override bool RequestJoinPlayer(int localId)
        {
            return _conn.Send(new ClientPlayerMessage
            {
                LocalPlayerId = localId,
                Type = ClientPlayerMessageType.Join
            });            
        }

        public override bool RequestPlayerLeave(int localId)
        {
            _conn.Send(new ClientPlayerMessage
            {
                LocalPlayerId = localId,
                Type = ClientPlayerMessageType.Leave
            });

            return true;
        }

    }
}
