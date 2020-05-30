using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cirrus.Circuit.Controls;
using Cirrus;
using Mirror;

using UnityEngine;
using System.Reflection;
using UnityEditor;

namespace Cirrus.Circuit.Networking
{
    public class NetworkManagerHandler
    {
        protected CustomNetworkManager _net;

        public virtual void Stop()
        {

        }

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

        public virtual bool RequestJoinPlayer(int localPlayerId)           
        {
            return false;
        }

        public virtual bool RequestPlayerLeave(int localPlayerId)
        {
            return false;
        }
    }


    public class CustomNetworkManager : NetworkManager
    {
        private TelepathyTransport Transport => (TelepathyTransport)transport;
        private NetworkManagerHandler _handler;

        //public bool IsServer => _handler is ServerHandler;
        public static bool IsServer => Instance._handler is ServerHandler;
        public ClientHandler ClientHandler => IsServer ? null : (ClientHandler)_handler;
        public ServerHandler ServerHandler => IsServer ? (ServerHandler)_handler : null;

        private static bool _isStarted = false;
        public static bool IsStarted => _isStarted;


        private static CustomNetworkManager _instance = null;
        public static CustomNetworkManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<CustomNetworkManager>();
                return _instance;
            }
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

        public bool RequestJoinPlayer(int localPlayerId)
        {
            return _handler.RequestJoinPlayer(localPlayerId);
        }

        public bool RequestJoinPlayer(Player player)
        {
            return RequestJoinPlayer(player.LocalId);
        }

        public bool RequestPlayerLeave(int serverPlayerId)
        {
            return _handler.RequestPlayerLeave(serverPlayerId);
        }

        public void Stop()
        {
            _isStarted = false;
            _handler.Stop();
        }

        public bool StartHost(string port)
        {            
            _handler = null;
            _handler = new ServerHandler(this);
            Transport.port = ushort.TryParse(port, out ushort res) ? res : NetworkUtils.DefaultPort;
            _isStarted = true;
            StartHost();

            NetworkServer.Spawn(
                NetworkingLibrary.Instance.GameSession.gameObject.Create(),
                NetworkServer.localConnection);

            return true;
        }

        // 25.1.149.130:4040

        public bool StartClient(string hostAddress)
        {
            _handler = null;
            if (NetworkUtils.ParseAddress(hostAddress, out IPAddress adrs, out ushort port))
            {
                _handler = new ClientHandler(this);
                Transport.port = port;
                _isStarted = true;
                StartClient(NetworkUtils.ToUri(adrs, TelepathyTransport.Scheme));
                return true;
            }

            return false;
        }


        #region Editor

#if UNITY_EDITOR
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

                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(this);
            }
        }

        public override void OnValidate()
        {
            base.OnValidate();



            if (spawnPrefabs.Count == 0)
            {
                UpdatePrefabList();
            }

        }

#endif
        #endregion
    }



    #region Editor
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

    #endregion

}



