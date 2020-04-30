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
        [SyncVar]
        [SerializeField]        
        public float _score = 0;

        public float Score
        {
            get => _score;
            set
            {
                if (!hasAuthority) return;
                _score = value < 0 ? 0 : value;
                ClientPlayer.Instance.Cmd_PlayerSession_SetScore(gameObject, _score);
            }
        }

        [SyncVar]
        [SerializeField]
        public int _colorId = 0;

        [SyncVar]
        [SerializeField]        
        public int _characterId = -1;
        public int CharacterId
        {
            get => _characterId;
            set
            {
                if (!hasAuthority) return;
                _characterId = value;
                ClientPlayer.Instance.Cmd_PlayerSession_SetCharacterId(gameObject, _characterId);
            }                                    
        }


        [SerializeField]
        [SyncVar]
        public Color _color;
        public Color Color => _color;

        [SyncVar]
        [SerializeField]        
        public string _name;

        public string Name => _name;

        [SyncVar]
        [SerializeField]
        public int _connectionId = 0;

        [SyncVar]
        [SerializeField]
        public int _serverId = 0;

        public int ServerId => _serverId;

        [SyncVar]
        [SerializeField]        
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
            GameSession.Instance.LocalPlayers.Add(PlayerManager.Instance.LocalPlayers[_localId]);
            PlayerManager.Instance.LocalPlayers[_localId]._session = this;
            PlayerManager.Instance.LocalPlayers[_localId]._characterSlot = CharacterSelect.Instance._slots[_serverId];
        }       
    }
}