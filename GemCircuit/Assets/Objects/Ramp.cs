using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit.Objects
{
    public class Ramp : BaseObject
    {
        public override bool TryMove(Vector3 step, BaseObject incoming = null)
        {
            switch (incoming.Id)
            {
                default:
                    return false;
            }
        }
        
        public override bool TryEnter(Vector3 step, BaseObject incoming = null)
        {
            if (Utils.Vectors.CloseEnough(step.normalized, Object.transform.forward))
            {
                if (_visitor == null)
                {
                    incoming._targetPosition += Vector3.up * Levels.Level.CubeSize / 2;
                }
                else if (_visitor.TryMove(step, incoming))
                {
                    incoming._targetPosition += Vector3.up * Levels.Level.CubeSize / 2;
                }
                else
                {
                    return false;
                }               

                return true;
            }

            return false;
        }



        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
        }

        public override bool Visit(BaseObject incoming)
        {
            switch (incoming.Id)
            {
                case ObjectId.Gem:
                case ObjectId.Character:
                    base.Visit(incoming);
                    return incoming.TryChangeState(StateMachine.State.RampIdle);
                default:
                    return false;
            }
        }
    }
}