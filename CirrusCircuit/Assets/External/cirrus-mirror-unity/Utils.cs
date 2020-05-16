using System;
using Cirrus.Utils;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cirrus.MirrorExt
{
    [Serializable]
    public class GameObjectSyncList : SyncList<GameObject> { }

    [Serializable]
    public class IntObjectSyncList : SyncList<int> { }

    public static class ServerUtils
    {
        public static bool CreatePlayer(
            NetworkConnection conn,
            GameObject playerTemplate,
            out NetworkBehaviour player)
        {
            player = null;

            if (playerTemplate.GetComponent<NetworkBehaviour>() == null) return false;

            player = playerTemplate.Create().GetComponent<NetworkBehaviour>();

            if (NetworkServer.AddPlayerForConnection(conn, player.gameObject)) return true;

            return false;
        }

        public static bool DestroyNetworkObject(
            GameObject obj)
        {
            if (obj.GetComponent<NetworkIdentity>() == null) return false;

            NetworkServer.Destroy(obj.gameObject);
            Object.Destroy(obj);

            return true;
        }

    }
}
