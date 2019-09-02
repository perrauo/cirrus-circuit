//using UnityEngine;
//using System.Collections;
//using Cirrus.FSM;

//namespace Cirrus.Gembalaya.Objects.FSM
//{
//    public class Falling : Cirrus.FSM.Resource
//    {
//        override public int Id { get { return -1; } }

//        public override Cirrus.FSM.State Create(object[] context)
//        {
//            throw new System.NotImplementedException();
//        }

//        public class State : Cirrus.FSM.State
//        {
//            public State(object[] context, Cirrus.FSM.Resource resource) : base(context, resource) { }
//            protected BaseObject Object { get { return (BaseObject)context[0]; } }
//            public override void Enter(params object[] args) { }
//            public override void Exit() { }
//            public override void BeginUpdate() { }
//            public override void EndUpdate()
//            {

//            }
//        }
//    }
//}
