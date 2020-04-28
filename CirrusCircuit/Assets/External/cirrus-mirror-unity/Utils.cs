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
using Cirrus.Circuit.UI;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Cirrus.MirrorExt
{
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
