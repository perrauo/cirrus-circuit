using UnityEngine;
using UnityEditor;
using KinematicCharacterController;
using System;
using System.Linq;



// Character state

namespace Cirrus.Gembalaya.Objects.Characters.FSM
{
    [System.Serializable]
    public enum Id
    {
        Action = 1 << 1,
        Grounded = 1 << 2,
        Airborne = 1 << 3,
        Jump = 1 << 4,
        Dead = 1 << 5,
        InjuredGrounded = 1 << 6,
        InjuredAirborne = 1 << 7,
    }

    // We don't care if we can't instantiate this SO, because it's abstract
    public abstract class Resource : Cirrus.FSM.Resource
    {
        override public int Id { get { return -1; } }
    }

    public abstract class State : Cirrus.FSM.State
    {
        public State(object[] context, Cirrus.FSM.Resource resource) : base(context, resource) { }
        protected Character Character { get { return (Character)context[0]; } }
        protected Movements.MovementUser KinematicController { get { return (Movements.MovementUser)context[1]; } }

        public override void Enter(params object[] args) { }
        public override void Exit() { }
        public override void BeginTick() { }
        public override void EndTick()
        {
            //var health = Character.Persistence.Attributes.Health;


        }

        public virtual void Jump() { }
        
        public virtual void Injure()
        {
            if (Character.Animator != null)
            {
                Character.Animator.Play("Injured");
                Character.FSM.TrySetState(FSM.Id.InjuredGrounded);
            }
        }


        public virtual void Heals() { }

        protected virtual bool IsMovementInputEnabled { get { return true; } }

        private Vector3 _targetDirection = Vector3.zero;
        private Vector3 _direction = Vector3.zero;



        public virtual bool IsColliderValidForCollisions(Collider coll) { return true; }

        public virtual void OnGroundHit(
            Collider hitCollider, 
            Vector3 hitNormal, 
            Vector3 hitPoint, 
            ref HitStabilityReport hitStabilityReport) { }

        public virtual void OnMovementHit(
            Collider hitCollider, 
            Vector3 hitNormal, 
            Vector3 hitPoint, 
            ref HitStabilityReport hitStabilityReport) { }

        public virtual void PostGroundingUpdate(float deltaTime) { }

        public virtual void ProcessHitStabilityReport(
            Collider hitCollider, 
            Vector3 hitNormal, 
            Vector3 hitPoint,
            Vector3 atCharacterPosition, 
            Quaternion atCharacterRotation, 
            ref HitStabilityReport hitStabilityReport){ }


        public virtual void BeforeCharacterUpdate(float deltaTime)
        {
            if (!IsMovementInputEnabled)
                return;

            Vector2 a =
                Character.Axes.Left *
                Character.Movement.Resource.MaxSpeed;

            
            Vector3 b = new Vector3(a.x, 0, a.y);

            Character.Physic.MoveVelocity = Vector3.Lerp(Character.Physic.MoveVelocity, b, KinematicController.Resource.SpeedSmooth);





        }

        public virtual void AfterCharacterUpdate(float deltaTime) { }



        public virtual void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (!IsMovementInputEnabled)
                return;


      
            {
                // Smoothly interpolate from current to target look direction  
                _targetDirection = new Vector3(Character.Axes.Left.x, 0.0f, Character.Axes.Left.y);
                _direction = Vector3.Lerp(_direction, _targetDirection, KinematicController.Resource.RotationSpeed).normalized;


                if (_direction != Vector3.zero)
                    currentRotation = Quaternion.LookRotation(_direction, Character.transform.up);
            }


        }

        public virtual void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
              currentVelocity = Character.Physic.TotalVelocity;
        }

    }


}
