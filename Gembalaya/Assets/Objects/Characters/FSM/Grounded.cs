//using Cirrus.Gembalaya.Objects.Actions;
using UnityEngine;

namespace Cirrus.Gembalaya.Objects.Characters.FSM
{
    [CreateAssetMenu(menuName = "Cirrus/Objects/Characters/FSM/Grounded")]
    public class Grounded : Resource
    {
        override public int Id { get { return (int)FSM.Id.Grounded; } }


        public override Cirrus.FSM.State Create(object[] context)
        {
            return new State(context, this);
        }

        new public class State : FSM.State
        {
            public State(object[] context, Cirrus.FSM.Resource resource) : base(context, resource) { }

            public override void Enter(params object[] args)
            {
                if (Character.Animator != null)
                {
                    Character.Animator.Play("Idle");
                }
                //Character.CharacterController.Speed.y = 0;
                Character.Physic.BaseVelocity.y = 0;
            }

            public override void Reenter(params object[] args)
            {
                Character.Physic.BaseVelocity.y = 0;
            }


            public override void BeforeCharacterUpdate(float deltaTime)
            {
                base.BeforeCharacterUpdate(deltaTime);
            }
            

            public override void PostGroundingUpdate(float deltaTime)
            {
                base.PostGroundingUpdate(deltaTime);

                if (!Character.Movement.Motor.GroundingStatus.IsStableOnGround)
                {
                    Character.FSM.TrySetState((int)FSM.Id.Airborne);
                    return;
                }
            }
 
            public override void Jump()
            {
                if (!IsMovementInputEnabled)
                    return;

                Character.FSM.TrySetState((int)FSM.Id.Jump);
            }

            public override void Injure()
            {
                Character.FSM.TrySetState(FSM.Id.InjuredGrounded);
            }


            //

       

            public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
            {
                base.UpdateVelocity(ref currentVelocity, deltaTime);
                Character.Animator.SetFloat("Speed", currentVelocity.magnitude/Character.Movement.Resource.MaxSpeed);

            }

        }


    }

}