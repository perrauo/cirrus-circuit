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

        public virtual bool RequestPlayerJoin(int playerId)
        {
            return false;
        }

        public virtual bool RequestPlayerLeave(int localId)
        {
            return false;
        }
    }


    public class CustomNetworkManager : NetworkManager
    {
        private TelepathyTransport Transport => (TelepathyTransport)transport;
        private NetworkManagerHandler _handler;

        public bool IsServer => _handler is NetworkManagerServerHandler;
        public NetworkManagerClientHandler ClientHandler => IsServer ? null : (NetworkManagerClientHandler)_handler;
        public NetworkManagerServerHandler ServerHandler => IsServer ? (NetworkManagerServerHandler)_handler : null;

        public static CustomNetworkManager Instance => (CustomNetworkManager)singleton;


        public void UpdatePrefabList()
        {
            if (NetworkingLibrary.Instance != null)
            {
                spawnPrefabs.Clear();

                //Debug.Log("1");

                foreach (FieldInfo field in typeof(NetworkingLibrary).GetFields())
                {
                    //Debug.Log("2");

                    var val = field.GetValue(NetworkingLibrary.Instance);
                    if (val != null)
                    {
                        //Debug.Log(val);                        

                        if (val is MonoBehaviour)
                        {
                            if (!spawnPrefabs.Contains(((MonoBehaviour)val).gameObject))
                            {
                                spawnPrefabs.Add(((MonoBehaviour)val).gameObject);
                            }
                        }
                        else if (val is NetworkBehaviour)
                        {
                            if (!spawnPrefabs.Contains(((NetworkBehaviour)val).gameObject))
                            {
                                spawnPrefabs.Add(((NetworkBehaviour)val).gameObject);
                            }
                        }
                        else if (val is GameObject)
                        {
                            if (!spawnPrefabs.Contains(val))
                            {
                                spawnPrefabs.Add((GameObject)val);
                            }
                        }
                        else if (val is NetworkBehaviour[])
                        {
                            foreach (var obj in (NetworkBehaviour[])val)
                            {
                                if (!spawnPrefabs.Contains(obj.gameObject))
                                {
                                    spawnPrefabs.Add((GameObject)obj.gameObject);
                                }
                            }
                        }

                    }

                }
            }
        }

        public override void OnValidate()
        {
            base.OnValidate();


#if UNITY_EDITOR
            if (spawnPrefabs.Count == 0)
            {
                UpdatePrefabList();
            }
#endif
        }


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

        public bool RequestPlayerJoin(int playerId)
        {
            return _handler.RequestPlayerJoin(playerId);
        }

        public bool RequestPlayerJoin(Controls.Player player)
        {
            return RequestPlayerJoin(player.LocalId);
        }

        public bool RequestPlayerLeave(int playerId)
        {
            return false;
        }

        public bool TryInitHost(string port, out GameSession session)
        {
            session = null;
            _handler = null;

            _handler = new NetworkManagerServerHandler(this);

            Transport.port = ushort.TryParse(port, out ushort res) ? res : NetworkUtils.DefaultPort;

            StartHost();

            if (!ServerHandler.TryCreateNetworkObject(
                NetworkServer.localConnection,
                NetworkingLibrary.Instance.GameSession.gameObject,
                out NetworkIdentity obj))
            {
                StopHost();
                return false;
            }


            return true;
        }

        // 25.1.149.130:4040

        public bool TryInitClient(string hostAddress)
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


#if UNITY_EDITOR
    [CustomEditor(typeof(CustomNetworkManager))]
    public class CustomNetworkManagerEditor : Mirror.NetworkManagerEditor
    {
        private CustomNetworkManager _man;

        public virtual void OnEnable()
        {
            _man = serializedObject.targetObject as CustomNetworkManager;

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Called whenever the inspector is drawn for this object.
            //DrawDefaultInspector();

            if (GUILayout.Button("Update prefab list"))
            {
                _man.UpdatePrefabList();
            }
        }
    }

#endif

}



