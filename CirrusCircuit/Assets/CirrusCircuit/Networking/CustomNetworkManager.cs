using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit.Networking
{
    // TODO
    public class ClientConnectionError
    {
        public string Message = "Unknown error";   
    }

    public class CustomNetworkManager : BaseSingleton<CustomNetworkManager>
    {
        [SerializeField]
        private Mirror.NetworkManager _net;

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

        public bool TryServerHost()
        {
            _net.StartServer();
            return true;
        }

        // 25.1.149.130:4040

        public bool TryClientJoin(string hostAddress)
        {
            if (Utils.StringUtils.IsValidIpAddress(hostAddress))
            {
                _net.StartClient(new System.Uri(hostAddress));
                return true;
            }

            return false;
            //_manager.Start
        }


    }
}
