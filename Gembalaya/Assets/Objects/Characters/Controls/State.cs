//using Cirrus.Gembalaya.Controls;
using Cirrus.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
// AIController States

namespace Cirrus.Gembalaya.Objects.Characters.Controls.FSM
{
    public abstract class Resource : Cirrus.FSM.Resource
    {
        override public int Id { get { return -1; } }

        [SerializeField]
        public float RefreshRate = 2f;

        // Raycast to determine if slop, or jump
        [SerializeField]
        public float SlopeRaycastLenght = 2f;

        [SerializeField]
        public float AxesLeftStep = 0.4f;

        [SerializeField]
        public float AxesRightStep = 0.4f;

        //public override
        //    Cirrus.FSM.State Create(object[] context);

    }

    public enum Id
    {
        Human,
        Decide,
        Action,
        UseAction,
        Wander,
        Patrol,
        Observe,
        Follow,
        Decision,
        Idle,
        Escape,
        Injured
    }

    public abstract class State : Cirrus.FSM.State
    {
        public virtual Vector2 AxesLeft
        {
            get
            {
                return Character.Axes.Left;
            }

            set
            {
                Character.Axes.Left = value;
            }
        }

        public virtual Vector2 AxesRight
        {
            get
            {
                return Character.Axes.Right;
            }

            set
            {
                Character.Axes.Right = value;
            }
        }


        new protected virtual Resource Resource { get { return (Resource)resource; } }
        protected virtual Agent Agent { get { return (Agent)context[0]; } }
        protected virtual Controller Controller { get { return (Controller)context[1]; } }
        protected virtual Character Character { get { return (Character)context[2]; } }

        protected Vector2 _targetAxesLeft = Vector2.zero;

        protected Vector2 _targetAxesRight = Vector2.zero;

        protected IEnumerator _calculatePathCoroutine;

        protected bool _isCalculatingPath = false;

        protected Vector3 _sampledPosition = Vector3.zero;

        protected Vector3 _destination = Vector3.positiveInfinity;

        protected float _arrivalTolerance = 2.0f;

        protected float _destinationRange = 2.0f;


        public State(object[] context, Resource resource) : base(context, resource)
        {

        }

        // Override if you do not want to reconsider


        public override void Enter(params object[] args)
        {
            Character.Axes.Left = Vector2.zero;
            _targetAxesLeft = Vector2.zero;
        }

        public override void Exit() { }


        public override void BeginTick()
        {
         
        }

        public override void EndTick()
        {
            //Agent.NavMeshAgent.


            Character.Axes.Left = Vector3.Lerp(Character.Axes.Left, _targetAxesLeft, Resource.AxesLeftStep);
            Character.Axes.Right = Vector3.Lerp(Character.Axes.Right, _targetAxesRight, Resource.AxesRightStep);
        }



        public virtual void Move()
        {
            //Agent.NavMeshAgent.transform.position = Character.transform.position;
            //Agent.NavMeshAgent.velocity = Character.CharacterController.Motor.Velocity;

            //float step = Agent.NavMeshAgent.desiredVelocity.magnitude / Agent.NavMeshAgent.speed;
            //Vector2 dir = new Vector2(Agent.NavMeshAgent.desiredVelocity.x, Agent.NavMeshAgent.desiredVelocity.z).normalized;
            //_targetAxesLeft = Vector2.Lerp(Vector2.zero, dir, step);

            //Debug.DrawRay(Character.transform.position, new Vector3(dir.x, 0, dir.y)*10, Color.blue);

            //var ray = new Ray(Character.transform.position + (Vector3.up * 0.5f), dir);
            //if (Physics.Raycast(ray, out RaycastHit hit, Resource.SlopeRaycastLenght, Game.Instance.Layers.LayoutFlags))
            //{
            //    if (hit.point.y > Character.transform.position.y)
            //    {
            //        Character.Jump();
            //    }
            //}
        }


        public override void OnDrawGizmos()
        {        //{
        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawSphere(Character.transform.position, 2f);   
        }


        protected virtual IEnumerator CalculatePathCoroutine(float waitTime)
        {
            yield return null;
        }


        public void Jump()
        {
            Character.Jump();
        }


    }
}
