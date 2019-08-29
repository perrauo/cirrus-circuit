using System;
using System.Collections;
using System.Collections.Generic;
using Cirrus.FSM;
using UnityEngine;


// TODO:  OVERRIDE ALL BASE FUNCTIONS WITHOUT DOING ANYTHING

namespace Cirrus.Gembalaya.Objects.Characters.Controls.FSM
{
    public class HumanState : Resource
    {
        override public int Id { get { return (int)FSM.Id.Human; } }

        public override Cirrus.FSM.State Create(object[] context)
        {
            return new State(context, this);
        }

        public class State : FSM.State
        {
            public State(object[] context, Resource resource) : base(context, resource) { }

  

            public override void Enter(params object[] args)
            {

            }



            public override void Exit() { }



            public override void BeginTick()
            {

            }


            public override void EndTick()
            {

            }
        }

    }

}
