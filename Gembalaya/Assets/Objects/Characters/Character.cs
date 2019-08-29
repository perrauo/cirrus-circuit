//using Cirrus.Gembalaya.Objects.Actions;
//using Cirrus.Gembalaya.Objects.Attributes;
using Cirrus.Gembalaya.Objects.Characters.Controls;
//using Cirrus.Gembalaya.UI.HUD;
//using Cirrus.Gembalaya.Objects.Characters.Strategies;
using KinematicCharacterController;
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
        public Axes Axes;


        [SerializeField]
        public Controls.Controller Controller;

        [SerializeField]
        public Animator Animator;


        [SerializeField]
        private Movements.MovementUser _movement;
        public Movements.MovementUser Movement { get { return _movement; } }


        [SerializeField]
        private Cirrus.FSM.Machine _fsm;
        public Cirrus.FSM.Machine FSM { get { return _fsm; } }
        public FSM.State State { get { return ((FSM.State)FSM.Top); } }


        protected override void Awake()
        {
            base.Awake();
            FSM.SetContext(this, 0);
            FSM.SetContext(_movement, 1);
            FSM.SetContext(Controller, 2);
        }       


        protected void Start()
        {
            //base.Start();
            FSM.Start();
        }

        public void Update()
        {
            FSM.DoUpdate();
        }

        protected void OnDrawGizmos()
        {
            //This is why we need FSM to be a MonoBehaviour of its own (Sorry mom) TODO: change back from class to monobehaviour
            FSM.OnDrawGizmos();
        }

  
        public void Jump()
        {
            State.Jump();
        }



        #region Kinematic Character Controller

        public void AfterCharacterUpdate(float deltaTime)
        {
            State.AfterCharacterUpdate(deltaTime);
        }


        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            State.OnGroundHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            State.BeforeCharacterUpdate(deltaTime);
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            State.OnMovementHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            State.PostGroundingUpdate(deltaTime);
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
            State.ProcessHitStabilityReport(hitCollider, hitNormal, hitPoint, atCharacterPosition, atCharacterRotation, ref hitStabilityReport);
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            State.UpdateRotation(ref currentRotation, deltaTime);
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            State.UpdateVelocity(ref currentVelocity, deltaTime);
        }


#endregion

    }

}
