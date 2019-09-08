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
                incoming._targetPosition += Vector3.up * Levels.Level.BlockSize / 2;
                return true;
            }

            return false;
        }



        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
        }

        public override void Accept(BaseObject incoming)
        {
            incoming.TryChangeState(FSM.State.RampIdle);
        }
    }
}