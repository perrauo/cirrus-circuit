using UnityEngine;
using System.Collections;
using System;
using Cirrus.Utils;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cirrus.MirrorExt
{
    public class CustomNetworkBehaviour : NetworkBehaviour
    {
        public Events.Event OnStopServerHandler;
        public Events.Event OnStartAuthorityHandler;

        public Events.Event OnStartClientHandler;
        public Events.Event OnStopClientHandler;
        public Events.Event OnStopAuthorityHandler;
        public Events.Event OnStartServerHandler;
        public Events.Event OnStartLocalPlayerHandler;

        public bool IsServerOrClient => isServer || isClient;

        public override void OnStopServer()
        {
            base.OnStopServer();

            OnStopServerHandler?.Invoke();

        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();


            OnStartAuthorityHandler?.Invoke();

        }


        public override void OnStartClient()
        {
            base.OnStartClient();

            OnStartClientHandler?.Invoke();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            OnStopClientHandler?.Invoke();
        }

        public override void OnStopAuthority()
        {
            base.OnStopAuthority();

            OnStopAuthorityHandler?.Invoke();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            OnStartServerHandler?.Invoke();
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            OnStartLocalPlayerHandler?.Invoke();

        }

    }
}
