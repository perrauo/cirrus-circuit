using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Cirrus.MirrorExt;
using Mirror;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Cirrus.Circuit.Networking;

using Cirrus.Circuit.World;
using System;
using Cirrus.Circuit.World.Objects;

namespace Cirrus.Circuit.World.Objects
{
    public class ObjectSession : NetworkBehaviour
    {
        public enum CommandId
        {
            LevelSession_IsMoveAllowed,
            LevelSession_IsFallThroughAllowed
        }

        public class CommandRequest
        {
            public CommandId Id;
            public bool Result = false;
            public Vector3Int step;

        }

        public class CommandResponse
        {
            public CommandId Id;
            public bool Success = false;

        }

        [SerializeField]
        public BaseObject _object;

        [SyncVar]
        [SerializeField]
        public int _index = -1;

        private Mutex _mutex = new Mutex();

        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                CommandClient
                    .Instance
                    .Cmd_ObjectSession_SetIndex(
                        gameObject,
                        _index);
            }
        }

        [ClientRpc]
        public void Rpc_Interact(GameObject sourceGameObject)
        {
            if (sourceGameObject
                    .TryGetComponent(
                        out ObjectSession sourceSession))
            {
                _mutex.WaitOne();

                _object
                    .Interact(
                        sourceSession
                        ._object);

                _mutex.ReleaseMutex();
            }
        }

        [ClientRpc]
        public void Rpc_Fall()
        {
            _mutex.WaitOne();

            _object.Fall();

            _mutex.ReleaseMutex();
        }

        [ClientRpc]
        public void Rpc_Move(NetworkMove move)
        {
            _mutex.WaitOne();

            _object.Move(move.ToMove());

            _mutex.ReleaseMutex();
        }

        public void Cmd_Move(Move move)
        {
            CommandClient
                .Instance
                .Cmd_ObjectSession_Move(
                    gameObject,
                    move.ToNetworkMove());
        }

        public void Cmd_Fall()
        {
            CommandClient
                .Instance
                .Cmd_ObjectSession_Fall(gameObject);
        }

        public void Cmd_Interact(BaseObject source)
        {
            CommandClient
                .Instance
                .Cmd_ObjectSession_Interact(
                    gameObject,
                    source
                    ._session
                    .gameObject);
        }

        public bool IsMoveAllowed(Move move)
        {
            return _object.IsMoveAllowed(move);
        }

        public bool IsFallAllowed()
        {
            return _object
                .IsFallAllowed();
        }

        public void Cmd_FallThrough(Vector3Int step)
        {
            CommandClient
                .Instance
                .Cmd_ObjectSession_FallThrough(
                    gameObject,
                    step);
        }

        [ClientRpc]
        public void Rpc_FallThrough(
            Vector3Int step,
            Vector3Int position)
        {
            _object
                .FallThrough(
                    step,
                    position);
        }


        // TODO
        ///////
        ///
        public void Cmd_Request(CommandRequest req)
        {
            CommandClient
                .Instance
                .Cmd_ObjectSession_Request(
                    gameObject,
                    req);
        }

        public void Target_Response(CommandResponse res)
        {
            _object
                .Respond(res);
        }
    }
}
