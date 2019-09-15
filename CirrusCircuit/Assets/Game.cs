using System;
using Cirrus.Circuit.Controls;
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

    public class Game : MonoBehaviour
    {
        #region Game

        public Objects.Door.OnScoreValueAdded OnScoreValueAddedHandler;

        private static Game _instance;

        public static Game Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<Game>(); ;
                }

                return _instance;

            }
        }


        [SerializeField]
        public Clock _clock;


        public Clock Clock
        {
            get
            {
                return _clock;
            }
        }

        [SerializeField]
        public Lobby Lobby;

        [SerializeField]
        public UI.HUD HUD;


        public Layers Layers;

        [SerializeField]
        public Level[] _levels;

        public Level CurrentLevel;

        public int _currentLevelIndex = 0;

        public float _distanceLevelSelect = 35;

        [SerializeField]
        public Camera  _camera;

        [SerializeField]
        private Objects.Podium _podium;


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


        public Round _round;

        [SerializeField]
        public float _podiumTransitionSpeed = 0.2f;


        void Awake()
        {
            //if (_instance != null)
            //{
            //    Destroy(gameObject);
            //    return;
            //}

            Layers = new Layers();
            DontDestroyOnLoad(this.gameObject);

            FSMAwake();
        }

        public void Start()
        {
            FSMStart();
            
        }

        public void FixedUpdate()
        {
            FSMFixedUpdate();
        }

        public void Update()
        {
            FSMUpdate();
        }


        public void OnValidate()
        {
            _levels = GetComponentsInChildren<Level>(true);
            CurrentLevel = _levels.Length == 0 ? null : _levels[0];
        }


        private void OnScoreValueAdded(PlayerNumber player, float value)
        {
            Lobby.Controllers[(int)player].Score += value;
            HUD.OnScoreChanged(player, Lobby.Controllers[(int)player].Score);
        }


        public void OnLevelSelected(int step)
        {
            for (int i = 0; i < _levels.Length; i++)
            {
                if (_levels[i] == null)
                    continue;

                _levels[i].TargetPosition = Vector3.zero + Vector3.right * (i - _currentLevelIndex) * _distanceLevelSelect;

            }

            Lobby.Characters.Clear();
            Lobby.Characters.AddRange(CurrentLevel.Characters);
            CurrentLevel = _levels[_currentLevelIndex];

            _targetSizeCamera = CurrentLevel.CameraSize;

            HUD.OnLevelSelected(step);

        }

        // TODO: Simulate LeftStick continuous axis with WASD
        public void HandleAxesLeft(Controller controller, Vector2 axis)
        {
            FSMHandleAxesLeft(controller, axis);
        }

        public void HandleAction0(Controller controller)
        {
            FSMHandleAction0(controller);
        }

        public void HandleAction1(Controller controller)
        {
            FSMHandleAction1(controller);
        }


        public void OnLevelSelect()
        {
            HUD.OnLevelSelect();
        }

        public void OnWaiting()
        {
            HUD.OnWaiting();
        }

        public void OnRoundEnd()
        {
            CurrentLevel.TargetPosition = Vector3.zero + Vector3.right * _distanceLevelSelect;
            CurrentLevel._positionSpeed = _podiumTransitionSpeed;

            _podium.transform.position = Vector3.zero + Vector3.right * -_distanceLevelSelect;
            _podium.gameObject.SetActive(true);
            _podium.TargetPosition = Vector3.zero;

            _podium.OnRoundEnd();

            foreach (var c in Lobby.Controllers)
            {
                if (c == null)
                    continue;

                _podium.Add(c, c.Character);
            }

            CurrentLevel.OnRoundEnd();   
            TryChangeState(State.Podium);
        }

        #endregion


        #region FSM

        [System.Serializable]
        public enum State
        {
            LevelSelection,
            Round,
            Score,
            WaitingNextRound,
            Podium
        }

        [SerializeField]
        public State _state = Game.State.LevelSelection;

        public void FSMAwake()
        {
            TryChangeState(State.LevelSelection);
        }


        public void FSMStart()
        {

        }

        public void FSMFixedUpdate()
        {
            switch (_state)
            {
                case State.LevelSelection:
                case State.Round:
                case State.Score:
                case State.Podium:

                    _camera.orthographicSize =
                        Mathf.Lerp(
                            _camera.orthographicSize,
                            _targetSizeCamera,
                            _cameraSizeSpeed);

                    break;
            }
        }

        public void FSMUpdate()
        {
            switch (_state)
            {
                case State.LevelSelection:
                case State.Round:
                case State.Score:
                case State.Podium:

                    break;
            }
        }

        public bool TryChangeState(State transition, params object[] args)
        {
            if (TryTransition(transition, out State destination))
            {
                return TryFinishChangeState(destination, args);
            }

            return false;
        }

        protected bool TryTransition(State transition, out State destination, params object[] args)
        {
            switch (_state)
            {
                case State.Round:

                    switch (transition)
                    {
                        case State.WaitingNextRound:
                        case State.LevelSelection:
                        case State.Score:
                        case State.Podium:
                            destination = transition;
                            return true;
                    }
                    break;

                case State.LevelSelection:
                    switch (transition)
                    {
                        case State.WaitingNextRound:
                        case State.LevelSelection:
                        case State.Score:
                        case State.Podium:
                            destination = transition;
                            return true;
                    }
                    break;

                case State.Score:
                    switch (transition)
                    {
                        case State.WaitingNextRound:
                        case State.LevelSelection:
                        case State.Round:
                        case State.Podium:
                            destination = transition;
                            return true;
                    }
                    break;

                case State.WaitingNextRound:
                    switch (transition)
                    {
                        case State.LevelSelection:
                        case State.Round:
                        case State.Podium:
                            destination = transition;
                            return true;
                    }
                    break;

                case State.Podium:
                    switch (transition)
                    {
                        case State.WaitingNextRound:
                        case State.LevelSelection:
                        case State.Round:
                        case State.Podium:
                            destination = transition;
                            return true;
                    }
                    break;
            }

            destination = State.Round;
            return false;
        }

        internal void Join(Controller ctrl)
        {

            switch (_state)
            {
                case State.LevelSelection:


                    break;
            }
        }

        protected bool TryFinishChangeState(State target, params object[] args)
        {
            switch (target)
            {
                case State.LevelSelection:

                    _state = target;

                    foreach (Level lv in _levels)
                    {
                        lv.gameObject.SetActive(true);
                    }

                    OnLevelSelect();
                    OnLevelSelected(0);

                    return true;

                case State.Round:

                    _state = target;

                    // TODO enable
                    foreach (Level level in _levels)
                    {
                        if (level == null)
                            continue;

                        if (level == CurrentLevel)
                            continue;

                        level.gameObject.SetActive(false);
                    }

                    CurrentLevel.TargetPosition = Vector3.zero;
                    CurrentLevel.transform.position = Vector3.zero;

                    _round =
                        new Round(
                            _countDown,
                            _roundTime,
                            _countDownTime);

                    HUD.OnRound(_round);

                    CurrentLevel.OnScoreValueAdded += OnScoreValueAdded;

                    foreach (var ctrl in Lobby.Controllers)
                    {
                        if (ctrl == null)
                            continue;

                        //ctrl.OnScoreChangedHandler += HUD.OnScoreChanged;
                        //ctrl.OnMultiplierChangedHandler += HUD.OnMultiplierChanged;
                    }

                    _round.OnRoundBeginHandler += CurrentLevel.OnBeginRound;

                    _round.BeginCountdown();

                    _round.OnRoundEndHandler += OnRoundEnd;

                    _round.OnRoundEndHandler += HUD.OnRoundEnd;

                    return true;

                case State.Score:

                    _state = target;

                    return true;

                case State.WaitingNextRound:
                    Lobby.Characters.Clear();
                    Lobby.Characters.AddRange(CurrentLevel.Characters);

                    foreach (Controller ctrl in Lobby.Controllers)
                    {
                        if (ctrl == null)
                        {
                            continue;
                        }

                        ctrl.Character = null;
                    }

                    _state = target;

                    OnWaiting();
                    return true;


                default:
                    return false;
            }

        }

        private bool _wasMovingVertical = false;

        // TODO: Simulate LeftStick continuous axis with WASD
        public void FSMHandleAxesLeft(Controller controller, Vector2 axis)
        {
            bool isMovingHorizontal = Mathf.Abs(axis.x) > 0.5f;
            bool isMovingVertical = Mathf.Abs(axis.y) > 0.5f;

            Vector3 stepHorizontal = new Vector3(Mathf.Sign(axis.x), 0, 0);
            Vector3 stepVertical = new Vector3(0, 0, Mathf.Sign(axis.y));
            Vector3 step = Vector3.zero;

            if (isMovingVertical && isMovingHorizontal)
            {
                //moving in both directions, prioritize later
                if (_wasMovingVertical)
                {
                    step = stepHorizontal;
                }
                else
                {
                    step = stepVertical;
                }
            }
            else if (isMovingHorizontal)
            {
                step = stepHorizontal;
                _wasMovingVertical = false;


            }
            else if (isMovingVertical)
            {
                step = stepVertical;
                _wasMovingVertical = true;
            }


            switch (_state)
            {
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

                case State.Round:
                    controller.Character?.TryMove(axis);
                    break;

                case State.Score:
                    break;
            }
        }

        public void FSMHandleAction0(Controller controller)
        {
            switch (_state)
            {
                case State.LevelSelection:
                    break;

                case State.WaitingNextRound:

                    if (controller.Character != null)
                    {
                        Lobby.Characters.Add(controller.Character);
                        HUD.Leave(controller);
                        controller.Character = null;
                    }
                    else
                    {
                        foreach (Controller ctrl in Lobby.Controllers)
                        {
                            if (ctrl == null)
                            {
                                continue;
                            }

                            ctrl.Character = null;
                        }

                        TryChangeState(State.LevelSelection);
                    }

                    break;

                case State.Round:
                    controller.Character?.TryAction0();
                    break;

                case State.Score:
                    break;
            }
        }


        public void FSMHandleAction1(Controller controller)
        {
            switch (_state)
            {
                case State.Podium:

                    break;

                case State.LevelSelection:
                    TryChangeState(State.WaitingNextRound);
                    break;

                case State.WaitingNextRound:

                    if (controller.Character == null)
                    {
                        if (Lobby.Characters.Count != 0)
                        {
                            controller.Character = Lobby.Characters[0];
                            Lobby.Characters.RemoveAt(0);
                            HUD.Join(controller);
                            // TODO update character color
                        }

                    }
                    else
                    {
                        if (Lobby.Characters.Count == 0)
                        {
                            TryChangeState(State.Round, _roundTime);
                        }

                    }

                    break;



                case State.Round:
                    controller.Character?.TryAction1();
                    break;

                case State.Score:
                    break;
            }

        }


        #endregion

    }
}
