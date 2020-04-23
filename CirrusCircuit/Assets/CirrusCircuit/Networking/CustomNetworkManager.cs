using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit
{
    public class CustomNetworkManager : BaseSingleton<CustomNetworkManager>
    {
        [SerializeField]
        private Mirror.NetworkManager _manager;

        public override void Awake()
        {
            base.Awake();
        }

        public void DoStart()
        {
            //_manager.
        }

        public void DoStop()
        {

        }

    }
}
