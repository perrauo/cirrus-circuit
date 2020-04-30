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
        public static bool TryCreatePlayer(
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

        public static bool TryCreateNetworkObject(
            GameObject template,
            out GameObject obj,
            bool spawn = true)
        {
            return TryCreateNetworkObject(
                template, 
                out obj, 
                NetworkServer.localConnection, 
                spawn);
        }

        public static bool TryCreateNetworkObject(            
            GameObject template,
            out GameObject obj,
            NetworkConnection conn,
            bool spawn= true)
        {
            obj = null;

            if (template.GetComponent<NetworkIdentity>() == null) return false;

            obj = template.gameObject.Create();

            if(spawn) NetworkServer.Spawn(obj, conn);

            return true;
        }

        public static bool TryCreateNetworkObject(
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

        public static bool TryDestroyNetworkObject(
            GameObject obj)
        {
            if (obj.GetComponent<NetworkIdentity>() == null) return false;

            NetworkServer.Destroy(obj.gameObject);
            Object.Destroy(obj);

            return true;
        }

    }
}
