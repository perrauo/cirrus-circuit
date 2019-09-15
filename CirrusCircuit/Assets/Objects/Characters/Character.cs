//using Cirrus.Circuit.Objects.Actions;
//using Cirrus.Circuit.Objects.Attributes;
using Cirrus.Circuit.Objects.Characters.Controls;
//using Cirrus.Circuit.UI.HUD;
//using Cirrus.Circuit.Objects.Characters.Strategies;
using KinematicCharacterController;
using System;
using System.Collections;
using UnityEngine;
//using Cirrus.Circuit.Actions;
//using Cirrus.Circuit.Conditions;

namespace Cirrus.Circuit.Objects.Characters
{
    [System.Serializable]
    public struct Axes
    {
        public Vector2 Left;
        public Vector2 Right;
    }

    public class Character : BaseObject
    {
        public override ObjectId Id { get { return ObjectId.Character; } }

        [SerializeField]
        private Guide _guide;

        [SerializeField]
        public float _rotationSpeed = 0.6f;

        [SerializeField]
        public Axes Axes;

        [SerializeField]
        public Animator Animator;

        //private Vector3 _targetDirection = Vector3.zero;

        //private Vector3 _direction = Vector3.zero;

        private bool _wasMovingVertical = false;

        public override Color Color {

            get
            {
                return _color;
            }

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

        // Use the same raycast to show guide
        public void TryMove(Vector2 axis)
        {
            bool isMovingHorizontal = Mathf.Abs(axis.x) > 0.5f;
            bool isMovingVertical = Mathf.Abs(axis.y) > 0.5f;

            Vector3Int stepHorizontal = new Vector3Int(_stepSize * Math.Sign(axis.x), 0, 0);
            Vector3Int stepVertical = new Vector3Int(0, 0, _stepSize * Math.Sign(axis.y));

            if (isMovingVertical && isMovingHorizontal)
            {
                //moving in both directions, prioritize later
                if (_wasMovingVertical)
                {
                    if (base.TryMove(stepHorizontal))
                    {
                        _guide.Show(stepHorizontal);
                    }
                }
                else
                {
                    if (base.TryMove(stepVertical))
                    {
                        _guide.Show(stepVertical);
                    }
                }
            }
            else if (isMovingHorizontal)
            {
                if (base.TryMove(stepHorizontal))
                {
                    _guide.Show(stepHorizontal);
                    _wasMovingVertical = false;
                }
            }
            else if (isMovingVertical)
            {
                if (base.TryMove(stepVertical))
                {
                    _guide.Show(stepVertical);
                    _wasMovingVertical = true;
                }
            }
                        
        }


        public override void FSMUpdate()
        {
            base.FSMUpdate();


            switch (_state)
            {
                case State.Disabled:
                    break;

                case State.Moving:
                case State.Falling:
                case State.Entering:
                case State.Idle:
                case State.RampIdle:
                    
                    if (_direction != Vector3.zero)
                        Object.transform.rotation = Quaternion.LookRotation(_direction, Object.transform.up);
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
