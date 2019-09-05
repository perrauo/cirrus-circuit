using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Gembalaya.Objects
{
    public class Solid : BaseObject
    {

        public override bool TryMove(Vector3 step, BaseObject incoming = null)
        {
            switch (incoming.Id)
            {
                default:
                    return false;
            }
        }

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
        }
    }
}