﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit.World.Objects
{
    public class Breakable : BaseObject
    {
        public override ObjectId Id => ObjectId.Breakable;

        public override bool TryMove(Vector3Int step, BaseObject incoming = null)
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