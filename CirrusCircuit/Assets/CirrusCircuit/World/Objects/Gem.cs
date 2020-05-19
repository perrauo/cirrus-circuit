using Cirrus.Circuit.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cirrus.Circuit.World.Objects
{
    public enum GemType
    {
        Small,
        Large,
    }


    public class Gem : BaseObject
    {
        public override ObjectType Type => ObjectType.Gem;

        [SerializeField]
        [FormerlySerializedAs("Type")]
        public GemType GemType;

        [SerializeField]
        public float _value = 1f;

        [SerializeField]
        private const float RotateSpeed = 0.6f;

        public bool IsRequired = false;

        //public Gem Create(Transform parent, Vector3 position)
        //{
        //    return Instantiate(gameObject, position, Quaternion.identity, parent).GetComponent<Gem>();
        //}


        protected override void Awake()
        {
            base.Awake();
        }

        // Update is called once per frame
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _visual.Parent.transform.Rotate(Vector3.right * Time.deltaTime * RotateSpeed);
        }

        public override bool Enter(
            BaseObject source,
            Vector3Int step)
        {
            //out Vector3 offset,
            //out Vector3Int gridDest,
            //out Vector3Int stepDest,
            //out BaseObject dest)
            {
                //offset = Vector3.zero;
                //gridDest = Vector3Int.zero;
                //stepDest = step;
                //dest = this;

                return false;
            }

        }
    }
}