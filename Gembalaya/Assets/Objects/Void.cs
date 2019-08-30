using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Gembalaya.Objects
{
    public class Void : BaseObject
    {
        public override bool TryMove(Vector3 step, Status stat = null, BaseObject incoming = null)
        {
            return false;
        }


        protected override void Awake()
        {
            base.Awake();

            _visual.gameObject.SetActive(false);
        }
    }
}
