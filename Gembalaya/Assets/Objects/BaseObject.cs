//using Cirrus.DH.Actions;
//using Cirrus.DH.Objects.Characters;
//using Cirrus.DH.Objects.Characters.Controls;
using System.Collections.Generic;
using UnityEngine;
using Cirrus.Tags;
//using Cirrus.DH.Conditions;
//using Cirrus.DH.Objects.Actions;

namespace Cirrus.Gembalaya.Objects
{
    //public delegate void OnMoved();

    public abstract partial class BaseObject : MonoBehaviour
    {
        public enum ObjectId
        {
            Default,
            Character,
            Gem,
            Door,
        }

        public virtual ObjectId Id { get { return ObjectId.Default; } }

        [SerializeField]
        protected GameObject _visual;

        [SerializeField]
        public Collider _collider;

        [SerializeField]
        protected StateMachine _stateMachine;

        [SerializeField]
        public float _stepDistance = 2f;

        [SerializeField]
        public float _stepSpeed = 0.6f;

        [SerializeField]
        public float _fallSpeed = 0.6f;

        [SerializeField]
        public float _fallDistance = 100f;

        [SerializeField]
        public float _scaleSpeed = 0.6f;

        public BaseObject _destination = null;

        public Vector3 _step;

        public Vector3 _targetPosition;

        public float _targetScale = 1;

        public string Name
        {
            get
            {
                if (transform.parent == null)
                    return "<Unknown>";
                else return transform.parent.name;
            }
        }

        protected virtual void Awake()
        {
            _targetPosition = transform.position;
            _targetScale = 1f;
        }

        public virtual void Start()
        {
            
        }

        public virtual void FixedUpdate()
        {

        }

        public virtual bool TryMove(Vector3 step, BaseObject incoming = null)
        {
            return _stateMachine.TryChangeState(StateMachine.State.Moving, step, incoming);
        }

        // If two items can cooexist
        public virtual bool Accept(BaseObject incoming)
        {
            return false;
        }

        public virtual void Fall()
        {
            _stateMachine.TryChangeState(StateMachine.State.Falling);
        }

        public virtual bool Enter()//Vector3 step, BaseObject incoming = null)
        {
            return _stateMachine.TryChangeState(StateMachine.State.Entering);
        }

    }
}
