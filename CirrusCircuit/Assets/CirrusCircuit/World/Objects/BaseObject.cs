//using Cirrus.DH.Actions;
//using Cirrus.DH.Objects.Characters;
//using Cirrus.DH.Objects.Characters.Controls;
using System.Collections.Generic;
using UnityEngine;
using Cirrus.Resources;
using Cirrus.Utils;
using System;
using Cirrus.Circuit.Controls;
using Cirrus.Circuit.Networking;
//using Cirrus.DH.Conditions;
//using Cirrus.DH.Objects.Actions;

namespace Cirrus.Circuit.World.Objects
{
    //public delegate void OnMoved();

    public abstract partial class BaseObject : MonoBehaviour
    {
        #region Object

        public enum ObjectId
        {
            Default,
            Character,
            Gem,
            Door,
        }

        public virtual ObjectId Id => ObjectId.Default;

        [SerializeField]
        protected Visual _visual;

        [SerializeField]
        private Color[] _fallbackColors;

        [SerializeField]
        public Transform _transform;
        public Transform Transform => _transform;        

        [SerializeField]
        public int _stepSize = 1;

        [SerializeField]
        public float _stepSpeed = 0.2f;

        [SerializeField]
        public float _fallSpeed = 0.6f;

        [SerializeField]
        public float _fallDistance = 100f;

        [SerializeField]
        public float _scaleSpeed = 0.6f;

        public BaseObject _destination = null;

        public BaseObject _user = null;

        public Vector3Int _direction;

        public Vector3 _targetPosition;

        public Vector3Int _gridPosition;

        public float _targetScale = 1;

        [SerializeField]
        public int ColorId;

        [SerializeField]
        protected UnityEngine.Color _color;

        public virtual UnityEngine.Color Color
        {
            get => _color;

            set
            {
                _color = value;

                if(_visual != null)
                    _visual.Color = _color;
            }
        }

        [SerializeField]
        protected UnityEngine.Color _nextColor;

        private Timer _nextColorTimer;

        [SerializeField]
        private float _nextColorTime = 2;

        [SerializeField]
        protected int _nextColorIndex = 0;

        [SerializeField]
        protected float _nextColorSpeed = 0.05f;


        public string Name => transform.name;

        [SerializeField]
        public Level _level = null;

        public virtual void OnValidate()
        {
            if (_level == null) _level = GetComponentInParent<Level>();

            if (PlayerManager.Instance != null)
            {
                Color = PlayerManager.Instance.GetColor(ColorId);
                _nextColor = Color;
            }
        }

        // TODO: will not be called on disabled level
        protected virtual void Awake()
        {
            if (ColorId < 4)
            {
                _nextColorIndex = ColorId;
                _nextColorTimer = new Timer(_nextColorTime, start: false, repeat: true);
                _nextColorTimer.OnTimeLimitHandler += OnNextColorTimeOut;
            }

            _direction = Transform.transform.forward.ToVector3Int();
            _targetPosition = Transform.transform.position;
            _targetScale = 1f;

            FSMAwake();
        }

        private bool _isRegistered = false;

        public void Register(Level level)
        {
            if (_isRegistered)
                return;

            _level = level;

            if (_level != null)
                (transform.position, _gridPosition) = _level.RegisterObject(this);

            _isRegistered = true;
        }

        public virtual void Start()
        {
            Register(_level);

            FSMStart();
        }

        public virtual void OnEnable()
        {

        }


        public virtual void FixedUpdate()
        {
            FSMFixedUpdate();
        }

        public virtual void Update()
        {
            FSMUpdate();
        }

        //public void UpdateColor()
        //{
        //    foreach (Controls.Controller ctrl in Game.Instance._controllers)
        //    {
        //        if (ctrl.Number == Number)
        //        {
        //            Color = ctrl.Color;
        //            _nextColor = Color;
        //            break;
        //        }
        //    }
        //}

        public virtual void Interact(BaseObject source)
        {
            //if (Number >= 4)
            //{
            //    Number = source.Number;
            //    Color = source.Color;
            //}
        }

        public virtual bool TryMove(Vector3Int step, BaseObject incoming = null)
        {
            return TryChangeState(State.Moving, step, incoming);
        }

        public virtual bool TryEnter(Vector3Int step, ref Vector3 offset, BaseObject incoming = null)
        {
            if (_user != null)
            {
                if (_user.TryMove(step, incoming)) return true;
            }

            return true;            

        }

        public virtual bool TryFall(BaseObject incoming = null)
        {
            return TryChangeState(State.Falling, Vector3Int.down);
        }

        public virtual void Accept(BaseObject incoming)
        {
            //incoming.TryChangeState
        }

        public void OnNextColorTimeOut()
        {
            if (_state != State.LevelSelect)
                return;

            if (GameSession.Instance.LocalPlayers.Count == 0)
                return;

            _nextColorIndex = _nextColorIndex + 1;
            _nextColorIndex = MathUtils.Wrap(_nextColorIndex, 0, GameSession.Instance._players.Count);
            _nextColor = PlayerManager.Instance.GetColor(_nextColorIndex);

        }


        #endregion

        #region FSM

        [System.Serializable]
        public enum State
        {
            Disabled,
            LevelSelect,
            Entering,
            Falling,
            Idle,
            RampIdle,
            Moving,
            RampMoving
        }

        [SerializeField]
        protected State _state = State.Idle;

        public virtual void FSMAwake()
        {
            TryChangeState(State.Disabled);
        }

        public virtual void FSMStart()
        {
            //TryChangeState(State.Disabled);
        }

        public virtual void FSMFixedUpdate()
        {
            switch (_state)
            {
                case State.Disabled:
                    break;

                case State.LevelSelect:

                    if (ColorId < PlayerManager.PlayerMax)
                    {
                        Color = Color.Lerp(_color, _nextColor, _nextColorSpeed);
                    }

                    break;

                case State.Entering:
                case State.Falling:
                case State.Idle:
                case State.RampIdle:
                case State.Moving:
                case State.RampMoving:

                    Transform.transform.position = Vector3.Lerp(Transform.transform.position, _targetPosition, _stepSpeed);
                    float scale = Mathf.Lerp(Transform.transform.localScale.x, _targetScale, _scaleSpeed);
                    Transform.transform.localScale = new Vector3(scale, scale, scale);

                    break;
            }
        }

        public virtual void FSMUpdate()
        {
            switch (_state)
            {
                case State.Disabled:
                    return;
                case State.LevelSelect:
                    return;

                case State.Entering:

                case State.Idle:
                case State.RampIdle:
                    return;

                case State.Falling:
                case State.Moving:
                case State.RampMoving:

                    if (VectorUtils.IsCloseEnough(Transform.transform.position, _targetPosition))
                    {
                        if (_destination == null)
                        {
                            BaseObject obj;

                            if (_level.TryGet(_gridPosition + Vector3Int.down, out obj))
                            {
                                TryChangeState(State.Idle);
                            }
                            else
                            {
                                TryFall();// State.Falling, Vector3Int.down);                                
                            }
                        }
                        else
                        {
                            _destination.Accept(this);
                        }
                    }

                    break;
            }
        }

        public virtual void OnRound()
        {
            TryChangeState(State.Idle);
        }

        public virtual void OnRoundBegin()
        {

        }

        public virtual void OnRoundEnd()
        {

        }

        public virtual bool TryChangeState(State transition, params object[] args)
        {
            //Debug.Log(transition);

            if (TryTransition(transition, out State destination))
            {
                return TryFinishChangeState(destination, args);
            }

            return false;
        }

        protected virtual bool TryTransition(State transition, out State destination, params object[] args)
        {
            switch (_state)
            {
                case State.Disabled:

                    switch (transition)
                    {
                        case State.Disabled:
                        case State.LevelSelect:
                        case State.Entering:
                        case State.Falling:
                        case State.Idle:
                        case State.RampIdle:
                        case State.Moving:
                            destination = transition;
                            return true;
                    }
                    break;

                case State.LevelSelect:

                    switch (transition)
                    {
                        case State.Disabled:
                        case State.LevelSelect:
                        case State.Entering:
                        case State.Falling:
                        case State.Idle:
                        case State.RampIdle:
                        case State.Moving:
                            destination = transition;
                            return true;
                    }
                    break;


                case State.Entering:

                    switch (transition)
                    {
                        case State.Disabled:
                        case State.LevelSelect:
                        case State.Entering:
                        case State.Falling:
                        case State.Idle:
                        case State.RampIdle:
                        case State.Moving:
                            destination = transition;
                            return true;
                    }
                    break;

                case State.Falling:
                    switch (transition)
                    {
                        case State.Disabled:
                        case State.LevelSelect:
                        case State.Entering:
                        case State.Falling:
                        case State.Idle:
                        case State.RampIdle:
                        case State.Moving:
                            destination = transition;
                            return true;
                    }
                    break;

                case State.Idle:
                    switch (transition)
                    {
                        case State.Disabled:
                        case State.LevelSelect:
                        case State.Entering:
                        case State.Falling:
                        case State.Idle:
                        case State.RampIdle:
                        case State.Moving:
                            destination = transition;
                            return true;
                    }
                    break;

                case State.RampIdle:
                    switch (transition)
                    {
                        case State.Disabled:
                        case State.LevelSelect:
                        case State.Moving:
                            //case State.Moving:
                            destination = State.RampMoving;
                            return true;

                        default:
                            destination = State.Idle;
                            return false;
                    }

                case State.Moving:
                    switch (transition)
                    {
                        case State.Disabled:
                        case State.LevelSelect:
                        case State.Entering:
                        case State.Falling:
                        case State.Idle:
                        case State.RampIdle:
                            //case State.Moving:
                            destination = transition;
                            return true;
                    }
                    break;

                case State.RampMoving:
                    switch (transition)
                    {
                        case State.Disabled:
                        case State.LevelSelect:
                        case State.Entering:
                        case State.Falling:
                        case State.Idle:
                        case State.RampIdle:
                            //case State.Moving:
                            destination = transition;
                            return true;
                    }
                    break;
            }

            destination = State.Idle;
            return false;
        }

        protected virtual bool TryFinishChangeState(State target, params object[] args)
        {
            Vector3Int previousGridPosition = _gridPosition;
            Vector3Int newGridPosition = Vector3Int.zero;
            Vector3Int step;
            BaseObject incoming  = null;
            BaseObject destination;
            BaseObject pushed;
            BaseObject above;
            Vector3 offset = Vector3.zero;
            Vector3Int stepOffset = Vector3Int.zero;
            bool result = false;

            switch (target)
            {
                case State.Disabled:
                    result = true;
                    _state = target;
                    break;

                case State.LevelSelect:

                    if (ColorId < PlayerManager.PlayerMax)
                    {
                        OnNextColorTimeOut();
                        _nextColorTimer.Start();
                    }

                    result = true;
                    _state = target;
                    break;

                case State.Entering:
                    _targetScale = 0;

                    _state = target;
                    result = true;
                    break;

                case State.Falling:

                    step = (Vector3Int)args[0];

                    if (_level.TryMove(
                        this,
                        step,
                        ref offset,
                        out newGridPosition,
                        out destination,
                        out pushed))
                    {
                        _destination = destination;
                        _gridPosition = newGridPosition;
                        _targetPosition = _level.GridToWorld(_gridPosition);
                        _state = target;
                        result = true;
                    }
                    else if (_level.TryFallThrough(this,
                        step,
                        ref offset,
                        out newGridPosition,
                        out destination))
                    {
                        _destination = destination;
                        _gridPosition = newGridPosition;// _level.GridToWorld(newGridPosition);
                        _targetPosition = _level.GridToWorld(_gridPosition);
                        Transform.transform.position = _targetPosition; 

                        _state = target;
                        result = true;
                    }
                    else
                    {
                        _state = State.Idle;
                        result = false;
                    }

                    break;

                case State.Idle:
                    //_collider.enabled = true;

                    // TODO: Redundant
                    if (_level.TryGet(_gridPosition + Vector3Int.down, out destination))
                    {
                        _state = target;
                        result = true;

                    }
                    else
                    {
                        TryFall();// State.Falling, Vector3Int.down);                                
                    }

                    break;

                case State.RampIdle:
                    //_collider.enabled = false;

                    _state = target;
                    result = true;
                    break;

                case State.RampMoving:

                    step = (Vector3Int)args[0];
                    incoming = (BaseObject)args[1];

                    // Determine which direction to cast the ray

                    // Same direction (Look up)
                    if (step == _destination._direction)
                    {
                        stepOffset += Vector3Int.up;
                        //offset += Vector3.up * (Level.GridSize / 2);
                    }
                    // Opposing direction (look down)
                    else if (step == _destination._direction * -1)
                    { 
                        stepOffset += Vector3Int.up;
                        //offset -= Vector3.up * (Level.GridSize / 2);
                    }

                    if (_level.TryMove(
                        this, 
                        step + stepOffset, 
                        ref offset, 
                        out newGridPosition, 
                        out pushed, 
                        out destination))
                    {
                        if (pushed) pushed.Interact(this);
                        _destination = destination;
                        _gridPosition = newGridPosition;
                        _targetPosition = _level.GridToWorld(_gridPosition);
                        _targetPosition += offset;
                        _direction = step;
                        _state = target;
                        result = true;
                    }

                    break;

                case State.Moving:
                    step = (Vector3Int)args[0];
                    incoming = (BaseObject)args[1];

                    if (_level.TryMove(this, step, ref offset, out newGridPosition, out pushed, out destination))
                    {
                        //destination.
                        if(pushed) pushed.Interact(this);                        

                        _destination = destination;
                        _gridPosition = newGridPosition;
                        _targetPosition = _level.GridToWorld(_gridPosition);
                        _targetPosition += offset;
                        _direction = step;

                        _state = target;
                        result = true;                        
                    }

                    break;

                default:
                    result = false;
                    break;
            }

            if (result && incoming == null)
            {
                // Determine if object above to make it fall
                switch (target)
                {
                    case State.Moving:
                    case State.RampMoving:
                    case State.Falling:

                        if (_level.TryGet(previousGridPosition + Vector3Int.up, out above))
                        {
                            above.TryFall();// (State.Falling, Vector3Int.down);
                        }

                        _state = target;
                        break;
                }
            }


            return result;
        }

        #endregion
    }
}
