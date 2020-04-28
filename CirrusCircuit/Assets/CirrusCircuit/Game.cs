using Cirrus.Circuit.Controls;
using Cirrus.Circuit.Networking;
using Cirrus.Circuit.World.Objects.Characters;
using Cirrus.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cirrus.Events;
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

        [SerializeField]
        private bool _randomizeSeed = false;
        public bool IsSeedRandomized => _randomizeSeed;

        public Events.Event OnScreenResizedHandler;                

        [SerializeField]
        public World.Level[] _levels;

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
            TryChangeState(State.Menu);
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

        private void OnTransitionTimeOut()
        {
            TryChangeState(_transition);
        }

        public void FSMFixedUpdate()
        {
            switch (_state)
            {
                case State.Unknown:                    
                case State.Transition:
                case State.Menu:
                    break;
               
                case State.Session:
                    CameraManager.Instance.Camera.orthographicSize =
                        Mathf.Lerp(
                            CameraManager.Instance.Camera.orthographicSize,
                            GameSession.Instance._targetSizeCamera,
                            _cameraSizeSpeed);

                    break;
            }
        }

        public void FSMUpdate()
        {
            switch (_state)
            {
                case State.Menu:
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
                case State.Unknown:
                    switch (target)
                    {
                        case State.Menu:
                        case State.Transition:
                        case State.Session:
                            //case State.Round:
                            destination = target;
                            return true;
                    }
                    break;

                case State.Menu:
                    switch (target)
                    {                        
                        case State.Menu:
                        case State.Transition:
                        case State.Session:
                        //case State.Round:
                            destination = target;
                            return true;
                    }
                    break;

                case State.Session:
                    switch (target)
                    {
                        case State.Menu:
                        case State.Transition:
                        case State.Session:
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
                        case State.Session:

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
                case State.Unknown:
                    _state = target;
                    return true;

                case State.Menu:
                    _state = target;
                    StartMenu.Instance.Enabled = true;
                    return true;

                case State.Session:
                    _state = target;
                    StartMenu.Instance.Enabled = false;
                    return true;

                case State.Transition:
                    _transition = (State)args[0];                    
                    Transitions.Transition.Instance.Perform();
                    _state = target;
                    return true;                                                   

                default: return false;
            }
        }       
    }
}
