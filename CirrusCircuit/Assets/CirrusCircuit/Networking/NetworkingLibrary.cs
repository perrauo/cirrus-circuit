using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Cirrus.Utils;
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
        public ClientPlayer ClientConnectionPlayer;

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
