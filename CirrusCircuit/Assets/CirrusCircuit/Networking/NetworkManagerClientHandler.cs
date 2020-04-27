using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Mirror;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace Cirrus.Circuit.Networking
{

    public class NetworkManagerClientHandler : NetworkManagerHandler
    {
        public NetworkConnection _conn;

        public const int ServerResponseTimeout = 10000;

        public NetworkManagerClientHandler(CustomNetworkManager net) : base(net)
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

        public override bool TryPlayerJoin(int localId)
        {
            _conn.Send(new ClientPlayerMessage
            {
                LocalPlayerId = localId,
                Id = ClientPlayerMessageId.Join
            });

            var response = ClientConnectionPlayer.Instance.WaitResponse(ServerResponseTimeout);

            switch (response.Id)
            {
                case ServerMessageId.ServerId:

                    if (response.LocalPlayerId < 0)
                    {
                        Debug.Log("invalid local player id connected");
                        return false;
                    }

                    if (response.LocalPlayerId != localId)
                    {
                        Debug.Log("local player id conflict");
                        return false;
                    }

                    if (response.ServerPlayerId < 0)
                    {
                        Debug.Log("invalid server player id received");
                        return false;
                    }

                    Debug.Log("Assigned server id with success: " + response.ServerPlayerId);
                    LocalPlayerManager.Instance.Players[response.LocalPlayerId]._serverId = response.ServerPlayerId;
                    LocalPlayerManager.Instance.Players[response.LocalPlayerId]._characterSlot = CharacterSelect.Instance._slots[response.ServerPlayerId];
                    return true;

                default: return false;
            }

            //return false;
        }

        public override bool TryPlayerLeave(int localId)
        {
            _conn.Send(new ClientPlayerMessage
            {
                LocalPlayerId = localId,
                Id = ClientPlayerMessageId.Leave
            });

            return true;
        }
    }
}
