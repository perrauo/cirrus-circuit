using Cirrus.Circuit.Controls;
using Cirrus.Circuit.Networking;
using Cirrus.Circuit.World.Objects.Characters;
using Cirrus.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cirrus.Events;
using Cirrus.Circuit.World;
using StartMenu = Cirrus.Circuit.UI.StartMenu;

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

        public Event<bool> OnCharacterSelectHandler;

        public Event<bool> OnMenuHandler;

        public Event<bool> OnLevelSelectHandler;

        public Event<Level, int> OnLevelSelectedHandler;

        public Event<Level, int> OnLevelScrollHandler;

        public Events.Event OnRoundStartedHandler;


        public Events.Event OnScreenResizedHandler;

        public Events.Event OnPodiumHandler;

        public Events.Event OnFinalPodiumHandler;

        #endregion


        [Serializable]
        public enum State
        {
            Unknown,
            Menu,
            Session,                            
            CharacterSelection,
            LevelSelection,
            StartRound,
            Round,
            Score,
            WaitingNextRound,
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
        public float _countDownTime = 5f;
        public float CountDownTime => _countDownTime;

        [SerializeField]
        public int _countDown = 3;
        public int CountDown => _countDown;
    
        [SerializeField]
        private int _roundAmount = 3;
        public int RoundAmount => _roundAmount;
    
        [SerializeField]
        public float _podiumTransitionSpeed = 0.2f;
        public float PodiumTransitionSpeed => _podiumTransitionSpeed;

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

        public override void OnValidate()
        {
            base.OnValidate();

            _levels = GetComponentsInChildren<World.Level>(true);
        }

        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();

            _initialVectorBottomLeft = CameraManager.Instance.Camera.ScreenToWorldPoint(new Vector3(0, 0, 30));
            _initialVectorTopRight = CameraManager.Instance.Camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 30)); // I used 30 as my camera z is -30

            Transitions.Transition.Instance.OnTransitionTimeoutHandler += OnTransitionTimeOut;
            UI.CharacterSelect.Instance.OnCharacterSelectReadyHandler += OnCharacterSelected;

            _SetState(State.Menu, false);            
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

        public void JoinSession()
        {
            _SetState(State.CharacterSelection);
        }

        public void OnCharacterSelected(int playerCount)
        {
            Cmd_SetState(State.LevelSelection);
        }


        // TODO: Simulate LeftStick continuous axis with WASD
        public void HandleAxesLeft(Player player, Vector2 axis)
        {
            FSMHandleAxesLeft(player, axis);
        }

        public void HandleAction0(Player player)
        {
            FSMHandleAction0(player);
        }

        public void HandleAction1(Player player)
        {
            FSMHandleAction1(player);
        }

        public void Cmd_ScrollLevel(int delta)
        {
            ClientPlayer.Instance.Cmd_Game_ScrollLevel(delta);
        }

        // TODO change for SyncVar hook
        public void _SelectLevel(int index)
        {            
            for (int i = 0; i < _levels.Length; i++)
            {
                if (_levels[i] == null) continue;

                _levels[i].TargetPosition = Vector3.right * (i - index) * Instance.DistanceLevelSelect;
            }            

            _targetSizeCamera = _levels[index].CameraSize;

            OnLevelSelectedHandler?.Invoke(_levels[index], index);
        }

        public void _ScrollLevel(int step)
        {
            OnLevelScrollHandler?.Invoke(GameSession.Instance.SelectedLevel, step);
        }

        #region FSM


        public void FSMFixedUpdate()
        {
            switch (_state)
            {
                case State.CharacterSelection:
                case State.StartRound:
                case State.LevelSelection:
                case State.Round:
                case State.Score:
                case State.Podium:
                case State.FinalPodium:
                    CameraManager.Instance.Camera.orthographicSize =
                        Mathf.Lerp(
                            CameraManager.Instance.Camera.orthographicSize,
                            _targetSizeCamera,
                            CameraSizeSpeed);

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
                case State.StartRound:
                case State.LevelSelection:
                case State.Score:
                case State.Podium:
                case State.FinalPodium:
                    break;
            }
        }

        public bool Cmd_SetState(State transition, bool transitionEffect = true)
        {
            ClientPlayer.Instance.Cmd_Game_SetState(transition, transitionEffect);
            return true;
        }

        public void _SetState(State transition, bool transitionEffect=true)
        {
            if (transitionEffect)
            {
                _nextState = transition;
                Transitions.Transition.Instance.Perform();
            }
            else if (TryTransition(transition, out State destination))
            {
                ExitState(destination);
                TryFinishSetState(destination);
            }
        }


        private void OnTransitionTimeOut()
        {
            if (TryTransition(_nextState, out State destination))
            {
                ExitState(destination);
                TryFinishSetState(destination);
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

                default: break;
            }
        }

        private bool TryTransition(State transition, out State destination, params object[] args)
        {
            switch (_state)
            {
                case State.Unknown:

                    switch (transition)
                    {
                        case State.Menu:
                        case State.Round:
                        case State.CharacterSelection:
                        case State.StartRound:
                        case State.WaitingNextRound:
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
                        case State.StartRound:
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
                        case State.Menu:
                        case State.Round:
                        case State.CharacterSelection:
                        case State.StartRound:
                        case State.WaitingNextRound:
                        case State.LevelSelection:
                        case State.Score:
                        case State.Podium:
                        case State.FinalPodium:

                            destination = transition;
                            return true;
                    }
                    break;

                case State.StartRound:

                    switch (transition)
                    {
                        case State.Menu:
                        case State.Round:
                        case State.CharacterSelection:
                        case State.StartRound:
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
                        case State.StartRound:
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
                        case State.StartRound:
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
                        case State.Menu:
                        case State.Round:
                        case State.CharacterSelection:
                        case State.StartRound:
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
                        case State.StartRound:
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
                        case State.StartRound:
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
    

        protected bool TryFinishSetState(State target, params object[] args)
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

                    OnLevelSelectHandler?.Invoke(true);

                    GameSession.Instance.SelectedLevelIndex = 0;

                    _state = target;

                    foreach (Level level in _levels)
                    {
                        level.gameObject.SetActive(true);
                        level.OnLevelSelect();
                    }             

                    return true;


                case State.StartRound:

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
                        if (level == GameSession.Instance.SelectedLevel) continue;
                        level.gameObject.SetActive(false);
                    }

                    GameSession.Instance.SelectedLevel.gameObject.SetActive(false);
                    CustomNetworkManager.Instance.StartRound();

                    _state = target;
                    return true;

                case State.Round:

                    OnRoundStartedHandler?.Invoke();

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
                //moving in both directions, prioritize later
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
                        player._characterSlot.Scroll(step.z > 0);
                    }

                    break;

                case State.LevelSelection:

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

                case State.WaitingNextRound:

                    break;

                case State.Round:
                    if (player._character != null) player._character?.TryMove(axis);

                    break;

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

                    //if (LocalPlayers.Contains(player))
                    //{
                    //    UI.HUD.Instance.Leave(player);
                    //    LocalPlayers.Remove(player);
                    //}
                    //else
                    //{
                    //    foreach (Player other in PlayerManager.Instance.LocalPlayers) if (other == null) continue;
                    //    TrySetState(State.LevelSelection);
                    //}

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
                    Cmd_SetState(State.StartRound);
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
