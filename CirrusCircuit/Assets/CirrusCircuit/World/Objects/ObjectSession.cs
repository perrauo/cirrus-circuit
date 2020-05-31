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
using System.Collections.Generic;
using System.Linq;

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

        #region Action


        [ClientRpc]
        public void Rpc_Move(NetworkMoveResult[] results)
        {
            _mutex.WaitOne();

            _object.Move(results.Select(x =>x.ToMoveResult()));

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

        #endregion

        internal void Cmd_FSM_SetState(ObjectState state)
        {
            CommandClient
                .Instance
                .Cmd_ObjectSession_SetState(gameObject, state);
        }

        [ClientRpc]
        public void Rpc_SetState(ObjectState state)
        {
            _object.FSM_SetState(state);
        }



        #region Interact

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

        #endregion



        #region Idle

        public void Cmd_Idle()
        {
            CommandClient
                .Instance
                .Cmd_ObjectSession_Idle(gameObject);
        }


        [ClientRpc]
        public void Rpc_Idle()
        {
            _mutex.WaitOne();

            _object.Cmd_FSM_SetState(ObjectState.Idle);

            _mutex.ReleaseMutex();
        }

        #endregion


        #region Fall

        public void Cmd_Fall()
        {
            CommandClient
                .Instance
                .Cmd_ObjectSession_Fall(gameObject);
        }


        #endregion

        #region Land

        public void Cmd_Land()
        {
            CommandClient
                .Instance
                .Cmd_ObjectSession_Land(gameObject);
        }

        [ClientRpc]
        public void Rpc_Land()
        {
            _object.Land();
        }

        #endregion

        public void Cmd_Slide()
        {
            CommandClient
                .Instance
                .Cmd_ObjectSession_Slide(gameObject);
        }

    }
}
