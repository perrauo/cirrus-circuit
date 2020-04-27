using Cirrus.Circuit.Controls;
using Cirrus.Circuit.Networking;
using Cirrus.Circuit.World.Objects.Characters;
using Cirrus.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit
{
    public class Layers
    {
        //public int 
        public int MoveableFlags = 1 << LayerMask.NameToLayer("Moveable");
        public int SolidFlags = 1 << LayerMask.NameToLayer("Solid");
        public int Moveable = LayerMask.NameToLayer("Moveable");
        public int Solid = LayerMask.NameToLayer("Solid");
    }

    public class Game : BaseSingleton<Game>
    {
        #region Core

        [SerializeField]
        private bool _randomizeSeed = false;

        public Events.Event OnScreenResizedHandler;

        public World.Objects.Door.OnScoreValueAdded OnScoreValueAddedHandler;

        public Events.Event OnPodiumHandler;

        public Events.Event OnFinalPodiumHandler;

        public Events.Event<Round> OnNewRoundHandler;

        public Events.Event<World.Level, int> OnLevelSelectedHandler;

        public Events.Event<bool> OnMenuHandler;

        public Events.Event<bool> OnLevelSelectHandler;

        public Events.Event<bool> OnCharacterSelectHandler;

        public Events.Event<Player> OnLocalPlayerJoinHandler;

        public Layers Layers;

        public UI.CharacterSelect _characterSelect;

        public UI.StartMenu _startMenu;

        [SerializeField]
        public World.Level[] _levels;

        public World.Level _selectedLevel;

        public World.Level _currentLevel;

        public int _currentLevelIndex = 0;

        public float _distanceLevelSelect = 35;

        [SerializeField]
        public float _targetSizeCamera = 10f;

        [SerializeField]
        public float _cameraSizeSpeed = 0.8f;
    
        [SerializeField]
        public float _roundTime = 60f;

        [SerializeField]
        public float _countDownTime = 5f;

        [SerializeField]
        public int _countDown = 3;


        [SerializeField]
        private int _roundAmount = 3;

        [SerializeField]
        public float _podiumTransitionSpeed = 0.2f;

        [SerializeField]
        private float _intermissionTime = 2f;

        [SerializeField]
        private float _transitionTime = 5f;

        private Timer _transitionTimer = null;

        private State _transition;

        private float _transitionDistance = 48f;

        private Vector3 _initialVectorBottomLeft;

        private Vector3 _initialVectorTopRight;

        private Vector3 _updatedVectorBottomLeft;

        private Vector3 _updatedVectorTopRight;


        public int _roundIndex;

        public Round _round;

        public List<Player> _localPlayers;

        public override void OnValidate()
        {
            base.OnValidate();

            _levels = GetComponentsInChildren<World.Level>(true);
            _selectedLevel = _levels.Length == 0 ? null : _levels[0];
            if (_characterSelect == null) _characterSelect = FindObjectOfType<UI.CharacterSelect>();
            if (_startMenu == null) _startMenu = FindObjectOfType<UI.StartMenu>();
        }


        public override void Awake()
        {
            base.Awake();

            Layers = new Layers();

            if (_randomizeSeed) UnityEngine.Random.InitState(Environment.TickCount);

            _transitionTimer = new Timer(_transitionTime, start: false, repeat: false);

            _transitionTimer.OnTimeLimitHandler += OnTransitionTimeOut;            

            Podium.Instance.OnPodiumFinishedHandler += OnPodiumFinished;

            _characterSelect.OnCharacterSelectReadyHandler += OnCharacterSelectReady;

            TryChangeState(State.Menu);
        }

        public override void Start()
        {
            base.Start();

            _initialVectorBottomLeft = CameraManager.Instance.Camera.ScreenToWorldPoint(new Vector3(0, 0, 30));
            _initialVectorTopRight = CameraManager.Instance.Camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 30)); // I used 30 as my camera z is -30
        }

        public void Update()
        {
            _updatedVectorBottomLeft = CameraManager.Instance.Camera.ScreenToWorldPoint(new Vector3(0, 0, 30));
            _updatedVectorTopRight = CameraManager.Instance.Camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 30));

            if (
                _initialVectorBottomLeft != _updatedVectorBottomLeft || 
                _initialVectorTopRight != _updatedVectorTopRight)
            {
                OnScreenResizedHandler?.Invoke();
            }

            FSMUpdate();
        }

        public void FixedUpdate()
        {
            FSMFixedUpdate();
        }

        public void DoStart()
        {
            TryChangeState(State.Transition, State.CharacterSelection);
        }

        public void OnCharacterSelectReady(int playerCount)
        {
            TryChangeState(State.Transition, State.LevelSelection);
        }

        //private void OnScoreValueAdded(World.Objects.Gem gem, int player, float value)
        //{
        //    LocalPlayerManager.Instance.Players[player].Score += value;
        //    UI.HUD.Instance.OnScoreChanged(player, LocalPlayerManager.Instance.Players[player].Score);
        //}

        public void OnLevelCompleted(World.Level.Rule rule)
        {
            _round.Terminate();
        }

        public void OnLevelSelected(int step)
        {
            for (int i = 0; i < _levels.Length; i++)
            {
                if (_levels[i] == null) continue;
                _levels[i].TargetPosition = Vector3.zero + Vector3.right * (i - _currentLevelIndex) * _distanceLevelSelect;
            }

            _selectedLevel = _levels[_currentLevelIndex];

            _targetSizeCamera = _selectedLevel.CameraSize;

            OnLevelSelectedHandler?.Invoke(_selectedLevel, step);
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
            OnLevelSelectHandler?.Invoke(true);
        }

        public void OnWaiting()
        {
            UI.HUD.Instance.OnWaiting();
        }

        public void OnRoundEnd()
        {
            _roundIndex++;

            if (_roundIndex < _roundAmount)
            {
                _currentLevel.OnRoundEnd();
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
            if (_state == State.FinalPodium) TryChangeState(State.Transition, State.LevelSelection);            
            else
            {
                if (_currentLevel != null)
                {
                    Destroy(_currentLevel.gameObject);
                    _currentLevel = null;
                }

                TryChangeState(State.Transition, State.Round);
            }
        }

        private void OnTransitionTimeOut()
        {
            switch (_transition)
            {
                case State.Podium:
                case State.FinalPodium:
                    Destroy(_currentLevel.gameObject);
                    break;

                case State.Round:
                case State.LevelSelection:
                    Podium.Instance.gameObject.SetActive(false);
                    break;

                    //case State.Round:
                    //    _podium.gameObject.SetActive(false);
                    //    break;
            }

            TryChangeState(_transition);
        }


        #endregion

        #region FSM

        [Serializable]
        public enum State
        {
            Menu,
            CharacterSelection,
            LevelSelection,
            Begin,
            Round,
            Score,
            WaitingNextRound,
            Podium,
            FinalPodium,
            Transition,
            Session,
        }

        [SerializeField]
        public State _state = State.LevelSelection;

        public void FSMFixedUpdate()
        {
            switch (_state)
            {
                case State.Menu:
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
                            _cameraSizeSpeed);

                    break;

                case State.Session:
                    if (GameSession.Instance == null) return;
                    break;
            }
        }

        public void FSMUpdate()
        {
            switch (_state)
            {

                case State.Round:

                    foreach (Player player in _localPlayers)
                    {
                        player._character.TryMove(player.AxisLeft);
                    }

                    break;

                case State.Menu:
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
            if (TryTransition(transition, out State destination))
            {
                ExitState(destination);
                return TryFinishChangeState(destination, args);
            }

            return false;
        }

        public virtual void ExitState(State destination)
        {
            switch (_state)
            {
                case State.Menu:
                    break;

            }
        }


        private bool TryTransition(State transition, out State destination, params object[] args)
        {
            switch (_state)
            {
                case State.Menu:

                    switch (transition)
                    {
                        case State.Round:
                        case State.Menu:
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

                case State.CharacterSelection:

                    switch (transition)
                    {
                        case State.Round:
                        case State.Menu:
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
                        case State.Menu:
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
                        case State.Menu:
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
                        case State.Menu:
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
                        case State.Menu:
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
                        case State.Menu:
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
                        case State.Menu:
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
                        case State.Menu:
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

        public IEnumerator NewRoundCoroutine()
        {
            yield return new WaitForEndOfFrame();

            OnNewRoundHandler?.Invoke(_round);

            //_round.OnRoundBeginHandler += _currentLevel.OnBeginRound;

            _round.OnRoundEndHandler += OnRoundEnd;

            _round.BeginIntermission();

            yield return null;
        }

        private bool TryFinishChangeState(State target, params object[] args)
        {
            switch (target)
            {
                case State.Menu:
                    _state = target;
                    return true;

                case State.CharacterSelection:
                    OnCharacterSelectHandler?.Invoke(true);
                    _state = target;
                    return true;

                case State.Begin:
                    Podium.Instance.Clear();

                    foreach (var player in _localPlayers)
                    {
                        if (player == null) continue;
                        //Podium.Instance.Add(player, player._characterResource);
                    }

                    Podium.Instance.gameObject.SetActive(false);

                    OnLevelSelectHandler.Invoke(false);

                    foreach (World.Level level in _levels)
                    {
                        if (level == null)
                            continue;

                        if (level == _selectedLevel)
                            continue;

                        level.gameObject.SetActive(false);
                    }

                    _selectedLevel.gameObject.SetActive(false);

                    _state = target;
                    return TryChangeState(State.Round, _roundTime);


                case State.Transition:
                    _transition = (State)args[0];
                    _transitionTimer.Start();
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

                    foreach (World.Level lv in _levels)
                    {
                        lv.gameObject.SetActive(true);
                        lv.OnLevelSelect();
                    }

                    OnLevelSelect();
                    OnLevelSelected(0);

                    return true;

                case State.Round:

                    // TODO enable

                    _selectedLevel.TargetPosition = Vector3.zero;
                    _selectedLevel.transform.position = Vector3.zero;

                    _selectedLevel.gameObject.SetActive(true);

                    _currentLevel =
                        Instantiate(
                            _selectedLevel.gameObject,
                            Vector3.zero, Quaternion.identity,
                            gameObject.transform).GetComponent<World.Level>();

                    _selectedLevel.gameObject.SetActive(false);

                    //_currentLevel.OnScoreValueAddedHandler += OnScoreValueAdded;

                    _currentLevel.OnLevelCompletedHandler += OnLevelCompleted;

                    List<Placeholder> placeholders = new List<Placeholder>();
                    placeholders.AddRange(_currentLevel._characterPlaceholders);

                    int i = 0;
                    while (!placeholders.IsEmpty())
                    {
                        Placeholder placeholder = placeholders.RemoveRandom();

                        //_localPlayers[i]._character = _localPlayers[i]
                        //    ._characterResource.Create(
                        //        _currentLevel.GridToWorld(placeholder._gridPosition),
                        //        _currentLevel.transform);

                        //_localPlayers[i]._character.ColorId = _localPlayers[i].ServerId;

                        //_localPlayers[i]._character.Color = _localPlayers[i].Color;

                        _localPlayers[i]._character._level = _currentLevel;

                        _localPlayers[i]._character.TryChangeState(World.Objects.BaseObject.State.Disabled);

                        //_localPlayers[i].Score = 0;

                        //_localPlayers[i]._colorId = placeholder.ColorId;

                        i++;

                        Destroy(placeholder.gameObject);
                    }

                    _round =
                        new Round(
                            _countDown,
                            _roundTime,
                            _countDownTime,
                            _intermissionTime,
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

                    foreach (Player player in PlayerManager.Instance.LocalPlayers)
                    {
                        if (player == null) continue;

                        //ctrl.Character = null;
                    }

                    _state = target;

                    OnWaiting();
                    return true;

                default: return false;
            }
        }

        private bool[] _wasMovingVertical = new bool[PlayerManager.Max];

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
                        player._characterSlot.CmdScroll(step.z > 0);                        
                    }

                    break;

                case State.LevelSelection:

                    if (Mathf.Abs(step.x) > 0)
                    {

                        int prev = _currentLevelIndex;

                        _currentLevelIndex =
                            Mathf.Clamp(_currentLevelIndex + (int)Mathf.Sign(step.x), 0, _levels.Length - 1);

                        if (prev != _currentLevelIndex)
                        {
                            OnLevelSelected((int)Mathf.Sign(step.x));
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

                    if (_localPlayers.Contains(player))
                    {
                        UI.HUD.Instance.Leave(player);
                        _localPlayers.Remove(player);
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
