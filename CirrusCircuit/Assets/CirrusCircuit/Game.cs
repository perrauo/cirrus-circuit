using Cirrus.Circuit.Controls;
using Cirrus.Circuit.Networking;
using Cirrus.Circuit.World.Objects.Characters;
using Cirrus.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cirrus.Events;

namespace Cirrus.Circuit
{
    ///
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
        #region Game

        [SerializeField]
        private bool _randomizeSeed = false;
        public bool IsSeedRandomized => _randomizeSeed;

        public Events.Event OnScreenResizedHandler;                

        public Event<World.Level, int> OnLevelSelectedHandler;

        public Event<bool> OnMenuHandler;

        public Event<Player> OnLocalPlayerJoinHandler;

        public Layers Layers;

        [SerializeField]
        public World.Level[] _levels;

        public World.Level _selectedLevel;

        public World.Level _currentLevel;

        public int _currentLevelIndex = 0;

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

        private State _transition;

        private Vector3 _initialVectorBottomLeft;

        private Vector3 _initialVectorTopRight;

        private Vector3 _updatedVectorBottomLeft;

        private Vector3 _updatedVectorTopRight;        

        public List<Player> LocalPlayers = new List<Player>();

        private bool[] _wasMovingVertical = new bool[PlayerManager.Max];
        
        public override void OnValidate()
        {
            base.OnValidate();

            _levels = GetComponentsInChildren<World.Level>(true);
            _selectedLevel = _levels.Length == 0 ? null : _levels[0];
        }

        public override void Awake()
        {
            base.Awake();

            Layers = new Layers();

            if (_randomizeSeed) UnityEngine.Random.InitState(Environment.TickCount);
 
                       
            TryChangeState(State.Menu);
        }

        public override void Start()
        {
            base.Start();

            _initialVectorBottomLeft = CameraManager.Instance.Camera.ScreenToWorldPoint(new Vector3(0, 0, 30));
            _initialVectorTopRight = CameraManager.Instance.Camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 30)); // I used 30 as my camera z is -30

            Transitions.Transition.Instance.OnTransitionTimeoutHandler += OnTransitionTimeOut;
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

        public void StartSession()
        {
            TryChangeState(State.Transition, State.Session);
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

        private void OnTransitionTimeOut()
        {
            TryChangeState(_transition);
        }

        #endregion

        #region FSM

        [Serializable]
        public enum State
        {
            Unknown,
            Menu,
            Session,
            Transition
        }

        [SerializeField]
        public State _state = State.Menu;

        public void FSMFixedUpdate()
        {
            switch (_state)
            {
                case State.Menu:                                 
                    break;

                case State.Session:
                    if (GameSession.Instance == null) return;
                    GameSession.Instance.FSMFixedUpdate();
                    break;
            }
        }

        public void FSMUpdate()
        {
            switch (_state)
            {
                case State.Menu:
                    break;

                case State.Session:
                    if (GameSession.Instance == null) return;
                    GameSession.Instance.FSMUpdate();
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

        private bool TryTransition(State target, out State destination, params object[] args)
        {
            switch (_state)
            {
                case State.Menu:
                    switch (target)
                    {                        
                        case State.Menu:
                        case State.Transition:
                        //case State.Round:
                            destination = target;
                            return true;
                    }
                    break;

                case State.Transition:
                    switch (target)
                    {
                        case State.Menu:
                        case State.Transition:

                            destination = target;
                            return true;
                    }
                    break;

            }

            destination = State.Menu;
            return false;
        }

        private bool TryFinishChangeState(State target, params object[] args)
        {
            switch (target)
            {
                case State.Menu:
                    _state = target;
                    return true;

                case State.Session:
                    _state = target;
                    return true;

                case State.Transition:
                    _transition = (State)args[0];                    
                    Transitions.Transition.Instance.Perform();
                    _state = target;
                    return true;                                                   

                default: return false;
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
                default:
                    break;
            }
        }

        public void FSMHandleAction0(Player player)
        {
            switch (_state)
            {
                
            }
        }

        public void FSMHandleAction1(Player player)
        {
            switch (_state)
            {
                default: break;
            }
        }

        #endregion

    }
}
