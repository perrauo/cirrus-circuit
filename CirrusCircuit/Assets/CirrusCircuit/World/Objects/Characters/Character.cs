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
        public override ObjectType Type => ObjectType.Character;

        public Player _controller;

        [SerializeField]
        private Guide _guide;

        public const float MoveDelay = 0.01f;
        
        public const float RotationSpeed = 0.6f;

        private const float MoveIdleTransitionTime = 0.6f;
        public override bool IsSlidable => false;

        public Vector3Int _inputDirection;

        [SerializeField]
        public Axes Axes;

        [SerializeField]
        public Animator Animator;

        private Timer _moveIdleTransitionTimer;

        private CharacterAnimatorWrapper _animatorWrapper;

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

        private bool _wasMovingVertical = false;

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

        private Coroutine _moveCoroutine = null;

        public Vector2Int GetDirectionBoundStep(
            int sign, 
            bool vertical)
        {

            var axis = new Vector2Int(_direction.x, _direction.z);

            if (vertical) return sign * axis;

            else
            {
                if (axis == Vector2Int.right) return sign * Vector2Int.down;

                else if (axis == Vector2Int.up) return sign * Vector2Int.right;

                else if (axis == Vector2Int.left) return sign * Vector2Int.up;

                else if (axis == Vector2Int.down) return sign * Vector2Int.left;

                else
                {
                    Debug.Assert(false, "Unknown direction");
                    return Vector2Int.down;
                }
            }

        }

        Vector2Int _inputDirection2 = Vector2Int.zero;

        // Input scheme world :
        // Press one direction, if not in same direction as player, then just rotate

        // Input scheme local
        // Press left, right, back, to rotate
        // Press forward to move forward

        public IEnumerator Coroutine_Cmd_Move(Vector2 axis)
        {          
            bool isAxisHorizontal = Mathf.Abs(axis.x) > 0.5f;
            bool isAxisVertical = Mathf.Abs(axis.y) > 0.5f;
            
            if (!isAxisHorizontal && !isAxisVertical)
            {
                _moveCoroutine = null;
                yield return null;
            }

            bool wasMovingVertical = _direction.z != 0;

            bool isMovingVertical =
                (isAxisHorizontal && isAxisVertical) ?                    
                    !wasMovingVertical :
                    isAxisVertical;

            Vector2Int signAxis = new Vector2Int(
                MathUtils.Sign(axis.x) * (isAxisHorizontal ? 1 : 0), 
                MathUtils.Sign(axis.y) * (isAxisVertical ? 1 : 0));

            int sign = isMovingVertical ? signAxis.y : signAxis.x;

            _inputDirection2 =
                Settings.AreControlsBoundToDirection.Boolean ?
                    GetDirectionBoundStep(
                        sign,
                        isMovingVertical) :
                    isMovingVertical ? signAxis.SetX(0) : signAxis.SetY(0);

            Move move = new Move
            {
                User = this,
                Position = _gridPosition,
                Step = new Vector3Int(
                    _inputDirection2.x * StepSize,
                    0,
                    _inputDirection2.y * StepSize)
            };


            switch (_state)
            {
                case ObjectState.Disabled:
                case ObjectState.Falling:                    
                    move.Type = MoveType.Direction;                    
                    break;

                default:
                    move.Type = MoveType.Moving;
                    break;
            }          


            if (
                _preserveInputDirection && 
                _inputDirection == move.Step)
            {
                move.Step = _direction * StepSize;
            }
            else
            {
                _inputDirection = move.Step;
                _preserveInputDirection = false;
            }

            Cmd_Move(move);            

            yield return new WaitForSeconds(MoveDelay);

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

        public override bool GetEnterResults(
            Move move, 
            out EnterResult result, 
            out IEnumerable<MoveResult> moveResults)
        {
            result = null;
            moveResults = null;
            return false;
        }

        public override void PerformAction(ObjectAction action)
        {
            base.PerformAction(action);

            switch (action)
            {
                case ObjectAction.Land:
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

        public void DoAction1()
        {
            //throw new NotImplementedException();
        }

        public float GetStateSpeed(CharacterAnimation state)
        {
            throw new NotImplementedException();
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
