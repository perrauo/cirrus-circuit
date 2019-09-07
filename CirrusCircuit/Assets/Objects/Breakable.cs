using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit.Objects
{
    public class Breakable : BaseObject
    { 
        public override bool TryMove(Vector3 step, BaseObject incoming = null)
        {
            return false;
        }

        // Start is called before the first frame update
        public override void Start()
        {

        }
    }
}