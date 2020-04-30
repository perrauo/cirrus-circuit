//using Cirrus.Circuit.World.Objects.Actions;
//using Cirrus.Circuit.World.Objects.Attributes;
using Cirrus.Circuit.Controls;
using Cirrus.Circuit.World.Objects.Characters.Controls;
//using Cirrus.Circuit.UI.HUD;
//using Cirrus.Circuit.World.Objects.Characters.Strategies;
using KinematicCharacterController;
using System;
using System.Collections;
using UnityEngine;
//using Cirrus.Circuit.Actions;
//using Cirrus.Circuit.Conditions;

using UnityInputs = UnityEngine.InputSystem;
//using Cirrus.Circuit.Playable;

namespace Cirrus.Circuit.World.Objects.Characters
{
    [Serializable]
    public struct Axes
    {
        public Vector2 Left;
        public Vector2 Right;
    }

    public class Character : BaseObject
    {
        public override ObjectId Id => ObjectId.Character;

        public Player _controller;

        [SerializeField]
        private Guide _guide;

        [SerializeField]
        public float _moveDelay = 0.6f;

        [SerializeField]
        public float _rotationSpeed = 0.6f;

        [SerializeField]
        public Axes Axes;

        [SerializeField]
        public Animator Animator;

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

        protected override void Awake()
        {
            base.Awake(); 
        }

        public override void Start()
        {
            base.Start();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        private bool _wasMovingVertical = false;

        public override void OnRound()
        {
            base.OnRound();

            //_controller.SetCallbacks(this);
        }

        public override bool TrySetState(State transition, params object[] args)
        {
            return base.TrySetState(transition, args);
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

        private bool _moveCoroutineActive = false;

        public IEnumerator MoveCoroutine(Vector2 axis)
        {
            _moveCoroutineActive = true;

            bool isMovingHorizontal = Mathf.Abs(axis.x) > 0.5f;
            bool isMovingVertical = Mathf.Abs(axis.y) > 0.5f;

            Vector3Int stepHorizontal = new Vector3Int(_stepSize * Math.Sign(axis.x), 0, 0);
            Vector3Int stepVertical = new Vector3Int(0, 0, _stepSize * Math.Sign(axis.y));

            if (isMovingVertical && isMovingHorizontal)
            {
                //moving in both directions, prioritize later
                if (_wasMovingVertical)
                {
                    base.TryMove(stepHorizontal);
                    _guide.Show(stepHorizontal);
                }
                else
                {
                    base.TryMove(stepVertical);
                    _guide.Show(stepVertical);
                }
            }
            else if (isMovingHorizontal)
            {
                TryMove(stepHorizontal);                
                _guide.Show(stepHorizontal);
                _wasMovingVertical = false;                
            }
            else if (isMovingVertical)
            {
                TryMove(stepVertical);                
                _guide.Show(stepVertical);
                _wasMovingVertical = true;                
            }

            yield return new WaitForSeconds(_moveDelay);
            _moveCoroutineActive = false;
            yield return null;
        }

        private IEnumerator _moveCoroutine;

        // Use the same raycast to show guide
        public void TryMove(Vector2 axis)
        {
            if(!_moveCoroutineActive) StartCoroutine(MoveCoroutine(axis));
        }


        public override void FSMUpdate()
        {
            base.FSMUpdate();


            switch (_state)
            {
                case State.LevelSelect:
                    break;

                case State.Moving:
                case State.Falling:
                case State.Entering:
                case State.Idle:
                case State.RampIdle:
                    
                    if (_direction != Vector3.zero)
                        Transform.transform.rotation = Quaternion.LookRotation(_direction, Transform.transform.up);
                    break;

            }


        }

        public void TryAction0()
        {
            //throw new NotImplementedException();
        }

        public void TryAction1()
        {
            //throw new NotImplementedException();
        }
    }

}
