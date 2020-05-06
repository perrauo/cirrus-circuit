using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Cirrus.MirrorExt;
using Mirror;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using Cirrus.Circuit.World;
using System;
using Cirrus.Circuit.World.Objects;

namespace Cirrus.Circuit.Networking
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

        public int Index {
            get => _index;
            set {
                _index = value;
                CommandClient.Instance.Cmd_ObjectSession_SetIndex(gameObject, _index);
            }
        }

        [ClientRpc]
        public void Rpc_TryInteract(GameObject sourceObject)
        {
            ObjectSession sourceSession = null;
            if ((sourceSession = sourceObject.GetComponent<ObjectSession>()) != null)
            {
                _mutex.WaitOne();

                _object._TryInteract(sourceSession._object);

                _mutex.ReleaseMutex();
            }            
        }

        [ClientRpc]
        public void Rpc_TryFall()
        {
            _mutex.WaitOne();

            _object._TryFall();

            _mutex.ReleaseMutex();
        }

        [ClientRpc]
        public void Rpc_TryMove(Vector3Int step)
        {
            _mutex.WaitOne();

            _object._TryMove(step, null);

            _mutex.ReleaseMutex();
        }

        public void Cmd_TryMove(Vector3Int step)
        {
            CommandClient.Instance.Cmd_ObjectSession_TryMove(gameObject, step);
        }

        public void Cmd_TryFall()
        {
            CommandClient.Instance.Cmd_ObjectSession_TryFall(gameObject);
        }

        public void Cmd_TryInteract(BaseObject source)
        {
            CommandClient.Instance.Cmd_ObjectSession_TryInteract(gameObject, source._session.gameObject);
        }

        public bool IsMoveAllowed(Vector3Int step)
        {
            return _object.IsMoveAllowed(step);
        }

        public bool IsFallAllowed()
        {
            return _object.IsFallAllowed();
        }

        public void Cmd_LevelSession_TryFallThrough(Vector3Int step)
        {
            CommandClient.Instance
                .Cmd_ObjectSession_LevelSession_TryFallThrough(
                    gameObject, 
                    step);
        }

        [ClientRpc]
        public void Rpc_ObjectSession_LevelSession_TryFallThrough(
            Vector3Int step,
            Vector3Int position)            
        {
            _object._LevelSession_TryFallThrough(step, position);
        }


        // TODO
        ///////
        ///
        public void Cmd_ObjectSession_Request(CommandRequest req)
        {
            CommandClient.Instance.Cmd_ObjectSession_Request(gameObject, req);
        }

        public void Target_Response(CommandResponse res)
        {
            _object._Response(res);
        }
    }
}
