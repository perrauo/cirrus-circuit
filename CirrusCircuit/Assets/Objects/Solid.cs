using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit.Objects
{
    public class Solid : BaseObject
    {

        public override bool TryMove(Vector3Int step, BaseObject incoming = null)
        {
            switch (incoming.Id)
            {
                default:
                    return false;
            }
        }

        public override bool TryFall(BaseObject incoming = null)
        {
            return false;
        }

        public override bool TryEnter(Vector3Int step, ref Vector3 offset, BaseObject incoming = null)
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