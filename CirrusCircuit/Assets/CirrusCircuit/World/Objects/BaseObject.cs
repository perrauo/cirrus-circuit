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
//using System.Numerics;

namespace Cirrus.Circuit.World.Objects
{
    public abstract partial class BaseObject : MonoBehaviour
    {

        [Serializable]
        public enum State
        {
            Disabled,
            LevelSelect,
            Entering,
            Falling,
            FallingThrough,
            Idle,
            SlopeIdle,
            Moving,
            SlopeMoving
        }

        public enum ObjectType
        {
            None,
            Default,
            Character,
            CharacterPlaceholder,
            Gem,
            Door,
            Portal,
            Solid,
            Slope,
            Breakable
        }

        public virtual ObjectType Type => ObjectType.Default;

        [SerializeField]
        public ObjectSession _session;

        [SerializeField]
        protected Visual _visual;

        [SerializeField]
        private Color[] _fallbackColors;

        [SerializeField]
        public Transform _transform;
        public Transform Transform => _transform;

        public const int StepSize = 1;

        public const float StepSpeed = 0.4f;

        public const float FallSpeed = 0.8f;

        public const float FallDistance = 100f;

        public const float ScaleSpeed = 0.6f;

        public BaseObject _destination = null;

        public BaseObject _visitor = null;

        public Vector3Int _direction;

        public Vector3 _targetPosition;

        public Vector3 _offset;

        Vector3Int _previousGridPosition;

        public Vector3Int _gridPosition;

        public float _targetScale = 1;

        public int ColorId
        {
            set => _colorId = (Number)value;
            get => (int)_colorId;
        }

        [SerializeField]
        private Number _colorId;

        [SerializeField]
        protected Color _color;

        public virtual Color Color
        {
            get => _color;

            set
            {
                _color = value;

                if (_visual != null) _visual.Color = _color;
            }
        }

        [SerializeField]
        protected Color _nextColor;

        private Timer _nextColorTimer;

        private const float NextColorTime = 2;

        protected const float NextColorSpeed = 0.05f;

        [SerializeField]
        protected int _nextColorIndex = 0;

        public string Name => transform.name;

        [SerializeField]
        public Level _level = null;

        public LevelSession _levelSession = null;

        private bool _hasArrived = false;

        [SerializeField]
        private const float ExitScaleTime = 0.01f;

        private Timer _exitScaleTimer;

        [SerializeField]
        protected State _state = State.Disabled;


        #region Unity Engine

        public virtual void OnValidate()
        {
            if (PlayerManager.Instance != null)
            {
                Color = PlayerManager
                    .Instance
                    .GetColor(ColorId);

                _nextColor = Color;
            }
        }

        // TODO: will not be called on disabled level
        protected virtual void Awake()
        {
            if (PlayerManager.IsValidPlayerId(ColorId))
            {
                _nextColorIndex = ColorId;
                _nextColorTimer = new Timer(NextColorTime, start: false, repeat: true);
                _nextColorTimer.OnTimeLimitHandler += OnNextColorTimeOut;
            }
            else
            {
                _visual.MakeMaterialsUnique();

                Color = PlayerManager
                    .Instance
                    .GetColor(ColorId);
            }

            _exitScaleTimer = new Timer(ExitScaleTime, start: false, repeat: false);
            _exitScaleTimer.OnTimeLimitHandler += OnExitScaleTimeout;

            _direction = Transform.forward.ToVector3Int();
            _targetPosition = Transform.position;
            _targetScale = 1f;

            FSM_Awake();
        }

        // TODO remove

        public virtual void Start()
        {
            FSM_Start();
        }

        public virtual void OnEnable()
        {

        }

        public virtual void FixedUpdate()
        {
            FSM_FixedUpdate();
        }

        public virtual void Update()
        {
            FSM_Update();
        }

        #endregion

        public void Register(Level level)
        {
            _level = level;
            if (_level == null) return;

            (transform.position, _gridPosition) = _level.RegisterObject(this);
            Transform.position = transform.position;
        }

        public virtual void OnRoundEnd()
        {

        }


        // TODO remove
        public void Respond(ObjectSession.CommandResponse res)
        {
            switch (res.Id)
            {
                case ObjectSession.CommandId.LevelSession_IsFallThroughAllowed:


                    break;

                case ObjectSession.CommandId.LevelSession_IsMoveAllowed:

                    break;
            }
        }

        public virtual void Accept(BaseObject source)
        {
            //source.SetState
        }

        public virtual void Disable()
        {
            InitState(State.Disabled, null);
        }

        public virtual void Land()
        {

        }

        public virtual void WaitLevelSelect()
        {
            if (PlayerManager.IsValidPlayerId(ColorId))
            {
                OnNextColorTimeOut();
                _nextColorTimer.Start();

                InitState(State.LevelSelect, null);
            }
        }


        public void OnNextColorTimeOut()
        {
            if (_state != State.LevelSelect) return;

            _nextColorIndex = _nextColorIndex + 1;
            _nextColorIndex = MathUtils.Wrap(_nextColorIndex, 0, GameSession.Instance.PlayerCount);
            _nextColor = PlayerManager.Instance.GetColor(_nextColorIndex);
        }

        public void OnExitScaleTimeout()
        {
            _targetScale = 1;
            _targetPosition -= _offset;
        }


        #region Interact

        public virtual void Interact(BaseObject source)
        {
            if (!PlayerManager.IsValidPlayerId(ColorId))
            {
                ColorId = source.ColorId;
                Color = source.Color;
            }
        }


        public virtual void Cmd_Interact(BaseObject source)
        {
            _session.Cmd_Interact(source);
        }

        #endregion

        #region Fall

        // TODO State move argument
        public virtual bool IsFallAllowed(
            BaseObject source = null)
        {

            return _levelSession.IsMoveAllowed(this, Vector3Int.down);
        }


        public virtual void Cmd_Fall()
        {
            _session.Cmd_Fall();
        }

        public virtual void Fall()
        {
            Vector3Int step = Vector3Int.down;

            if (_levelSession.Move(
                this,
                step,
                out Vector3 offset,
                out Vector3Int gridDest,
                out BaseObject dest,
                out BaseObject moved))
            {
                _destination = dest;
                _gridPosition = gridDest;
                //Debug.Log(Name + " " + _gridPosition);
                _targetPosition = _level.GridToWorld(_gridPosition);

                InitState(State.Falling, null);

            }
            else if (_levelSession.IsFallThroughAllowed(
                this,
                step))
            {
                Cmd_FallThrough(step);

                InitState(State.Falling, null);
            }
            else
            {
                InitState(State.Idle, null);
            }
        }

        public virtual void Cmd_FallThrough(Vector3Int step)
        {
            _session.Cmd_FallThrough(step);
        }

        public virtual void FallThrough(
            Vector3Int step,
            Vector3Int fallThroughPosition)
        {
            if (LevelSession.Instance.MoveToPosition(
                this,
                fallThroughPosition,
                step,
                out Vector3 offset,
                out Vector3Int gridDest,
                out BaseObject moved,
                out BaseObject destination))
            {
                _destination = destination;
                _gridPosition = gridDest;
                _targetPosition = _level.GridToWorld(_gridPosition);
                Transform.position = _targetPosition;

                InitState(State.FallingThrough, null);
            }
        }


        #endregion

        #region Move

        public virtual bool Move(
            Vector3Int step,
            BaseObject source)
        {
            switch (_state)
            {
                case State.SlopeIdle:
                    MoveFromSlope(step, source);
                    break;

                default:
                    if (_levelSession.Move(
                        this,
                        step,
                        out _offset,
                        out Vector3Int gridDest,
                        out BaseObject moved,
                        out BaseObject destination))
                    {
                        //destination.
                        if (moved) moved.Cmd_Interact(this);

                        _destination = destination;
                        _gridPosition = gridDest;
                        _targetPosition = _level.GridToWorld(_gridPosition);
                        _targetPosition += _offset;
                        _direction = step;

                        InitState(State.Moving, source);
                        return true;
                    }
                    break;

            }

            return false;

        }

        public virtual void MoveFromSlope(
            Vector3Int step,
            BaseObject source)
        {
            Vector3Int gridOffset = Vector3Int.zero;

            // Determine which direction to cast the ray

            // Same direction (Look up)
            if (step.Copy().SetY(0) == _destination._direction)
            {
                gridOffset = Vector3Int.up;
            }
            // Opposing direction (look down)
            else if (step.Copy().SetY(0) == -_destination._direction)
            {
                gridOffset = -Vector3Int.up;
            }

            if (_levelSession.Move(
                this,
                step + gridOffset,
                out Vector3 offset,
                out Vector3Int gridDest,
                out BaseObject moved,
                out BaseObject destination))
            {
                if (moved) moved.Cmd_Interact(this);
                _destination = destination;
                _gridPosition = gridDest;
                _targetPosition = _level.GridToWorld(_gridPosition);
                _targetPosition += offset;
                _direction = step;

                InitState(State.SlopeMoving, source);
            }
        }


        public virtual void Cmd_Move(Vector3Int step)
        {
            _session.Cmd_Move(step);
        }

        // TODO State move argument
        public virtual bool IsMoveAllowed(
            Vector3Int step,
            BaseObject source = null)
        {
            return _levelSession.IsMoveAllowed(this, step);
        }

        #endregion

        #region Enter

        public virtual bool IsEnterAllowed(
            Vector3Int step,
            BaseObject source = null)
        {
            if (_visitor != null) return _visitor.IsMoveAllowed(step, source);

            else return true;
        }

        public virtual bool Enter(
            Vector3Int step,
            BaseObject source,
            out Vector3 offset,
            out Vector3Int gridDest,
            out Vector3Int stepDest,
            out BaseObject dest)
        {
            offset = Vector3.zero;
            stepDest = step;
            gridDest = source._gridPosition + step;
            dest = this;

            if (_visitor != null) _visitor.Move(step, source);

            return true;
        }

        #endregion

        #region Idle

        public virtual void Idle()
        {
            // TODO: Redundant
            if (_levelSession.Get(
                _gridPosition + Vector3Int.down,
                out BaseObject other))
            {
                State state =
                    _destination != null && _destination.Type == ObjectType.Slope ?
                    State.SlopeIdle :
                    State.Idle;

                InitState(state, null);
            }
            else Cmd_Fall();
        }

        public virtual void RampIdle()
        {
            InitState(State.SlopeIdle, null);
        }


        #endregion

        #region Exit

        public virtual void OnExited()
        {
            _exitScaleTimer.Start();
            _targetScale = 0;
        }

        #endregion

        #region FSM

        public virtual void FSM_Awake()
        {
            Disable();
        }

        public virtual void FSM_Start()
        {
            //SetState(State.Disabled);
        }

        public void InitState(State target, BaseObject source = null)
        {
            _state = target;

            switch (_state)
            {
                case State.Falling:
                case State.FallingThrough:
                case State.SlopeMoving:
                case State.Moving:
                case State.Entering:
                    _hasArrived = false;
                    break;
                default:
                    _hasArrived = true;
                    break;
            }

            BaseObject above;

            if (source == null)
            {
                // Determine if object above to make it fall
                switch (target)
                {
                    case State.Moving:
                    case State.SlopeMoving:
                    case State.Falling:

                        if (_levelSession.Get(_previousGridPosition + Vector3Int.up, out above))
                        {
                            above.Cmd_Fall();
                        }

                        _state = target;
                        break;
                }
            }

        }


        #region Update

        public virtual void FSM_FixedUpdate()
        {
            switch (_state)
            {
                case State.Disabled:
                    break;

                case State.LevelSelect:

                    if (ColorId < PlayerManager.PlayerMax)
                    {
                        Color = Color.Lerp(_color, _nextColor, NextColorSpeed);
                    }

                    break;

                case State.Entering:
                case State.Falling:
                case State.Idle:
                case State.SlopeIdle:
                case State.Moving:
                case State.FallingThrough:
                case State.SlopeMoving:

                    Transform.position = Vector3.Lerp(
                        Transform.position,
                        _targetPosition,
                        StepSpeed);

                    float scale =
                        Mathf.Lerp(
                            Transform.localScale.x,
                            _targetScale,
                            ScaleSpeed);

                    Transform.localScale =
                        new Vector3(
                            scale,
                            scale,
                            scale);

                    break;
            }
        }

        public virtual void FSM_Update()
        {
            switch (_state)
            {
                case State.Disabled:
                    return;
                case State.LevelSelect:
                    return;

                case State.Idle:
                case State.SlopeIdle:
                    return;

                case State.Entering:
                case State.Falling:
                case State.Moving:
                case State.SlopeMoving:
                case State.FallingThrough:

                    if (_hasArrived) break;

                    if (VectorUtils.IsCloseEnough(
                        Transform.position,
                        _targetPosition))
                    {
                        _hasArrived = true;

                        if (_destination == null)
                        {
                            if (!_level.IsWithinBoundsY(_gridPosition.y - 1))
                            {
                                Cmd_FallThrough(_gridPosition + Vector3Int.down);
                            }
                            else if (_levelSession.Get(
                                _gridPosition + Vector3Int.down,
                                out BaseObject obj))
                            {
                                if (_state == State.Falling || _state == State.FallingThrough) Land();

                                Idle();
                            }
                            else Cmd_Fall();
                        }
                        else _destination.Accept(this);
                    }

                    break;
            }
        }

        #endregion

        #endregion        
    }
}

