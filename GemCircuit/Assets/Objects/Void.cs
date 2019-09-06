using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit.Objects
{
    public class Void : BaseObject
    {
        public override bool TryMove(Vector3 step, BaseObject incoming = null)
        {
            return false;
        }

        public override bool TryEnter(Vector3 step, BaseObject incoming = null)
        {
            switch (incoming.Id)
            {
                case ObjectId.Gem:
                    return true;
                case ObjectId.Character:
                    return false;
                default:
                    return false;
            }
        }


        protected override void Awake()
        {
            base.Awake();

            _visual.Enabled = false;
        }

        public override bool Visit(BaseObject incoming)
        {
            switch (incoming.Id)
            {
                case ObjectId.Gem:
                    return incoming.TryChangeState(StateMachine.State.Falling);                    
                default:
                    return false;
            }
        }
    }
}
