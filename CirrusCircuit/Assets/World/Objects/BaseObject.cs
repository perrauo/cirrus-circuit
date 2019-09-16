//using Cirrus.DH.Actions;
//using Cirrus.DH.Objects.Characters;
//using Cirrus.DH.Objects.Characters.Controls;
using System.Collections.Generic;
using UnityEngine;
using Cirrus.Tags;
using Cirrus.Extensions;
using System;
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

        public virtual ObjectId Id { get { return ObjectId.Default; } }

        [SerializeField]
        protected Game _game;

        [SerializeField]
        protected World.Level _level;

        [SerializeField]
        protected Visual _visual;

        //[SerializeField]
        //public Collider _collider;

        [SerializeField]
        public GameObject _object;

        public GameObject Object
        {
            get
            {
                return _object;
            }
        }        

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
        public Controls.PlayerNumber PlayerNumber;

        [SerializeField]
        protected Color _color;

        public virtual Color Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;

                if(_visual != null)
                    _visual.Color = _color;
            }
        }

        public string Name
        {
            get
            {
                return transform.name;
            }
        }

        public virtual void OnValidate()
        {
            if (_level == null)
                _level = GetComponentInParent<World.Level>();

            if (_game == null)
                _game = FindObjectOfType<Game>();

            Color = _game.Lobby.GetColor(PlayerNumber);
            
        }

        // TODO: will not be called on disabled level
        protected virtual void Awake()
        {
            _direction = Object.transform.forward.ToVector3Int();
            _targetPosition = Object.transform.position;
            _targetScale = 1f;

            FSMAwake();
        }

        public virtual void Start()
        {
            (transform.position, _gridPosition) = _level.RegisterObject(this);

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

        public bool TryChangeState(FSM.State state, params object[] args)
        {    
            return TryChangeState(state, args);
        }

        public virtual bool TryMove(Vector3Int step, BaseObject incoming = null)
        {
            return TryChangeState(State.Moving, step, incoming);
        }

        public virtual bool TryEnter(Vector3Int step, ref Vector3 offset, BaseObject incoming = null)
        {
            if (_user != null)
            {
                if (_user.TryMove(step, incoming))
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;

        }

        public virtual bool TryFall(BaseObject incoming = null)
        {
            return TryChangeState(State.Falling, Vector3Int.down);
        }

        public virtual void Accept(BaseObject incoming)
        {
            //incoming.TryChangeState
        }

        #endregion

        #region FSM

        [System.Serializable]
        public enum State
        {
            Disabled,
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
            TryChangeState(State.Disabled);
        }

        public virtual void FSMFixedUpdate()
        {
            switch (_state)
            {
                case State.Disabled:
                    break;

                case State.Entering:
                case State.Falling:
                case State.Idle:
                case State.RampIdle:
                case State.Moving:
                case State.RampMoving:

                    Object.transform.position = Vector3.Lerp(Object.transform.position, _targetPosition, _stepSpeed);

                    float scale = Mathf.Lerp(Object.transform.localScale.x, _targetScale, _scaleSpeed);
                    Object.transform.localScale = new Vector3(scale, scale, scale);

                    break;
            }
        }

        public virtual void FSMUpdate()
        {
            switch (_state)
            {
                case State.Disabled:
                    return;

                case State.Entering:

                case State.Idle:
                case State.RampIdle:
                    return;

                case State.Falling:
                case State.Moving:
                case State.RampMoving:

                    if (Utils.Vectors.CloseEnough(Object.transform.position, _targetPosition))
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
            
        }

        public virtual void OnRoundBegin()
        {

        }

        public virtual void OnRoundEnd()
        {

        }

        public virtual bool TryChangeState(State transition, params object[] args)
        {
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
                        case State.Idle:
                            destination = transition;
                            return true;
                    }
                    break;


                case State.Entering:

                    switch (transition)
                    {
                        case State.Disabled:
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
            Vector3Int step;
            BaseObject incoming  = null;
            BaseObject destination;
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

                case State.Entering:
                    _targetScale = 0;

                    _state = target;
                    result = true;
                    break;

                case State.Falling:

                    step = (Vector3Int)args[0];

                    if (_level.TryMove(this, step, ref offset, out destination))
                    {
                        _destination = destination;
                        _gridPosition += step;
                        _targetPosition = _level.GridToWorld(_gridPosition);

                        _state = target;
                        result = true;
                    }

                    _state = State.Idle;
                    result = false;
                    break;

                case State.Idle:
                    //_collider.enabled = true;

                    _state = target;
                    result = true;
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

                    if (_level.TryMove(this, step + stepOffset, ref offset, out destination))
                    {
                        _destination = destination;
                        _gridPosition += step + stepOffset;
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

                    if (_level.TryMove(this, step, ref offset, out destination))
                    {
                        _destination = destination;
                        _gridPosition += step;
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
