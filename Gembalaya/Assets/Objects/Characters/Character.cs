//using Cirrus.Gembalaya.Objects.Actions;
//using Cirrus.Gembalaya.Objects.Attributes;
using Cirrus.Gembalaya.Objects.Characters.Controls;
//using Cirrus.Gembalaya.UI.HUD;
//using Cirrus.Gembalaya.Objects.Characters.Strategies;
using KinematicCharacterController;
using System.Collections;
using UnityEngine;
//using Cirrus.Gembalaya.Actions;
//using Cirrus.Gembalaya.Conditions;




namespace Cirrus.Gembalaya.Objects.Characters
{
    [System.Serializable]
    public struct Axes
    {
        public Vector2 Left;
        public Vector2 Right;
    }

    public class Character : BaseObject
    {
        [SerializeField]
        private Color _color = Color.red;

        [SerializeField]
        private Guide _guide;

        [SerializeField]
        public float _rotationSpeed = 0.6f;

        [SerializeField]
        public Axes Axes;

        [SerializeField]
        public Animator Animator;

        private Vector3 _targetDirection = Vector3.zero;

        private Vector3 _direction = Vector3.zero;

        private bool _wasMovingVertical = false;


        

        protected override void Awake()
        {
            base.Awake();
            
            
        }       
        
        protected void Start()
        {

        }

        public void Update()
        {

        }
         
        public void Jump()
        {
            
        }


        public override bool TryEnter()
        {
            return false;
        }


        // Use the same raycast to show guide.
        public void Move(Vector2 axis)
        {
            bool isMovingHorizontal = Mathf.Abs(axis.x) > 0.5f;
            bool isMovingVertical = Mathf.Abs(axis.y) > 0.5f;

            if (isMovingVertical && isMovingHorizontal)
            {
                //moving in both directions, prioritize later
                if (_wasMovingVertical)
                {
                    Vector3 step = new Vector3(_stepDistance * Mathf.Sign(axis.x), 0, 0);

                    if (TryMove(step))
                    {
                        _guide.Show(step);
                    }
                }
                else
                {
                    Vector3 step = new Vector3(0, 0, _stepDistance * Mathf.Sign(axis.y));

                    if (TryMove(step))
                    {
                        _guide.Show(step);
                    }
                }
            }
            else if (isMovingHorizontal)
            {
                Vector3 step = new Vector3(_stepDistance * Mathf.Sign(axis.x), 0, 0);

                if (TryMove(step))
                {
                    _guide.Show(step);
                    _wasMovingVertical = false;
                }
            }
            else if (isMovingVertical)
            {
                Vector3 step = new Vector3(0, 0, _stepDistance * Mathf.Sign(axis.y));

                if (TryMove(step))
                {
                    _guide.Show(step);
                    _wasMovingVertical = true;
                }
            }
                        
            // Smoothly interpolate from current to target look direction  
            _targetDirection = new Vector3(axis.x, 0.0f, axis.y);
            _direction = Vector3.Lerp(_direction, _targetDirection, _rotationSpeed).normalized;


            if (_direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(_direction, transform.up);
        }

    }

}
