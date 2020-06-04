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

        #region Move

        public void Cmd_Move(Move move)
        {
            CommandClient
                .Instance
                .Cmd_ObjectSession_Move(
                    gameObject,
                    move.ToNetworkMove());
        }

        #endregion

        public void Cmd_FSM_SetState(ObjectState state)
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

        public void Cmd_PerformAction(ObjectAction action)
        {
            CommandClient
                .Instance
                .Cmd_ObjectSession_PerformAction(gameObject, action);
        }

        [ClientRpc]
        public void Rpc_PerformAction(ObjectAction action)
        {
            _object.PerformAction(action);
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


        //#region Fall

        //public void Cmd_Fall()
        //{
        //    CommandClient
        //        .Instance
        //        .Cmd_ObjectSession_Fall(gameObject);
        //}


        //#endregion

        //public void Cmd_Slide()
        //{
        //    CommandClient
        //        .Instance
        //        .Cmd_ObjectSession_Slide(gameObject);
        //}

    }
}
