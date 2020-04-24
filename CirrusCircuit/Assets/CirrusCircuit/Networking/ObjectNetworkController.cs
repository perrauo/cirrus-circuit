using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Cirrus.Circuit.Networking
{
    public class ObjectNetworkController : NetworkBehaviour
    {        
        [Editor.GetComponent(typeof(NetworkIdentity),Editor.GetComponentAttributeMode.Self)]
        [SerializeField]
        public NetworkIdentity _netIdentity;

        [Editor.GetComponent(typeof(NetworkTransformBase), Editor.GetComponentAttributeMode.Self)]
        [SerializeField]
        public NetworkTransformBase _netTransform;

    }
}
