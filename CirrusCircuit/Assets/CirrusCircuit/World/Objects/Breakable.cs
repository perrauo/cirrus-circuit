using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit.World.Objects
{
    public class Breakable : BaseObject
    {
        public override ObjectType Type => ObjectType.Breakable;

        public override bool Move(Vector3Int step, BaseObject source = null)
        {
            return false;
        }

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
        }
    }
}