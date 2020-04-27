using Cirrus.Circuit.Controls;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;
//using Cirrus.Circuit.World.Objects.Characters.Actions;
//using Cirrus.Circuit.Controls;
using UnityInput = UnityEngine.InputSystem;
//using Cirrus.Circuit.Playable;

using Inputs = UnityEngine.InputSystem;

// Controls Navmesh Navigation

// TODO control inv and Hotbar through the inventory user the the inv directly

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using Cirrus.Circuit.Networking;
using Mirror;
using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;

namespace Cirrus.Circuit.Networking
{
    public class PlayerSession : NetworkBehaviour
    {
        [SerializeField]
        [SyncVar]
        private float _score = 0;

        [SerializeField]
        [SyncVar]
        public int _colorId = 0;

        public float Score
        {
            get => _score;
            set => _score = value < 0 ? 0 : value;
        }

        [SerializeField]
        [SyncVar]
        public int _characterResourceId = -1;

        [SerializeField]
        [SyncVar]
        public Color _color;

        public Color Color => _color;

        [SerializeField]
        [SyncVar]
        public string _name;

        public string Name => _name;

        [SerializeField]
        [SyncVar]
        public int _serverId = 0;

        public int ServerId => _serverId;

        [SerializeField]
        [SyncVar]
        public int _localId = 0;

        public int LocalId => _localId;

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            if (_localId < 0)
            {
                Debug.Log("invalid local player id connected");
                return;
            }

            if (_serverId < 0)
            {
                Debug.Log("invalid server player id received");
                return;
            }

            Debug.Log("Assigned server id with success: " + _serverId);
            Game.Instance._localPlayers.Add(PlayerManager.Instance.LocalPlayers[_localId]);
            PlayerManager.Instance.LocalPlayers[_localId]._session = this;
            PlayerManager.Instance.LocalPlayers[_localId]._characterSlot = CharacterSelect.Instance._slots[_serverId];

        }

    }
}