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

using Random = UnityEngine.Random;

namespace Cirrus.Circuit.Networking
{
    public class GameSession : NetworkBehaviour
    {
        public static Event<bool> OnStartClientStaticHandler;

        protected static GameSession _instance;

        public static bool IsNull => _instance == null;

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
        public Round _round;

        [SyncVar]
        [SerializeField]
        public int _roundIndex;

        [SerializeField]
        public List<Player> LocalPlayers = new List<Player>();

        [SerializeField]
        public List<PlayerSession> _players = new List<PlayerSession>();

        public Events.Event OnScreenResizedHandler;

        public Event<Gem, int, float> OnScoreValueAddedHandler;

        public Events.Event OnPodiumHandler;

        public Events.Event OnFinalPodiumHandler;

        public Event<Round> OnNewRoundHandler;

        public Event<World.Level, int> OnLevelSelectedHandler;

        public Event<bool> OnMenuHandler;

        public Event<bool> OnLevelSelectHandler;

        public Event<bool> OnCharacterSelectHandler;

        public Event<Player> OnLocalPlayerJoinHandler;

        public int _selectedLevelIndex;

        public int _currentLevelIndex = 0;

        public World.Level SelectedLevel => Game.Instance._levels[_selectedLevelIndex];        

        [SyncVar]
        [SerializeField]
        public float _targetSizeCamera = 10f;


        [Serializable]
        public enum State
        {
            CharacterSelection,
            LevelSelection,
            Begin,
            Round,
            Score,
            WaitingNextRound,
            Podium,
            FinalPodium,
            Transition
        }

        [SerializeField]
        public State _state = State.LevelSelection;

        [SerializeField]
        private State _transition;

        ////////   

        public virtual void OnValidate()
        {
            
        }

        void Awake()
        {
            if (Game.Instance.IsSeedRandomized)
                Random.InitState(Environment.TickCount);

            UI.CharacterSelect.Instance.OnCharacterSelectReadyHandler += OnCharacterSelected;
        }

        public void OnCharacterSelected(int playerCount)
        {
            TryChangeState(State.Transition, State.LevelSelection);
        }

        public virtual void Start()
        {
            Transitions.Transition.Instance.OnTransitionTimeoutHandler += OnTransitionTimeOut;
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
            Debug.Log("Client Started");
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

        [ClientRpc]
        public void Rpc_SelectLevel(int step)
        {
            DoSelectLevel(step);
        }

        public void SelectLevel(int step)
        {
            ClientPlayer.Instance.Cmd_GameSession_SelectLevel(gameObject, step);
        }

        public void DoSelectLevel(int step)
        {
            for (int i = 0; i < Game.Instance._levels.Length; i++)
            {
                if (Game.Instance._levels[i] == null) continue;
                Game.Instance._levels[i].TargetPosition = Vector3.zero + Vector3.right * (i - _currentLevelIndex) * Game.Instance.DistanceLevelSelect;
            }

            _selectedLevelIndex = _currentLevelIndex;

            _targetSizeCamera = Game.Instance._levels[_selectedLevelIndex].CameraSize;

            OnLevelSelectedHandler?.Invoke(SelectedLevel, step);
        }



        // TODO: Simulate LeftStick continuous axis with WASD
        public void HandleAxesLeft(Player controller, Vector2 axis)
        {
            FSMHandleAxesLeft(controller, axis);
        }

        public void HandleAction0(Player controller)
        {
            FSMHandleAction0(controller);
        }

        public void HandleAction1(Player controller)
        {
            FSMHandleAction1(controller);
        }        

        public void OnLevelSelect()
        {
            //OnLevelSelectHandler?.Invoke();
        }        

        public void OnRoundEnd()
        {
            _roundIndex++;

            if (_roundIndex < Game.Instance.RoundAmount)
            {
                TryChangeState(State.Transition, State.Podium);
            }
            else
            {
                _roundIndex = 0;
                TryChangeState(State.Transition, State.FinalPodium);
            }
        }

        private void OnPodiumFinished()
        {
            if (_state == State.FinalPodium)
                TryChangeState(State.Transition, State.LevelSelection);
            
            else TryChangeState(State.Transition, State.Round);
            
        }

        private void OnTransitionTimeOut()
        {
            switch (_transition)
            {
                case State.Podium:
                case State.FinalPodium:
                    //Destroy(_currentLevel.gameObject);
                    break;

                case State.Round:
                case State.LevelSelection:
                    //_podium.gameObject.SetActive(false);
                    break;

                    //case State.Round:
                    //    _podium.gameObject.SetActive(false);
                    //    break;
            }

            TryChangeState(_transition);
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

        public void Join(Player ctrl)
        {
            switch (_state)
            {
                case State.LevelSelection:
                    break;
            }
        }






        #region FSM


        public void FSMFixedUpdate()
        {
            switch (_state)
            {                
                case State.CharacterSelection:
                case State.Begin:
                case State.LevelSelection:
                case State.Round:
                case State.Score:
                case State.Podium:
                case State.FinalPodium:
                    CameraManager.Instance.Camera.orthographicSize =
                        Mathf.Lerp(
                            CameraManager.Instance.Camera.orthographicSize,
                            _targetSizeCamera,
                            Game.Instance.CameraSizeSpeed);

                    break;
            }
        }

        public void FSMUpdate()
        {
            switch (_state)
            {

                case State.Round:                    
                    break;
                
                case State.CharacterSelection:
                case State.Begin:
                case State.LevelSelection:
                case State.Score:
                case State.Podium:
                case State.FinalPodium:
                    break;
            }
        }

        public bool TryChangeState(State transition, params object[] args)
        {
            ClientPlayer.Instance.Cmd_GameSession_TryChangeState(gameObject, transition, args);
            return true;
        }

        [ClientRpc]
        public void Rpc_TryChangeState(State transition, params object[] args)
        {
            if (TryTransition(transition, out State destination))
            {
                ExitState(destination);
                TryFinishChangeState(destination, args);
            }
        }


        private void ExitState(State destination)
        {
            switch (_state)
            {
                default: break;

            }
        }


        private bool TryTransition(State transition, out State destination, params object[] args)
        {
            switch (_state)
            {

                case State.CharacterSelection:

                    switch (transition)
                    {
                        case State.Round:
                        case State.CharacterSelection:
                        case State.Transition:
                        //case State.Round:
                        case State.Begin:
                        case State.WaitingNextRound:
                        case State.LevelSelection:
                        case State.Score:
                        case State.Podium:
                        case State.FinalPodium:

                            destination = transition;
                            return true;
                    }
                    break;

                case State.Begin:

                    switch (transition)
                    {
                        case State.Round:
                        case State.CharacterSelection:
                        case State.Transition:
                        //case State.Round:
                        case State.Begin:
                        case State.WaitingNextRound:
                        case State.LevelSelection:
                        case State.Score:
                        case State.Podium:
                        case State.FinalPodium:

                            destination = transition;
                            return true;
                    }
                    break;

                case State.Transition:
                    switch (transition)
                    {
                        case State.Round:                        
                        case State.CharacterSelection:
                        case State.Transition:
                        case State.Begin:
                        case State.WaitingNextRound:
                        case State.LevelSelection:
                        case State.Score:
                        case State.Podium:
                        case State.FinalPodium:

                            destination = transition;
                            return true;
                    }
                    break;

                case State.Round:

                    switch (transition)
                    {
                        case State.CharacterSelection:
                        case State.Begin:
                        case State.Transition:
                        case State.WaitingNextRound:
                        case State.LevelSelection:
                        case State.Score:
                        case State.Podium:
                        case State.FinalPodium:

                            destination = transition;
                            return true;
                    }
                    break;

                case State.LevelSelection:
                    switch (transition)
                    { 
                        case State.CharacterSelection:
                        case State.Begin:
                        case State.Transition:
                        case State.WaitingNextRound:
                        case State.LevelSelection:
                        case State.Score:
                        case State.Podium:
                        case State.FinalPodium:

                            destination = transition;
                            return true;
                    }
                    break;


                case State.WaitingNextRound:
                    switch (transition)
                    {
                        case State.Round:                        
                        case State.CharacterSelection:
                        case State.Begin:
                        case State.Transition:
                        case State.LevelSelection:
                        //case State.Round:
                        case State.Podium:
                        case State.FinalPodium:

                            destination = transition;
                            return true;
                    }
                    break;

                case State.Podium:
                    switch (transition)
                    {
                        case State.CharacterSelection:
                        case State.Begin:
                        case State.Transition:
                        case State.WaitingNextRound:
                        case State.LevelSelection:
                        case State.Round:
                        case State.Podium:
                        case State.FinalPodium:

                            destination = transition;
                            return true;
                    }
                    break;

                case State.FinalPodium:
                    switch (transition)
                    {
                        case State.CharacterSelection:
                        case State.Begin:
                        case State.Transition:
                        case State.WaitingNextRound:
                        case State.LevelSelection:
                        case State.Round:
                        case State.Podium:
                        case State.FinalPodium:

                            destination = transition;
                            return true;
                    }
                    break;
            }

            destination = State.Round;
            return false;
        }

        protected bool TryFinishChangeState(State target, params object[] args)
        {
            switch (target)
            {            
                case State.CharacterSelection:
                    OnCharacterSelectHandler?.Invoke(true);
                    _state = target;
                    return true;

                case State.Begin:
                    //_podium.Clear();
                    Podium.Instance.Clear();

                    foreach (var player in _players)
                    {
                        if (player == null)
                            continue;

                        Podium.Instance.Add(
                            player, 
                            CharacterLibrary.Instance.Characters[player.CharacterId]);
                    }

                    Podium.Instance.gameObject.SetActive(false);

                    OnLevelSelectHandler.Invoke(false);

                    foreach (World.Level level in Game.Instance._levels)
                    {
                        if (level == null) continue;
                        if (level == Game.Instance._levels[_selectedLevelIndex]) continue;                            
                        level.gameObject.SetActive(false);
                    }

                    Game.Instance._levels[_selectedLevelIndex].gameObject.SetActive(false);                    

                    _state = target;
                    return TryChangeState(State.Round, Game.Instance.RoundTime);


                case State.Transition:

                    _transition = (State)args[0];

                    Transitions.Transition.Instance.Perform();

                    _state = target;
                    return true;


                case State.Podium:
                    Podium.Instance.gameObject.SetActive(true);
                    OnPodiumHandler?.Invoke();
                    _state = target;
                    return true;

                case State.FinalPodium:
                    Podium.Instance.gameObject.SetActive(true);
                    OnFinalPodiumHandler?.Invoke();
                    _state = target;
                    return true;

                case State.LevelSelection:

                    _state = target;

                    foreach (World.Level lv in Game.Instance._levels)
                    {
                        lv.gameObject.SetActive(true);
                        lv.OnLevelSelect();
                    }

                    OnLevelSelect();
                    SelectLevel(0);

                    return true;

                case State.Round:

                    Game.Instance._levels[_selectedLevelIndex].TargetPosition = Vector3.zero;
                    Game.Instance._levels[_selectedLevelIndex].transform.position = Vector3.zero;
                    Game.Instance._levels[_selectedLevelIndex].gameObject.SetActive(true);

                    //_currentLevel =
                    //    Instantiate(
                    //        Game.Instance._levels[_selectedLevelIndex].gameObject,
                    //        Vector3.zero, Quaternion.identity,
                    //        gameObject.transform).GetComponent<World.Level>();

                    //_selectedLevelIndex.gameObject.SetActive(false);


                    //_currentLevel.OnScoreValueAddedHandler += OnScoreValueAdded;

                    //_currentLevel.OnLevelCompletedHandler += OnLevelCompleted;

                    //List<Placeholder> placeholders = new List<Placeholder>();
                    //placeholders.AddRange(_currentLevel._characterPlaceholders);

                    int i = 0;
                    //while (!placeholders.IsEmpty())
                    //{
                    //    Placeholder placeholder = placeholders.RemoveRandom();

                    //    _controllers[i]._character = _controllers[i]
                    //        ._characterResource.Create(
                    //            _currentLevel.GridToWorld(placeholder._gridPosition),
                    //            _currentLevel.transform);

                    //    _controllers[i]._character.Number = _controllers[i].Number;

                    //    _controllers[i]._character.Color = _controllers[i].Color;

                    //    _controllers[i]._character._level = _currentLevel;

                    //    _controllers[i]._character.TryChangeState(Character.State.Disabled);

                    //    _controllers[i].Score = 0;

                    //    _controllers[i]._assignedNumber = placeholder.Number;

                    //    i++;

                    //    Destroy(placeholder.gameObject);
                    //}

                    _round =
                        new Round(
                            Game.Instance.CountDown,
                            Game.Instance.RoundTime,
                            Game.Instance.CountDownTime,
                            Game.Instance.IntermissionTime,
                            _roundIndex);

                    StartCoroutine(NewRoundCoroutine());

                    _state = target;

                    return true;



                case State.Score:

                    _state = target;

                    return true;

                case State.WaitingNextRound:
                    //Lobby.Characters.Clear();
                    //Lobby.Characters.AddRange(_selectedLevel.Characters);


                    _state = target;

                    //OnWaiting();
                    return true;


                default:
                    return false;
            }

        }



        // TODO: Simulate LeftStick continuous axis with WASD
        public void FSMHandleAxesLeft(Player player, Vector2 axis)
        {
            bool isMovingHorizontal = Mathf.Abs(axis.x) > 0.5f;
            bool isMovingVertical = Mathf.Abs(axis.y) > 0.5f;

            Vector3 stepHorizontal = new Vector3(Mathf.Sign(axis.x), 0, 0);
            Vector3 stepVertical = new Vector3(0, 0, Mathf.Sign(axis.y));
            Vector3 step = Vector3.zero;

            if (isMovingVertical && isMovingHorizontal)
            {
                ////moving in both directions, prioritize later
                //if (_wasMovingVertical[player.LocalId]) step = stepHorizontal;
                //else step = stepVertical;
            }
            else if (isMovingHorizontal)
            {
                step = stepHorizontal;
                //_wasMovingVertical[player.LocalId] = false;
            }
            else if (isMovingVertical)
            {
                step = stepVertical;
                //_wasMovingVertical[player.LocalId] = true;
            }

            switch (_state)
            {
                case State.CharacterSelection:

                    if (Mathf.Abs(step.z) > 0)
                    {
                        if (player._characterSlot == null) return;
                        player._characterSlot.Scroll(step.z > 0);
                    }

                    break;

                case State.LevelSelection:

                    if (Mathf.Abs(step.x) > 0)
                    {

                        int prev = _currentLevelIndex;

                        _currentLevelIndex =
                            Mathf.Clamp(_currentLevelIndex + (int)Mathf.Sign(step.x), 0, Game.Instance._levels.Length - 1);

                        if (prev != _currentLevelIndex)
                        {
                            SelectLevel((int)Mathf.Sign(step.x));
                        }
                    }

                    break;

                case State.WaitingNextRound:

                    break;

                //case State.Round:
                //    if (controller._character)
                //        controller._character?.TryMove(axis);
                //    break;

                case State.Score:
                    break;

                default:
                    break;
            }
        }

        public void FSMHandleAction0(Player player)
        {
            switch (_state)
            {
                case State.LevelSelection:
                    break;

                case State.CharacterSelection:

                    //if (_controllers.Contains(controller))
                    //{
                    //    _controllers.Remove(controller);
                    //    OnControllerJoinHandler?.Invoke(controller);
                    //}
                    //else
                    {
                        player._characterSlot.HandleAction0();
                    }

                    break;

                case State.WaitingNextRound:

                    if (LocalPlayers.Contains(player))
                    {
                        UI.HUD.Instance.Leave(player);
                        LocalPlayers.Remove(player);
                    }
                    else
                    {
                        foreach (Player other in PlayerManager.Instance.LocalPlayers) if (other == null) continue;
                        TryChangeState(State.LevelSelection);
                    }

                    break;

                case State.Round:
                    if (player._character) player._character?.TryAction0();

                    break;

                case State.Score:
                    break;
            }
        }

        public void FSMHandleAction1(Player player)
        {
            switch (_state)
            {
                case State.Podium:

                    break;

                case State.LevelSelection:
                    TryChangeState(State.Transition, State.Begin);
                    break;

                case State.CharacterSelection:

                    if (player._characterSlot != null) player._characterSlot.HandleAction1(player);
                    else CustomNetworkManager.Instance.RequestPlayerJoin(player);

                    break;

                case State.WaitingNextRound:
                    break;

                case State.Round:
                    if (player._character) player._character?.TryAction1();
                    break;

                case State.Score:
                    break;
            }

        }

        #endregion

    }
}
