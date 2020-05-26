using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Cirrus.Circuit.World;
using Cirrus.Circuit.World.Objects;
using Cirrus;
using Mirror;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Cirrus.Circuit.Networking
{
    public class NetworkingLibrary : Resources.BaseAssetLibrary<NetworkingLibrary>
    {
        [SerializeField]
        public CommandClient ClientConnectionPlayer;

        [SerializeField]
        public ServerTimer ServerTimer;

        [SerializeField]
        public GameSession GameSession;

        [SerializeField]
        public PlayerSession PlayerSession;

        [SerializeField]
        public LevelSession LevelSession;

        [SerializeField]
        public RoundSession RoundSession;


        [SerializeField]
        public ObjectSession ObjectSession;

        [SerializeField]
        public NetworkBehaviour[] Others;        
    }
}
