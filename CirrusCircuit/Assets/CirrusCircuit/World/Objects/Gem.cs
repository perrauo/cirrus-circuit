using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit.World.Objects
{
    public class Gem : BaseObject
    {
        public override ObjectId Id { get { return ObjectId.Gem; } }

        public enum GemType
        {
            Small,
            Large,

        }

        [SerializeField]
        public GemType Type;

        [SerializeField]
        public float Value = 1f;

        [SerializeField]
        private float _rotateSpeed = 0.6f;

        public bool IsRequired = false;

        //public Gem Create(Transform parent, Vector3 position)
        //{
        //    return Instantiate(gameObject, position, Quaternion.identity, parent).GetComponent<Gem>();
        //}

        public override void Interact(BaseObject source)
        {
            base.Interact(source);

            if (ColorId >= 4)
            {
                ColorId = source.ColorId;
                Color = source.Color;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if(ColorId >= 4)
                _visual.MakeUnique();
        }

        // Update is called once per frame
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _visual.Parent.transform.Rotate(Vector3.right * Time.deltaTime * _rotateSpeed);
        }

        public override bool TryEnter(Vector3Int step, ref Vector3 offset, BaseObject incoming = null)
        {
            return false;
        }

    }
}