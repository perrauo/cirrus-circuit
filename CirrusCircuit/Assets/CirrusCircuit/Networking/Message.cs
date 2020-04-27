using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cirrus.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using Mirror;
using System;
using System.Net.Sockets;


namespace Cirrus.Circuit.Networking
{
    public enum ServerMessageId
    {        
        Failure = 400,
        Timeout = 401,
        NullResponse = 402,
        Unknown = 404,
        ServerId = 200
    }

    public class ServerMessage : MessageBase
    {
        public ServerMessageId Id = ServerMessageId.ServerId;

        public int LocalPlayerId = -1;

        public int ServerPlayerId = -1;
    }

    public enum ClientPlayerMessageId
    {   
        Join,
        Leave
    }

    public class ClientPlayerMessage : MessageBase
    {
        public ClientPlayerMessageId Id = ClientPlayerMessageId.Join;

        public string Name;

        public int LocalPlayerId = -1;
    }

    public class ClientConnectedMessage : MessageBase
    {
        //public string Name;
    }
}
