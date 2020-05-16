using Cirrus.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit.World.Objects
{
    public class Ramp : BaseObject
    {
        public override ObjectType Type => ObjectType.Ramp;

        public override bool Move(Vector3Int step, BaseObject source = null)
        {
            switch (source.Type)
            {
                default:
                    return false;
            }
        }

        public override bool Enter(
            Vector3Int step,
            BaseObject source,
            out Vector3 offset,
            out Vector3Int gridDest,
            out Vector3Int stepDest,
            out BaseObject dest)
        {
            offset = Vector3.zero;
            stepDest = step;
            gridDest = source._gridPosition;
            dest = this;

            if (step == _direction)
            {
                if (base.Enter(
                    step, 
                    source, 
                    out offset,
                    out gridDest,
                    out stepDest,
                    out dest))
                {
                    _visitor = source;
                    offset += Vector3.up * Level.CellSize / 2;
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

        public override void FSM_FixedUpdate()
        {

        }

        public override void FSM_Update()
        {

        }

        public override void Accept(BaseObject source)
        {
            source.RampIdle();
        }
    }
}