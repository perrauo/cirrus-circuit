using System;
using Cirrus.Circuit.Controls;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using Cirrus.Extensions;
using Cirrus.Circuit.World.Objects.Characters;

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

    public delegate void OnWindowResized();// bool enabled = true);

    public delegate void OnCharacterSelect(bool enabled=true);

    public delegate void OnLevelSelect(bool enabled=true);

    public delegate void OnMenu(bool enabled = true);

    public delegate void OnPodium();

    public delegate void OnFinalPodium();

    public delegate void OnNewRound(Round round);

    public delegate void OnLevelSelected(World.Level level, int step);

    public delegate void OnControllerJoin(Controller controller);

    public class Game : MonoBehaviour
    {
        #region Game

        [SerializeField]
        private bool _randomizeSeed = false;

        public OnWindowResized OnScreenResizedHandler;

        public World.Objects.Door.OnScoreValueAdded OnScoreValueAddedHandler;

        public OnPodium OnPodiumHandler;

        public OnFinalPodium OnFinalPodiumHandler;

        public OnNewRound OnNewRoundHandler;

        public OnLevelSelected OnLevelSelectedHandler;

        //public delegate void OnLevelSelect();
        public OnMenu OnMenuHandler;

        public OnLevelSelect OnLevelSelectHandler;

        public OnCharacterSelect OnCharacterSelectHandler;

        public OnControllerJoin OnControllerJoinHandler;

        //public OnLevelSelect OnLevelSelectHandler;

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
        private Transitions.Transition _transitionEffect;

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

        public UI.CharacterSelect _characterSelect;

        public UI.StartMenu _startMenu;


        [SerializeField]
        public World.Level[] _levels;

        public World.Level _selectedLevel;

        public World.Level _currentLevel;


        public int _currentLevelIndex = 0;

        public float _distanceLevelSelect = 35;

        [SerializeField]
        public CameraWrapper _camera;

        [SerializeField]
        private Podium _podium;

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
        private int _roundAmount = 3;

        public int _roundIndex;

        /// Controllers in player in game
        public List<Controller> _controllers;

        [SerializeField]
        public float _podiumTransitionSpeed = 0.2f;

        [SerializeField]
        private float _intermissionTime = 2f;
        
        [SerializeField]
        private float _transitionTime = 5f;

        private Timer _transitionTimer = null;

        private State _transition;

        private float _transitionDistance = 48f;


        Vector3 initialVectorBottomLeft;
        Vector3 initialVectorTopRight;

        Vector3 UpdatedVectorBottomLeft;
        Vector3 UpdatedVectorTopRight;




        public void OnValidate()
        {
            if (_camera == null)
                _camera = FindObjectOfType<CameraWrapper>();

            _levels = GetComponentsInChildren<World.Level>(true);
            _selectedLevel = _levels.Length == 0 ? null : _levels[0];

            if (_transitionEffect == null)
                _transitionEffect = FindObjectOfType<Transitions.Transition>();

            if (_characterSelect == null)
                _characterSelect = FindObjectOfType<UI.CharacterSelect>();

            if (_startMenu == null)
                _startMenu = FindObjectOfType<UI.StartMenu>();
        }


        void Awake()
        {
            if(_randomizeSeed)
                UnityEngine.Random.InitState(Environment.TickCount);

            _transitionTimer = new Timer(_transitionTime, start: false, repeat: false);

            _transitionTimer.OnTimeLimitHandler += OnTransitionTimeOut;

            _controllers = new List<Controller>();

            Layers = new Layers();
            DontDestroyOnLoad(this.gameObject);

            _podium.OnPodiumFinishedHandler += OnPodiumFinished;

            _characterSelect.OnCharacterSelectReadyHandler += OnCharacterSelectReady;

            TryChangeState(State.Menu);
        }

        void Start()
        {
            initialVectorBottomLeft = _camera.Camera.ScreenToWorldPoint(new Vector3(0, 0, 30));
            initialVectorTopRight = _camera.Camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 30)); // I used 30 as my camera z is -30
        }

        void Update()
        {
            UpdatedVectorBottomLeft = _camera.Camera.ScreenToWorldPoint(new Vector3(0, 0, 30));
            UpdatedVectorTopRight = _camera.Camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 30));

            if ((initialVectorBottomLeft != UpdatedVectorBottomLeft) || (initialVectorTopRight != UpdatedVectorTopRight))
            {
                OnScreenResizedHandler?.Invoke();
            }

            FSMUpdate();
        }

        public void FixedUpdate()
        {
            FSMFixedUpdate();
        }

        public void OnStartClicked()
        {
            TryChangeState(State.Transition, State.CharacterSelection);
        }

        public void OnCharacterSelectReady(int playerCount)
        {
            TryChangeState(State.Transition, State.LevelSelection);
        }

        private void OnScoreValueAdded(int player, float value)
        {
            Lobby.Controllers[player].Score += value;
            HUD.OnScoreChanged(player, Lobby.Controllers[player].Score);
        }

        public void OnLevelSelected(int step)
        {
            for (int i = 0; i < _levels.Length; i++)
            {
                if (_levels[i] == null)
                    continue;

                _levels[i].TargetPosition = Vector3.zero + Vector3.right * (i - _currentLevelIndex) * _distanceLevelSelect;
            }

            _selectedLevel = _levels[_currentLevelIndex];

            _targetSizeCamera = _selectedLevel.CameraSize;

            OnLevelSelectedHandler?.Invoke(_selectedLevel, step);            
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
            OnLevelSelectHandler?.Invoke();
        }

        public void OnWaiting()
        {
            HUD.OnWaiting();
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
            if (_state == State.FinalPodium)
            {
                TryChangeState(State.Transition, State.LevelSelection);
            }
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
                    _podium.gameObject.SetActive(false);
                    break;

                //case State.Round:
                //    _podium.gameObject.SetActive(false);
                //    break;
            }

            TryChangeState(_transition);
        }

        public bool TryJoin(Controller controller)
        {
            if (_controllers.Count >= _selectedLevel.CharacterCount)
                return false;

            _controllers.Add(controller);

            return false;
        }

        public bool TryLeave(Controller controller)
        {
            if (_controllers.Count >= _selectedLevel.CharacterCount)
                return false;

            _controllers.Remove(controller);

            return false;
        }

        public void Join(Controller ctrl)
        {
            switch (_state)
            {
                case State.LevelSelection:
                    break;
            }
        }

        //IEnumerator OnRound()
        //{
        //    yield return new WaitForEndOfFrame();

        //    for (int i = 0; i < _currentLevel.CharacterCount; i++)
        //    {
        //        _controllers[i]._character = _currentLevel._characters[i];
        //    }
        //}

        #endregion


        #region FSM

        [System.Serializable]
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
            Transition
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
                    _camera.Camera.orthographicSize =
                        Mathf.Lerp(
                            _camera.Camera.orthographicSize,
                            _targetSizeCamera,
                            _cameraSizeSpeed);

                    break;
            }
        }

        public void FSMUpdate()
        {
            switch (_state)
            {

                case State.Round:

                    foreach (Controller ctrl in _controllers)
                    {
                        ctrl._character.TryMove(ctrl.AxisLeft);
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


        protected bool TryTransition(State transition, out State destination, params object[] args)
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


        protected bool TryFinishChangeState(State target, params object[] args)
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
                    _podium.Clear();

                    foreach (var c in _controllers)
                    {
                        if (c == null)
                            continue;

                        _podium.Add(c, c._characterResource);
                    }

                    _podium.gameObject.SetActive(false);

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

                    _transition = (State) args[0];
                    _transitionTimer.Start();

                    _transitionEffect.Perform();
                                      
                    _state = target;
                    return true;

                
                case State.Podium:
                    _podium.gameObject.SetActive(true);
                    OnPodiumHandler?.Invoke();
                    _state = target;
                    return true;

                case State.FinalPodium:                    
                    _podium.gameObject.SetActive(true);
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

                    _currentLevel.OnScoreValueAddedHandler += OnScoreValueAdded;

                    List<Placeholder> placeholders = new List<Placeholder>();
                    placeholders.AddRange(_currentLevel._characterPlaceholders);

                    int i = 0;
                    while (!placeholders.IsEmpty())
                    {
                        Placeholder placeholder = placeholders.RemoveRandom();
 
                        _controllers[i]._character = _controllers[i]
                            ._characterResource.Create(_currentLevel.GridToWorld(placeholder._gridPosition), _currentLevel.transform);

                        _controllers[i]._character.Number = _controllers[i].Number;

                        _controllers[i]._character.UpdateColor();

                        _controllers[i]._character._level = _currentLevel;

                        _controllers[i]._character.UpdateColor();

                        _controllers[i]._character.TryChangeState(Character.State.Disabled);

                        _controllers[i].Score = 0;

                        _controllers[i]._assignedNumber = placeholder.Number;

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

                    OnNewRoundHandler?.Invoke(_round);

                    //_round.OnRoundBeginHandler += _currentLevel.OnBeginRound;

                    _round.OnRoundEndHandler += OnRoundEnd;

                    _round.BeginIntermission();

                    _state = target;

                    return true;

                case State.Score:

                    _state = target;

                    return true;

                case State.WaitingNextRound:
                    //Lobby.Characters.Clear();
                    //Lobby.Characters.AddRange(_selectedLevel.Characters);

                    foreach (Controller ctrl in Lobby.Controllers)
                    {
                        if (ctrl == null)
                        {
                            continue;
                        }

                        //ctrl.Character = null;
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
                case State.CharacterSelection:
                    if (controller._characterSlot == null)
                        break;
                 
                    if (Mathf.Abs(step.z) > 0)
                    {
                        controller._characterSlot.Scroll(step.z > 0);
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

        public void FSMHandleAction0(Controller controller)
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
                        controller._characterSlot.HandleAction0();
                    }

                    break;

                case State.WaitingNextRound:

                    if (_controllers.Contains(controller))
                    {
                        HUD.Leave(controller);
                        _controllers.Remove(controller);
                    }
                    else
                    {
                        foreach (Controller ctrl in Lobby.Controllers)
                        {
                            if (ctrl == null)
                            {
                                continue;
                            }
                        }

                        TryChangeState(State.LevelSelection);
                    }

                    break;

                case State.Round:
                    if (controller._character)
                        controller._character?.TryAction0();

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
                    TryChangeState(State.Transition, State.Begin);
                    break;

                case State.CharacterSelection:

                    if (!_controllers.Contains(controller))
                    {
                        _controllers.Add(controller);
                        OnControllerJoinHandler?.Invoke(controller);
                    }
                    else
                    {
                        controller._characterSlot.HandleAction1(controller);
                    }

                    break;


                case State.WaitingNextRound:

                    break;


                case State.Round:
                    if(controller._character)
                    controller._character?.TryAction1();
                    break;

                case State.Score:
                    break;
            }

        }


        #endregion

    }
}
