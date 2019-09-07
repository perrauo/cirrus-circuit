using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Cirrus.FSM
{
    public abstract class State
    {
        public Resource resource;
        public object[] context;

        public int Id { get { return resource.Id; } }

        public State(object[] context, Resource resource)
        {
            this.resource = resource;
            this.context = context;
        }

        public virtual void Enter(params object[] args) { }
        public virtual void Exit() { }
        public virtual void Reenter(params object[] args) { }

        public virtual void BeginUpdate() { }
        public virtual void EndUpdate() { }

        public virtual void BeginFixedUpdate() { }
        public virtual void EndFixedUpdate() { }

        virtual public void OnDrawGizmos() { }

        //[SerializeField]
        //public List<Transition> Transitions;
    }

}
