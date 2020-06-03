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
using Cirrus.Circuit.World.Objects;
////using Cirrus.Events;
using Cirrus.Circuit.World.Objects.Characters;
using Cirrus.MirrorExt;

using Cirrus.Circuit.World;

using Random = UnityEngine.Random;


namespace Cirrus.Circuit
{    
    public class GameSession : NetworkBehaviour
    {
        public static GameSession Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<GameSession>();
                return _instance;
            }
        }

        public static Delegate<bool> OnStartClientStaticHandler;

        protected static GameSession _instance;

        public static bool IsNull => _instance == null;       

        [SyncVar]
        [SerializeField]        
        public int _characterSelectReadyCount = 0;

        public int CharacterSelectReadyCount
        {
            get => _characterSelectReadyCount;
            set
            {
                _characterSelectReadyCount = value < 0 ? 0 : value;
                _characterSelectReadyCount = value > CharacterSelectOpenCount ? CharacterSelectOpenCount : value;
                CommandClient.Instance
                    .Cmd_GameSession_SetCharacterSelectReadyCount(
                        gameObject, 
                        _characterSelectReadyCount);
            }
        }

        [SyncVar]
        [SerializeField]        
        public int _characterSelectOpenCount = 0;

        public int CharacterSelectOpenCount
        {
            get => _characterSelectOpenCount;
            set
            {
                _characterSelectOpenCount = value < 0 ? 0 : value;
                CommandClient.Instance
                    .Cmd_GameSession_SetCharacterSelectOpenCount(
                        gameObject, 
                        _characterSelectOpenCount);
            }
        }

        [SyncVar]
        [SerializeField]
        public int _roundIndex;
        public int RoundIndex
        {
            get => _roundIndex;
            set
            {
                _roundIndex = value < 0 ? 0 : value;
                CommandClient.Instance
                    .Cmd_GameSession_SetCharacterSelectOpenCount(
                        gameObject, 
                        _characterSelectOpenCount);
            }
        }


        [SerializeField]
        public List<Player> LocalPlayers = new List<Player>();

        [SyncVar]
        [SerializeField]
        public int _playerCount = 0;

        public int PlayerCount
        {
            get => _playerCount;
            set
            {
                _playerCount = value < 0 ? 0 : value;                
                CommandClient.Instance
                    .Cmd_GameSession_SetPlayerCount(
                        gameObject, 
                        _playerCount);
            }
        }

        [SyncVar]
        [SerializeField]
        public GameObjectSyncList _players = new GameObjectSyncList();
        public IEnumerable<PlayerSession> Players => _players.Select(x => x.GetComponent<PlayerSession>());

        public PlayerSession GetPlayer(int i)
        {
            if (i < 0) return null;
            if (i >= _players.Count) return null;
            return _players[i].GetComponent<PlayerSession>();
        }

        [SyncVar]
        [SerializeField]        
        public int _selectedLevelIndex;

        public int SelectedLevelIndex
        {
            get => _selectedLevelIndex;
            set
            {
                _selectedLevelIndex = value < 0 ? 0 : value;
                CommandClient.Instance.Cmd_GameSession_SetSelectedLevelIndex(gameObject, _selectedLevelIndex);
            }
        }

        public Level SelectedLevel => Game.Instance._levels[_selectedLevelIndex];



        #region Unity Engine
        public virtual void OnValidate()
        {
            
        }

        public virtual void Awake()
        {
            
        }

        public virtual void Start()
        {            

        }

        #endregion

        public override void OnStartClient()
        {
            base.OnStartClient();
            OnStartClientStaticHandler?.Invoke(true);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            OnStartClientStaticHandler?.Invoke(true);
            _instance = null;
        }
    

        public void OnRoundEnd()
        {
            _roundIndex++;
        }

        public bool Cmd_RemovePlayer(PlayerSession player)
        {
            CommandClient.Instance.Cmd_GameSession_RemovePlayer(
                gameObject, 
                player.gameObject);

            return true;
        }        
    }
}
