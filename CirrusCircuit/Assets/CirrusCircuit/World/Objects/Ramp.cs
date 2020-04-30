using Cirrus.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit.World.Objects
{
    public class Ramp : BaseObject
    {
        public override ObjectId Id => ObjectId.Ramp;

        public override bool TryMove(Vector3Int step, BaseObject incoming = null)
        {
            switch (incoming.Id)
            {
                default:
                    return false;
            }
        }

        public override bool TryEnter(Vector3Int step, ref Vector3 offset, BaseObject incoming = null)
        {
            if (step == _direction)
            {
                if (base.TryEnter(step, ref offset, incoming))
                {
                    _user = incoming;
                    offset += Vector3.up * Level.GridSize / 2;
                    return true;
                }
            }

            return false;
        }

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
        }

        public override void FSMFixedUpdate()
        {

        }

        public override void FSMUpdate()
        {

        }

        public override void Accept(BaseObject incoming)
        {
            incoming.TrySetState(BaseObject.State.RampIdle);
        }
    }
}