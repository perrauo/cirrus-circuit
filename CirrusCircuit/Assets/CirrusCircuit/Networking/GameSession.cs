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
using Cirrus.Events;
using Cirrus.Circuit.World.Objects.Characters;
using Cirrus.MirrorExt;

using Cirrus.Circuit.World;

using Random = UnityEngine.Random;

namespace Cirrus.Circuit.Networking
{
    public class GameSession : NetworkBehaviour
    {
        public static Event<bool> OnStartClientStaticHandler;

        protected static GameSession _instance;

        public static bool IsNull => _instance == null;

        public Event<RoundSession> OnNewRoundHandler;

        private bool[] _wasMovingVertical = new bool[PlayerManager.Max];

        [SyncVar]
        [SerializeField]        
        public int _characterSelectReadyCount = 0;

        public int CharacterSelectReadyCount
        {
            get => _characterSelectReadyCount;
            set
            {
                _characterSelectReadyCount = value < 0 ? 0 : value;
                ClientPlayer.Instance.Cmd_GameSession_SetCharacterSelectReadyCount(gameObject, _characterSelectReadyCount);
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
                ClientPlayer.Instance.Cmd_GameSession_SetCharacterSelectOpenCount(gameObject, _characterSelectOpenCount);
            }
        }

        public static GameSession Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<GameSession>();
                return _instance;
            }
        }

        [SerializeField]
        public RoundSession _round;

        [SyncVar]
        [SerializeField]
        public int _roundIndex;

        [SerializeField]
        public List<Player> LocalPlayers = new List<Player>();

        [SerializeField]
        public List<PlayerSession> _players = new List<PlayerSession>();
        
        public Event<Gem, int, float> OnScoreValueAddedHandler;

        [SyncVar]
        [SerializeField]        
        public int _selectedLevelIndex;

        public int SelectedLevelIndex
        {
            get => _selectedLevelIndex;
            set
            {
                _selectedLevelIndex = value < 0 ? 0 : value;
                ClientPlayer.Instance.Cmd_GameSession_SetSelectedLevelIndex(gameObject, _selectedLevelIndex);
            }
        }

        public Level SelectedLevel => Game.Instance._levels[_selectedLevelIndex];        




        ////////   

        public virtual void OnValidate()
        {
            
        }

        void Awake()
        {
            if (Game.Instance.IsSeedRandomized) Random.InitState(Environment.TickCount);
        }

        public virtual void Start()
        {            

        }


        public IEnumerator NewRoundCoroutine()
        {
            yield return new WaitForEndOfFrame();

            OnNewRoundHandler?.Invoke(_round);

            //_round.OnRoundBeginHandler += _currentLevel.OnBeginRound;

            _round.OnRoundEndHandler += OnRoundEnd;

            _round.BeginIntermission();

            yield return null;
        }


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

        public void OnLevelCompleted(World.Level.Rule rule)
        {
            _round.Terminate();
            //OnRoundEnd();
        }


        public void SelectLevel(int step)
        {
            ClientPlayer.Instance.Cmd_Game_SelectLevel(step);
        }

    
        // TODO: Simulate LeftStick continuous axis with WASD  

        public void OnLevelSelect()
        {
            //OnLevelSelectHandler?.Invoke();
        }        

        public void OnRoundEnd()
        {
            _roundIndex++;

        }

        private void OnPodiumFinished()
        {
            //if (_state == State.FinalPodium)
            //    TryChangeState(State.LevelSelection);
            
            //else TryChangeState(State.Round);
            
        }

        public bool TryJoin(Player player)
        {
            //if (_controllers.Count >= _selectedLevelIndex.CharacterCount)
            //    return false;

            //_controllers.Add(player);

            return false;
        }

        public bool TryLeave(Player player)
        {
            //if (_controllers.Count >= _selectedLevelIndex.CharacterCount)
            //    return false;

            //_controllers.Remove(player);

            return false;
        }

        public void Join(Player player)
        {
            //switch (_state)
            //{
            //    case State.LevelSelection:
            //        break;
            //}
        }
        
    }
}
