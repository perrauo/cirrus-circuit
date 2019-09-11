//using Cirrus.DH.Actions;
//using Cirrus.DH.Objects.Characters;
//using Cirrus.DH.Objects.Characters.Controls;
using System.Collections.Generic;
using UnityEngine;
using Cirrus.Tags;
//using Cirrus.DH.Conditions;
//using Cirrus.DH.Objects.Actions;

namespace Cirrus.Circuit.Objects
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
        protected Level _level;

        [SerializeField]
        protected Visual _visual;

        [SerializeField]
        public Collider _collider;

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
        public float _stepDistance = 2f;

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

        public Vector3 _step;

        public Vector3 _targetPosition;

        private Vector3Int _gridPosition;


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
                _level = GetComponentInParent<Level>();
        }

        protected virtual void Awake()
        {
            (transform.position, _gridPosition) = _level.RegisterObject(this);
            _targetPosition = Object.transform.position;
            _targetScale = 1f;

            FSMAwake();
        }

        public virtual void Start()
        {
            FSMStart();
        }

        public virtual void FixedUpdate()
        {
            FSMFixedUpdate();
        }

        public virtual void Update()
        {
            FSMUpdate();
        }

        //public virtual void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.red;

        //    Gizmos.DrawSphere(_targetPosition, 0.1f);
        //}

        public bool TryChangeState(FSM.State state, params object[] args)
        {    
            return TryChangeState(state, args);
        }

        public virtual bool TryMove(Vector3 step, BaseObject incoming = null)
        {
            return TryChangeState(State.Moving, step, incoming);
        }

        public virtual bool TryEnter(Vector3 step, BaseObject incoming = null)
        {
            return false;
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
        private State _state = State.Idle;

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
                        // If the destination can coexist with incoming object once arrived we return true
                        if (_destination == null)
                        {
                            RaycastHit hit;

                            if (Physics.Raycast(
                                _targetPosition,
                                Vector3.down,
                                out hit,
                                Level.BlockSize / 2))
                            {
                                TryChangeState(State.Idle);
                            }
                            else
                            {
                                // Raycast down to get distance
                                if (
                                    Physics.Raycast(
                                    _targetPosition,
                                    Vector3.down,
                                    out hit,
                                    10f))
                                {
                                    //Debug.Log(_object._targetPosition);
                                    //Debug.Log(hit.point);
                                    TryChangeState(State.Falling, hit.distance);
                                }
                                else
                                {
                                    // TODO
                                    // Fall to infinity and destroy
                                }
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
            Vector3 step;
            BaseObject incoming;
            RaycastHit hit;

            switch (target)
            {
                case State.Entering:
                    _targetScale = 0;

                    _state = target;
                    return true;

                case State.Falling:
                    float distance = (float)args[0];
                    _targetPosition += Vector3.down * distance;
                    _targetPosition += Vector3.up * Level.BlockSize / 2;

                    _state = target;
                    return true;

                case State.Idle:
                    _collider.enabled = true;

                    _state = target;
                    return true;

                case State.RampIdle:
                    _collider.enabled = false;

                    _state = target;
                    return true;

                case State.RampMoving:

                    step = (Vector3)args[0];
                    incoming = (BaseObject)args[1];

                    Vector3 offset = Vector3.zero; ;
                    // Determine which direction to cast the ray

                    Ray ray;
                    // Same direction (Look up)
                    if (Utils.Vectors.CloseEnough(step.normalized, Object.transform.forward))
                    {
                        ray = new Ray(_targetPosition + Vector3.up * Level.BlockSize, step);
                        offset += Vector3.up * Level.BlockSize / 2;
                    }
                    // Opposing direction (look down)
                    else if (Utils.Vectors.CloseEnough(step.normalized, -Object.transform.forward))
                    {
                        ray = new Ray(_targetPosition + Vector3.down * Level.BlockSize, step);
                        offset -= Vector3.up * Level.BlockSize / 2;
                    }
                    // Perp direction (Look ahead)
                    else
                    {
                        ray = new Ray(_targetPosition, step);
                    }

                    if (Physics.Raycast(ray, out hit, Level.BlockSize))
                    {
                        _collider.enabled = true;
                        var destination = hit.collider.GetComponentInParent<BaseObject>();

                        if (destination != null)
                        {
                            if (destination.TryMove(step, this))
                            {
                                destination._user = null;
                                _destination = null; // We pushed it the destination was moved
                                _targetPosition += step;
                                _targetPosition += offset;

                                _state = target;
                                return true;
                            }
                            else if (destination.TryEnter(step, this))
                            {
                                if (destination._user)
                                {
                                    if (destination._user.TryMove(step, this))
                                    {
                                        destination._user = this; // We pushed it the destination was moved
                                        _destination = destination;
                                        _targetPosition += step;
                                        _targetPosition += offset;

                                        _state = target;
                                        return true;
                                    }
                                }
                                else
                                {
                                    destination._user = this;
                                    _destination = destination;
                                    _targetPosition += step;
                                    _targetPosition += offset;

                                    _state = target;
                                    return true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_destination)
                            _destination._user = null;

                        _destination = null;
                        _targetPosition += step;
                        _targetPosition += offset;

                        _state = target;
                        return true;
                    }

                    break;

                case State.Moving:

                    step = (Vector3)args[0];
                    incoming = (BaseObject)args[1];

                    // Raycast front
                    if (Physics.Raycast(
                        _targetPosition,
                        step,
                        out hit,
                        Level.BlockSize))
                    {
                        _collider.enabled = true;
                        var destination = hit.collider.GetComponentInParent<BaseObject>();

                        if (destination != null)
                        {
                            if (destination.TryMove(step, this))
                            {
                                destination._user = null;
                                _destination = null; // We pushed it the destination was moved                                
                                _targetPosition += step;

                                _state = target;
                                return true;
                            }
                            else if (destination.TryEnter(step, this))
                            {
                                if (destination._user)
                                {
                                    if (destination._user.TryMove(step, this))
                                    {
                                        destination._user = this; // We pushed it the destination was moved
                                        _destination = destination;
                                        _targetPosition += step;

                                        _state = target;
                                        return true;
                                    }
                                }
                                else
                                {
                                    destination._user = this;
                                    _destination = destination;
                                    _targetPosition += step;

                                    _state = target;
                                    return true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_destination)
                            _destination._user = null;

                        _destination = null;
                        _targetPosition += step;

                        _state = target;
                        return true;
                    }


                    break;

                default:
                    return false;
            }

            return false;
        }

        #endregion
    }
}
