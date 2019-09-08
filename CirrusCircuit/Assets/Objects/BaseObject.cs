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
        public enum ObjectId
        {
            Default,
            Character,
            Gem,
            Door,
        }

        public virtual ObjectId Id { get { return ObjectId.Default; } }

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
        protected FSM _stateMachine;

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

        public float _targetScale = 1;

        [SerializeField]
        public Controls.PlayerNumber PlayerNumber;

        //protected BaseObject _visitor;


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

        protected virtual void Awake()
        {
            _targetPosition = Object.transform.position;
            _targetScale = 1f;
        }

        public virtual void Start()
        {
            
        }

        public virtual void FixedUpdate()
        {

        }

        //public virtual void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.red;

        //    Gizmos.DrawSphere(_targetPosition, 0.1f);
        //}

        public bool TryChangeState(FSM.State state, params object[] args)
        {
            return _stateMachine.TryChangeState(state, args);
        }

        public virtual bool TryMove(Vector3 step, BaseObject incoming = null)
        {
            return _stateMachine.TryChangeState(FSM.State.Moving, step, incoming);
        }

        public virtual bool TryEnter(Vector3 step, BaseObject incoming = null)
        {
            return false;
        }

        public virtual void Accept(BaseObject incoming)
        {
            //incoming.TryChangeState
        }



    }
}
