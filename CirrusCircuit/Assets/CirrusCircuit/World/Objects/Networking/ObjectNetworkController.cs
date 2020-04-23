using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Cirrus.Circuit.World.Objects
{
    public class ObjectNetworkController : MonoBehaviour
    {        
        [Editor.GetComponent(typeof(NetworkIdentity),Editor.GetComponentAttributeMode.Self)]
        [SerializeField]
        public NetworkIdentity _netIdentity;

        [Editor.GetComponent(typeof(NetworkTransform), Editor.GetComponentAttributeMode.Self)]
        [SerializeField]
        public NetworkTransform _netTransform;

    }
}
