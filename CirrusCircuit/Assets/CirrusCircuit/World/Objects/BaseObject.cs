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
            Unknown,
            Disabled,
            LevelSelect,
            Entering,
            Falling,
            FallingThrough,
            Idle,
            //SlopeIdle,
            Moving,
            //SlopeMoving
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

        public BaseObject _entered = null;

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
        public Level Level => LevelSession.Instance.Level;

        public LevelSession LevelSession => LevelSession.Instance;

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
            (transform.position, _gridPosition) = level.RegisterObject(this);
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

        // TODO : OnMoved
        public virtual void Cmd_Interact(BaseObject source)
        {
            _session.Cmd_Interact(source);
        }

        #endregion

        #region Move

        public virtual void Cmd_Move(Move move)
        {
            _session.Cmd_Move(move);
        }

        public virtual void Move(MoveResult result)
        {
            _entered = result.Entered;
            _gridPosition = result.Destination;
            _targetPosition = Level.GridToWorld(result.Destination);
            _targetPosition += result.Offset;
            _direction = result.Step;


            //destination.
            if (result.Moved != null)
            {
                result.Moved.Interact(this);
            }

            if (_entered != null)
            {
                _entered.Exit(this);
                _entered = null;
            }

            if (result.Entered != null) result.Entered.Enter(result);

            LevelSession.Instance.ApplyResult(result);
            InitState(
                result.State,
                this);
        }


        public virtual void Move(IEnumerable<MoveResult> results)
        {
            foreach (var result in results)
            {
                if (result == null) continue;
                result.Move.User.Move(result);
            }
        }


        public virtual bool GetMoveResults(
            Move move, 
            out IEnumerable<MoveResult> results)
        {
            return LevelSession.GetMoveResults(
               move,
               out results);
        }


        #endregion

        #region Enter


        public virtual void Enter(MoveResult result)
        {
            if (_visitor != null)
            {
                _visitor.GetMoveResults(new Move
                {

                },
                out IEnumerable<MoveResult> enterResult);
            }
            //else e            
        }

        public virtual bool GetEnterResult(
            Move move,            
            out MoveResult result)
        {
            result = new MoveResult();

            //if (_visitor != null) return _visitor.IsMoveAllowed(move);

            return true;
        }

        #endregion

        #region Exit

        public virtual void OnExited()
        {
            _exitScaleTimer.Start();
            _targetScale = 0;
        }

        // Ramp exit with offset
        public virtual bool GetExitValue(
            Move move,
            out MoveResult result)
        {
            result = new MoveResult();
            return true;
        }

        public virtual void Exit(BaseObject source)
        {

        }


        #endregion

        #region Idle

        public virtual void Cmd_Idle()
        { 
        
            _session.Cmd_Idle();
        }


        // TODO play some anim
        public virtual void Idle()
        {
            InitState(State.Idle, null);
        }


        #endregion


        #region Land

        public virtual void Land()
        { 
        
        }
        public virtual void Cmd_Land()
        {
            _session.Cmd_Land();
        }


        #endregion


        #region Fall

        public virtual void Cmd_Fall()
        {
            _session.Cmd_Fall();
        }

        // TODO play some anim
        public virtual void Fall()
        {
            InitState(State.Falling, null);
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


        // Common to all state or subset of states
        public void InitState(State target, BaseObject source = null)
        {
            _state = target;

            switch (_state)
            {
                case State.Falling:
                case State.FallingThrough:
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
                    case State.Falling:

                        if (LevelSession.Get(
                            _previousGridPosition + Vector3Int.up, 
                            out above))
                        {
                            //above.Cmd_Fall();
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
                case State.Moving:
                case State.FallingThrough:

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
                    return;

                case State.Entering:
                case State.Falling:
                case State.Moving:
                case State.FallingThrough:

                    if (_hasArrived) break;

                    if (VectorUtils.IsCloseEnough(
                        Transform.position,
                        _targetPosition))
                    {
                        _hasArrived = true;

                        if (_entered == null)
                        {                            
                            if (LevelSession.Get(
                                _gridPosition + Vector3Int.down,
                                out BaseObject obj))
                            {
                                if (_state == State.Falling || _state == State.FallingThrough) Land();

                                Cmd_Idle();
                            }
                            else Cmd_Fall();
                        }
                    }

                    break;
            }
        }

        #endregion

        #endregion        
    }
}

