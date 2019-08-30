using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Gembalaya.Objects
{
    public class Breakable : BaseObject
    {

        public override bool TryMove(Vector3 step, Status stat = null, BaseObject incoming = null)
        {
            return false;
        }

        // Start is called before the first frame update
        public void Start()
        {

        }
    }
}