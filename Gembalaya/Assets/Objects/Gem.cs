using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Gembalaya.Objects
{
    public class Gem : BaseObject
    {
        [SerializeField]
        private float _rotateSpeed = 0.6f;


        // Update is called once per frame
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _visual.transform.Rotate(Vector3.right * Time.deltaTime * _rotateSpeed);
        }
    }
}