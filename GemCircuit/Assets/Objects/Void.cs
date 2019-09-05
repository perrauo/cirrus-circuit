using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.GemCircuit.Objects
{
    public class Void : BaseObject
    {
        public override bool TryMove(Vector3 step, BaseObject incoming = null)
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

        public override bool Accept(BaseObject incoming)
        {
            incoming.Fall();
            return true;
        }
    }
}
