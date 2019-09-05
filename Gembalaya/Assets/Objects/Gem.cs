using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Gembalaya.Objects
{
    public class Gem : BaseObject
    {
        public override ObjectId Id { get { return ObjectId.Gem; } }

        [SerializeField]
        private float _rotateSpeed = 0.6f;

        public Controls.PlayerNumber PlayerNumber;

        private Color _color;

        public Color Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
                _visual.Color = _color;
            }
        }

        // Update is called once per frame
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _visual.transform.Rotate(Vector3.right * Time.deltaTime * _rotateSpeed);
        }

    }
}