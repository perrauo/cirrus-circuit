//using Cirrus.Circuit.World.Objects.Actions;
//using Cirrus.Circuit.World.Objects.Attributes;
using Cirrus.Circuit.Controls;
//using Cirrus.Circuit.World.Objects.Characters.Controls;
using Cirrus;
//using Cirrus.Circuit.UI.HUD;
//using Cirrus.Circuit.World.Objects.Characters.Strategies;
//using KinematicCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Cirrus.Circuit.Actions;
//using Cirrus.Circuit.Conditions;

using UnityInputs = UnityEngine.InputSystem;
//using Cirrus.Circuit.Playable;
using Cirrus;
using UnityEngine.Assertions;

namespace Cirrus.Circuit.World.Objects.Characters
{
    [Serializable]
    public struct Axes
    {
        public Vector2 Left;
        public Vector2 Right;
    }

    public class Character : BaseObject, ICharacterAnimatorWrapper
    {
        [Header("----------------------------", order = 0)]
        [Header("Character", order = 1)]
        [Header("----------------------------", order = 2)]
        [SerializeField]
        private Guide _guide;
        
        public override ObjectType Type => ObjectType.Character;

        public Player _controller;

        public const float MoveDelay = 0.01f;
        
        public const float RotationSpeed = 0.6f;

        private const float MoveIdleTransitionTime = 0.6f;
        public override bool IsSlidable => false;

        //public Delegate OnHeldReleasedHandler;
        //public Delegate<BaseObject> OnForceReleaseHoldHandler;

        public Vector3Int _inputDirection;

        public Vector3Int _previousStep;

        [SerializeField]
        public Axes Axes;

        [SerializeField]
        public Animator Animator;

        private Timer _moveIdleTransitionTimer;

        private CharacterAnimatorWrapper _animatorWrapper;

        private Coroutine _moveCoroutine = null;

        private const float HoldTimerTime = 1f;

        private Timer _holdTimerRefresh;

        public override Color Color {

            get => _color;            

            set
            {
                _color = value;
                if (_guide != null)
                {
                    _guide.Color = _color;
                }
            }
        }

        public float BaseLayerLayerWeight { set => throw new NotImplementedException(); }

        public override void Awake()
        {
            base.Awake();

            _moveIdleTransitionTimer = new Timer(MoveIdleTransitionTime, start: false, repeat: false);
            _moveIdleTransitionTimer.OnTimeLimitHandler += OnMoveIdleTransitionTimeout;
            _animatorWrapper = new CharacterAnimatorWrapper(Animator);

            _inputDirection = Transform.forward.ToVector3Int();

            _holdTimerRefresh = new Timer(HoldTimerTime, start: false, repeat: false);
            _holdTimerRefresh.OnTimeLimitHandler += () => BeginHold();
        }

        public override void Start()
        {
            base.Start();
        }

        public override void FSM_SetState(ObjectState state, BaseObject source = null)
        {
            base.FSM_SetState(state, source);

            switch (state)
            {
                case ObjectState.CharacterSelect:
                    Play(CharacterAnimation.Character_Idle);
                    break;

                case ObjectState.Falling:
                    break;


                case ObjectState.Idle:
                    break;



                case ObjectState.Moving:
                    break;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public void OnMoveIdleTransitionTimeout()
        {
            Play(CharacterAnimation.Character_Idle);
        }


        public override void OnRoundEnd()
        {
            // Remove callbacks??
            //_controller.SetCallbacks(this);
        }

        // Cancel
        public void OnAction0(UnityInputs.InputAction.CallbackContext context)
        {
            //Game.Instance.HandleAction0(this);
        }

        // Accept
        public void OnAction1(UnityInputs.InputAction.CallbackContext context)
        {
            //Game.Instance.HandleAction1(this);
        }

        public bool GetMoveAlt(
            int sign, 
            bool vertical, 
            ref Move move)
        {
            move.Type = MoveType.Moving;

            var axis = new Vector2Int(_direction.x, _direction.z);

            if (vertical && sign > 0) move.Step = _direction;

            else if (vertical) move.Step = -_direction;

            else if (axis == Vector2Int.right) move.Step = (sign * Vector2Int.down).ToVector3IntAlt();

            else if (axis == Vector2Int.up) move.Step = (sign * Vector2Int.right).ToVector3IntAlt();

            else if (axis == Vector2Int.left) move.Step = (sign * Vector2Int.up).ToVector3IntAlt();

            else if (axis == Vector2Int.down) move.Step = (sign * Vector2Int.left).ToVector3IntAlt();

            else
            {
                Debug.Assert(
                    false, 
                    "Unknown direction");
                return false;
            }

            move.Type = 
                move.Step == _direction ? 
                    MoveType.Moving : 
                    MoveType.Direction;
            return true;
        }

        public bool GetMove(
            int sign,
            bool vertical,
            ref Move move)
        {
            move.Step = vertical ? new Vector3Int(0, 0, sign) : new Vector3Int(sign, 0, 0);
            move.Type = 
                (move.Step == _previousStep || _heldAction != null) ?                 
                    MoveType.Moving : 
                    MoveType.Direction;
            
            _previousStep = move.Step;

            if (
                _preserveInputDirection && 
                _inputDirection == move.Step)
            {
                move.Step = _direction;
            }
            else
            {
                _inputDirection = move.Step;
                _preserveInputDirection = false;
            }

            return true;
        }


        public IEnumerator Coroutine_Cmd_Move(Vector2 axis)
        {          
            bool isAxisHorizontal = Mathf.Abs(axis.x) > 0.5f;
            bool isAxisVertical = Mathf.Abs(axis.y) > 0.5f;

            if (!isAxisHorizontal && !isAxisVertical)
            {
                _moveCoroutine = null;
                yield break;
            }
           
            bool isMovingVertical =
                (isAxisHorizontal && isAxisVertical) ?
                    _direction.z == 0 :
                    isAxisVertical;

            Vector2Int signAxis = new Vector2Int(
                MathUtils.Sign(axis.x) * (isAxisHorizontal ? 1 : 0),
                MathUtils.Sign(axis.y) * (isAxisVertical ? 1 : 0));

            int sign = isMovingVertical ? signAxis.y : signAxis.x;

            var move = new Move()
            {
                User = this,
                Position = _levelPosition,
                Type = MoveType.Direction,
                Entered = _entered,                
            };

            if (Settings.IsUsingAlternateControlScheme.Boolean ?
                GetMoveAlt(
                    sign,
                    isMovingVertical,
                    ref move) :
                GetMove(
                    sign,
                    isMovingVertical,
                    ref move))
            {
                switch (_state)
                {
                    case ObjectState.Disabled:
                    case ObjectState.Falling:
                        move.Type = MoveType.Direction;
                        break;
                }

                Cmd_Move(move);
                yield return new WaitForSeconds(MoveDelay);
            }



            _moveCoroutine = null;
            yield return null;
        }

        // Use the same raycast to show guide
        public void Cmd_Move(Vector2 axis)
        {
            switch (_state)
            {               
                case ObjectState.Moving:
                case ObjectState.Falling:
                case ObjectState.Idle:
                case ObjectState.Disabled:
                case ObjectState.Climbing:
                    
                    if (_moveCoroutine == null) _moveCoroutine = StartCoroutine(Coroutine_Cmd_Move(axis));

                    break;

                default:
                    break;

            }
        }

        public override void ApplyMoveResult(MoveResult result)
        {
            base.ApplyMoveResult(result);

            switch (result.MoveType)
            {
                case MoveType.Moving:
                    Play(CharacterAnimation.Character_Walking, false);
                    //_guide.Show(move.Step);
                break;
            }
        }

        public override ReturnType GetEnterResults(
            Move move, 
            out EnterResult result, 
            out IEnumerable<MoveResult> moveResults)
        {
            result = null;
            moveResults = null;
            return ReturnType.Failed;
        }

        public override void ApplyAction(Action action)
        {
            base.ApplyAction(action);

            switch (action.Type)
            {
                case ActionType.Land:
                    Play(CharacterAnimation.Character_Falling);
                    break;                    
            }
        }

        public override void FSM_Update()
        {
           base.FSM_Update();
        }

        public void HandleAction0()
        {
            //throw new NotImplementedException();
        }

        public void ReleaseHold()
        {
            _holdTimerRefresh.Stop();

            if (_heldAction != null)
            {
                _heldAction.Target._holding.Remove(this);
                _heldAction.Target.ApplyPhysics();
                _heldAction = null;

                Cmd_Perform(new Action { Type = ActionType.ReleaseHold });
            }            
        }

        public void BeginHold()
        {
            _holdTimerRefresh.Start();

            if (_heldAction == null)
            {
                /*onst Vector3Int[] directions = { new Vector3Int() }*/
                // TODO use server levelsession
                //

                int idx = Utils.DirectionToIndex(_direction);
                if (idx < 0) return;

                for (int i = idx; i < idx + 4; i++)
                {
                    int j = i % 4;

                    if (LevelSession.Instance.Get(
                        _levelPosition + Utils.Directions[j],
                        out BaseObject obj))
                    {
                        _heldAction = new Action
                        {
                            Type = ActionType.BeginHold,
                            Source = this,
                            Target = obj,
                            Direction = Utils.Directions[j]
                        };

                        _heldAction.Target._holding.Add(this);
                        _holdTimerRefresh.Stop();

                        Cmd_Perform(_heldAction);

                        return;
                    }
                }
            }
        }

        public float GetStateSpeed(CharacterAnimation state)
        {
            //throw new NotImplementedException();
            return -1;
        }

        public void Play(CharacterAnimation animation, float normalizedTime, bool reset = true)
        {
            _animatorWrapper.Play(animation, normalizedTime, reset);
        }

        public void Play(
            CharacterAnimation animation, 
            bool reset = true)
        {
            _animatorWrapper.Play(animation, reset);
        }
    }

}
