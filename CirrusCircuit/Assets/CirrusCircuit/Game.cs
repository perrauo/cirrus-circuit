using Cirrus.Circuit.Controls;
using Cirrus.Circuit.Networking;
using Cirrus.Circuit.World.Objects.Characters;
using Cirrus;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Cirrus.Events;
using Cirrus.Circuit.World;
using StartMenu = Cirrus.Circuit.UI.StartMenu;
using System.Threading;
using Cirrus.Circuit.UI;
using Random = UnityEngine.Random;

namespace Cirrus.Circuit
{
    ///
    public static class Layers
    {
        //public int 
        public static int MoveableFlags = 1 << LayerMask.NameToLayer("Moveable");
        public static int SolidFlags = 1 << LayerMask.NameToLayer("Solid");
        public static int Moveable = LayerMask.NameToLayer("Moveable");
        public static int Solid = LayerMask.NameToLayer("Solid");
    }

    public class Game : BaseSingleton<Game>
    {
        #region Events

        public Delegate<bool> OnCharacterSelectHandler;

        public Delegate<bool> OnMenuHandler;

        public Delegate<bool> OnLevelSelectHandler;

        public Delegate<Level, int> OnLevelSelectedHandler;

        public Delegate<Level, int> OnLevelScrollHandler;

        public Delegate OnRoundHandler;

        public Delegate OnRoundInitHandler;


        public Delegate OnScreenResizedHandler;

        public Delegate OnPodiumHandler;

        public Delegate OnFinalPodiumHandler;

        #endregion


        [Serializable]
        public enum State
        {
            Unknown,
            Menu,
            Session,                            
            CharacterSelection,
            LevelSelection,
            InitRound,
            Round,
            Score,
            //WaitingNextRound,
            Podium,
            FinalPodium
        }        

        [SerializeField]
        public State _state = State.Menu;

        [SerializeField]
        public State _nextState = State.Unknown;

        [SerializeField]
        private bool _randomizeSeed = false;
        public bool IsSeedRandomized => _randomizeSeed;


        [SerializeField]
        private bool _isRainEnabled = false;
        public bool IsRainEnabled => _isRainEnabled;


        [SerializeField]
        public Level[] _levels;

        [SerializeField]
        public float _distanceLevelSelect = 35;
        public float DistanceLevelSelect => _distanceLevelSelect;   

        [SerializeField]        
        public float _cameraSizeSpeed = 0.8f;
        public float CameraSizeSpeed => _cameraSizeSpeed;

        [SerializeField]
        public float _roundTime = 60f;
        public float RoundTime => _roundTime;

        [SerializeField]
        public float _countdownTime = 5f;
        public float CountdownTime => _countdownTime;

        // TODO
        [SerializeField]
        public int _countDown = 3;
        public int CountDown => _countDown;
    
        [SerializeField]
        private int _numRounds = 3;
        public int NumRounds => _numRounds;
    
        [SerializeField]
        public float _podiumTransitionSpeed = 0.2f;
        public float PodiumTransitionSpeed => _podiumTransitionSpeed;

        [SerializeField]
        private bool _isFullScreen = false;
        public bool IsFullScreen => _isFullScreen;

        [SerializeField]
        private float _intermissionTime = 2f;
        public float IntermissionTime => _intermissionTime;

        [SerializeField]
        private float _transitionDistance = 48f;
        public float TransitionDistance => _transitionDistance;

        [SerializeField]
        public float _targetSizeCamera = 10f;

        private Vector3 _initialVectorBottomLeft;

        private Vector3 _initialVectorTopRight;

        private Vector3 _updatedVectorBottomLeft;

        private Vector3 _updatedVectorTopRight;

        public int SelectedLevelIndex => GameSession.Instance.SelectedLevelIndex;

        private bool[] _wasMovingVertical = new bool[PlayerManager.PlayerMax];

        //Threading.CoroutineBarrier _roundInitBarrier;

        public override void OnValidate()
        {
            base.OnValidate();

            _levels = GetComponentsInChildren<Level>(true);
        }

        public override void Awake()
        {
            base.Awake();

            if (IsSeedRandomized) Random.InitState(Environment.TickCount);
        }

        public override void Start()
        {
            base.Start();

            _initialVectorBottomLeft = CameraController.Instance.Camera.ScreenToWorldPoint(new Vector3(0, 0, 30));
            _initialVectorTopRight = CameraController.Instance.Camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 30)); // I used 30 as my camera z is -30

            Screen.fullScreen = IsFullScreen; 

            SetState(
                State.Menu, 
                false);            
        }


        public void Update()
        {
            _updatedVectorBottomLeft = CameraController.Instance.Camera.ScreenToWorldPoint(new Vector3(0, 0, 30));
            _updatedVectorTopRight = CameraController.Instance.Camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 30));

            if (
                _initialVectorBottomLeft != _updatedVectorBottomLeft || 
                _initialVectorTopRight != _updatedVectorTopRight)
            {
                OnScreenResizedHandler?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.F)) Screen.fullScreen = !Screen.fullScreen;

            FSM_Update();
        }

        public void FixedUpdate()
        {
            FSM_FixedUpdate();
        }

        public void OnPodiumFinished()
        {
            if (CustomNetworkManager.IsServer)
            {
                if (_state == State.FinalPodium) Cmd_SetState(State.LevelSelection);

                else Cmd_SetState(State.InitRound);
            }
        }

        public void JoinSession()
        {
            SetState(State.CharacterSelection);
        }

        public void OnCharacterSelected(int playerCount)
        {            
            Cmd_SetState(State.LevelSelection);            
        }

        public void OnRoundEnd()
        {
            if (CustomNetworkManager.IsServer)
            {
                GameSession.Instance.RoundIndex++;

                if (GameSession.Instance.RoundIndex < NumRounds) Cmd_SetState(State.Podium);

                else
                {
                    GameSession.Instance.RoundIndex = 0;
                    Cmd_SetState(State.FinalPodium);
                }
            }
        }

        // TODO: Simulate LeftStick continuous axis with WASD
        public void HandleAxesLeft(Player player, Vector2 axis)
        {
            FSM_HandleAxesLeft(player, axis);
        }

        public void HandleAction0(Player player)
        {
            FSM_HandleAction0(player);
        }

        public void HandleAction1(Player player)
        {
            FSM_HandleAction1(player);
        }

        public void Cmd_ScrollLevel(int delta)
        {
            if (CustomNetworkManager.IsServer)
            {
                CommandClient.Instance.Cmd_Game_ScrollLevel(delta);
            }
        }

        // TODO change for SyncVar hook
        public void SelectLevel(int index)
        {            
            for (int i = 0; i < _levels.Length; i++)
            {
                if (_levels[i] == null) continue;

                _levels[i].TargetPosition = Vector3.right * (i - index) * Instance.DistanceLevelSelect;
            }            

            _targetSizeCamera = _levels[index].CameraSize;

            OnLevelSelectedHandler?.Invoke(_levels[index], index);
        }

        public void ScrollLevel(int step)
        {
            OnLevelScrollHandler
                ?.Invoke(
                    GameSession.Instance.SelectedLevel, 
                    step);
        }

        #region FSM

        public void FSM_FixedUpdate()
        {
            switch (_state)
            {
                case State.CharacterSelection:
                case State.InitRound:
                case State.LevelSelection:
                case State.Round:
                case State.Score:
                case State.Podium:
                case State.FinalPodium:
                    CameraController
                        .Instance
                        .Camera.orthographicSize =
                            Mathf.Lerp(
                                CameraController
                                    .Instance
                                    .Camera
                                    .orthographicSize,
                                _targetSizeCamera,
                                CameraSizeSpeed);

                    break;
            }
        }

        public void FSM_Update()
        {
            switch (_state)
            {

                case State.Round:

                    foreach (var player in PlayerManager.Instance.LocalPlayers)
                    {
                        if (player == null) continue;

                        if (player._character == null) continue;

                        if (player.IsAxesLeft) player._character.Cmd_Move(player.AxisLeft);
                        
                        
                    }
                    break;

                case State.InitRound:
                case State.LevelSelection:
                case State.Score:
                case State.Podium:
                case State.FinalPodium:
                    break;
            }
        }

        public bool Cmd_SetState(
            State transition, 
            bool transitionEffect = true)
        {
            if (CustomNetworkManager.IsServer)
            {
                CommandClient.Instance.Cmd_Game_SetState(transition, transitionEffect);
                return true;
            }
            return false;
        }

        public void SetState(
            State transition, 
            bool transitionEffect=true)
        {
            if (transitionEffect)
            {
                _nextState = transition;
                Transitions
                    .Transition
                    .Instance
                    .Perform();
            }
            else if (GetAllowedTransition(
                transition, 
                out State destination))
            {
                ExitState(destination);
                InitState(destination);
            }
        }

        public void OnTransitionTimeOut()
        {
            if (GetAllowedTransition(
                _nextState, 
                out State destination))
            {
                ExitState(destination);
                InitState(destination);
            }
        }

        private void ExitState(State destination)
        {
            switch (_state)
            {
                case State.Menu:
                    OnMenuHandler.Invoke(false);
                    break;

                case State.LevelSelection:
                    OnLevelSelectHandler.Invoke(false);
                    break;

                case State.CharacterSelection:
                    OnCharacterSelectHandler?.Invoke(false);
                    break;

                case State.FinalPodium:
                case State.Podium:
                    Podium
                        .Instance
                        .gameObject
                        .SetActive(false);
                    break;

                default: break;
            }
        }

        private bool GetAllowedTransition(
            State transition, 
            out State destination, 
            params object[] args)
        {
            switch (_state)
            {
                case State.Unknown:

                    switch (transition)
                    {
                        case State.Menu:
                        case State.Round:
                        case State.CharacterSelection:
                        case State.InitRound:                        
                        case State.LevelSelection:
                        case State.Score:
                        case State.Podium:
                        case State.FinalPodium:

                            destination = transition;
                            return true;
                    }
                    break;


                case State.Menu:

                    switch (transition)
                    {
                        case State.Menu:
                        case State.Round:
                        case State.CharacterSelection:
                        case State.InitRound:                        
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
                        case State.Menu:
                        case State.Round:
                        case State.CharacterSelection:
                        case State.InitRound:                        
                        case State.LevelSelection:
                        case State.Score:
                        case State.Podium:
                        case State.FinalPodium:

                            destination = transition;
                            return true;
                    }
                    break;

                case State.InitRound:

                    switch (transition)
                    {
                        case State.Menu:
                        case State.Round:
                        case State.CharacterSelection:
                        case State.InitRound:                        
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
                        case State.InitRound:                        
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
                        case State.InitRound:                        
                        case State.LevelSelection:
                        case State.Score:
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
                        case State.InitRound:                        
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
                        case State.InitRound:                      
                        case State.LevelSelection:
                        case State.Round:
                        case State.Podium:
                        case State.FinalPodium:

                            destination = transition;
                            return true;
                    }
                    break;
            }

            destination = State.Unknown;
            return false;
        }
       

        protected bool InitState(
            State target, 
            params object[] args)
        {
            switch (target)
            {
                case State.Menu:
                    OnMenuHandler?.Invoke(true);
                    _state = target;
                    return true;

                case State.CharacterSelection:

                    GameSession.Instance.SelectedLevelIndex = 0;

                    OnCharacterSelectHandler?.Invoke(true);
                    _state = target;
                    return true;
        
                case State.Podium:
                    if (RoundSession.Instance != null) RoundSession.Instance.Destroy();
                    if (LevelSession.Instance != null) LevelSession.Instance.Destroy();
                    Podium.Instance.gameObject.SetActive(true);
                    OnPodiumHandler?.Invoke();
                    _state = target;
                    return true;

                case State.FinalPodium:
                    if (RoundSession.Instance != null) RoundSession.Instance.Destroy();
                    if (LevelSession.Instance != null) LevelSession.Instance.Destroy();
                    Podium.Instance.gameObject.SetActive(true);
                    OnFinalPodiumHandler?.Invoke();
                    _state = target;
                    return true;

                case State.LevelSelection:
                    if (RoundSession.Instance != null) RoundSession.Instance.Destroy();
                    if (LevelSession.Instance != null) LevelSession.Instance.Destroy();

                    OnLevelSelectHandler?.Invoke(true);

                    GameSession.Instance.SelectedLevelIndex = 0;

                    _state = target;

                    foreach (Level level in _levels)
                    {
                        if (level == null) continue;
                        level.gameObject.SetActive(true);
                        level.OnLevelSelect();
                    }             

                    return true;


                case State.InitRound:

                    Podium.Instance.Clear();

                    foreach (var player in GameSession.Instance.Players)
                    {
                        if (player == null)
                            continue;

                        Podium.Instance.Add(
                            player,
                            CharacterLibrary.Instance.Characters[player.CharacterId]);
                    }

                    Podium.Instance.gameObject.SetActive(false);

                    foreach (Level level in _levels)
                    {
                        if (level == null) continue;
                        level.gameObject.SetActive(false);
                    }

                    if (CustomNetworkManager.IsServer)
                    {
                        //LevelSession.Instance = null;
                        //LevelRoun
                        if (RoundSession.Instance != null) RoundSession.Instance.Destroy();
                        if (LevelSession.Instance != null) LevelSession.Instance.Destroy();

                        LevelSession.Create();
                        RoundSession.Create(
                            CountDown,
                            RoundTime,
                            CountdownTime,
                            IntermissionTime,
                            GameSession.Instance._roundIndex);
                    }

                    Threading.CoroutineBarrier.Wait(
                        this,
                        () =>
                        {
                            OnRoundInitHandler?.Invoke();
                            SetState(State.Round);
                            RoundSession.Instance.StartIntermisison();
                        },
                        () => RoundSession.Instance != null && RoundSession.Instance.IsClientStarted,
                        () => LevelSession.Instance != null && LevelSession.Instance.IsClientStarted
                        );

                    _state = target;

                    return true;

                case State.Round:

                    OnRoundHandler?.Invoke();
                    Announcement.Instance.RoundIndex = RoundSession.Instance.Index;

                    _state = target;

                    return true;



                case State.Score:

                    _state = target;

                    return true;

                default:
                    return false;
            }

        }



        // TODO: Simulate LeftStick continuous axis with WASD
        public void FSM_HandleAxesLeft(Player player, Vector2 axis)
        {
            bool isMovingHorizontal = Mathf.Abs(axis.x) > 0.5f;
            bool isMovingVertical = Mathf.Abs(axis.y) > 0.5f;

            Vector3 stepHorizontal = new Vector3(Mathf.Sign(axis.x), 0, 0);
            Vector3 stepVertical = new Vector3(0, 0, Mathf.Sign(axis.y));
            Vector3 step = Vector3.zero;

            if (isMovingVertical && isMovingHorizontal)
            {
                // Moving in both directions, prioritize later
                if (_wasMovingVertical[player.LocalId]) step = stepHorizontal;
                else step = stepVertical;
            }
            else if (isMovingHorizontal)
            {
                step = stepHorizontal;
                _wasMovingVertical[player.LocalId] = false;
            }
            else if (isMovingVertical)
            {
                step = stepVertical;
                _wasMovingVertical[player.LocalId] = true;
            }

            switch (_state)
            {
                case State.CharacterSelection:

                    if (Mathf.Abs(step.z) > 0)
                    {
                        if (player._characterSlot == null) return;
                        player._characterSlot.Cmd_Scroll(step.z > 0);
                    }

                    break;

                case State.LevelSelection:

                    if (!CustomNetworkManager.IsServer) break;

                    if (Mathf.Abs(step.x) > 0)
                    {
                        int prev = SelectedLevelIndex;
                        int delta = (int)Mathf.Sign(step.x);

                        GameSession.Instance.SelectedLevelIndex =
                            Mathf.Clamp(
                                SelectedLevelIndex + delta, 
                                0, 
                                _levels.Length - 1);

                        if (prev != SelectedLevelIndex) Cmd_ScrollLevel(delta);                                                    
                    }

                    break;

                case State.Round:
                    if (player._character != null) player._character?.Cmd_Move(axis);

                    break;

                case State.Score:
                    break;

                default:
                    break;
            }
        }

        public void FSM_HandleAction0(Player player)
        {
            switch (_state)
            {
                case State.LevelSelection:
                    break;

                case State.CharacterSelection:

                    if (player._characterSlot != null)
                    {
                        player._characterSlot.HandleAction0();

                        if (
                            player._characterSlot.State == 
                            CharacterSelectSlotState.Selecting)
                        {
                            CustomNetworkManager
                              .Instance
                              .RequestPlayerLeave(player.ServerId);
                        }
                    }

                    break;

                case State.Round:
                    if (player._character) player._character?.HandleAction0();

                    break;

                case State.Score:
                    break;
            }
        }

        public void FSM_HandleAction1(Player player)
        {
            switch (_state)
            {
                case State.Podium:

                    break;

                case State.LevelSelection:
                        Cmd_SetState(State.InitRound, false);
                    
                    break;


                case State.CharacterSelection:

                    if (player.ServerId < 0)
                    {
                        CustomNetworkManager
                              .Instance
                              .RequestJoinPlayer(player);
                    }
                    else if(player._characterSlot != null)
                    {
                        player._characterSlot.HandleAction1(player);                        
                    }

                    break;

                case State.Round:
                    if (player._character) player._character?.DoAction1();
                    break;

                case State.Score:
                    break;
            }

        }

        #endregion      
    }
}
