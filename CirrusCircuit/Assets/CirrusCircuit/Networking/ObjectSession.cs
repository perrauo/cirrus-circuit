using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Cirrus.MirrorExt;
using Mirror;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using Cirrus.Circuit.World;
using System;

namespace Cirrus.Circuit.Networking
{
    public class ObjectSession : NetworkBehaviour
    {
        [SerializeField]
        public World.Objects.BaseObject _object;

        private Mutex _mutex;

        [ClientRpc]
        public void Rpc_TryMove(Vector3Int step)
        {
            _mutex.WaitOne();

            _object._TryMove(step);

            _mutex.ReleaseMutex();
        }

        public void TryMove(Vector3Int step)
        {
            ClientPlayer.Instance.Cmd_ObjectSession_TryMove(gameObject, step);
        }

        public bool IsMoveAllowed(Vector3Int step)
        {
            return _object.IsMoveAllowed(step);
        }
    }
}
