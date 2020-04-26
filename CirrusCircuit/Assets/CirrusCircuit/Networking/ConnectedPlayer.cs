//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using Cirrus.Circuit.Controls;
//using Cirrus.Utils;
//using Mirror;

//using System.Collections;
//using UnityEngine;
//using Mirror;

//namespace Cirrus.Circuit.Networking
//{
//    public class ConnectedPlayer : NetworkBehaviour
//    {
//        // TODO replace with server send message to client

//        [SerializeField]
//        //[SyncVar]
//        public Player Player;

//        [SerializeField]
//        [SyncVar]
//        public int ServerId = -1;

//        [SerializeField]
//        [SyncVar]
//        public int LocalId = -1;

//        public override void OnStartAuthority()
//        {
//            base.OnStartAuthority();

//            // TODO destroy

//            if (LocalId < 0)
//            {
//                Debug.Log("Invalid local player id connected");
//                return;
//            }

//            Player =  LocalPlayerManager.Instance.Players[LocalId];

//            Player._serverId = ServerId;

//            Debug.Log("ServerId " + ServerId + " assigned to " + Player.LocalId);
//        }
//    }
//}
