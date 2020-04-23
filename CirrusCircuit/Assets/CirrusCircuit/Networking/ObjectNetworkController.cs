using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Cirrus.Circuit.Networking
{
    public class ObjectNetworkController : MonoBehaviour
    {        
        [Editor.GetComponent(typeof(NetworkIdentity),Editor.GetComponentAttributeMode.Self)]
        [SerializeField]
        public NetworkIdentity _netIdentity;

        [Editor.GetComponent(typeof(NetworkTransformChild), Editor.GetComponentAttributeMode.Self)]
        [SerializeField]
        public NetworkTransformChild _netTransform;

    }
}
