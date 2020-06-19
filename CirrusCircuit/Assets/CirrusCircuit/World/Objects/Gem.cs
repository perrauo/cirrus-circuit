using Cirrus.Circuit.Controls;
using Cirrus.Circuit.Networking;
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

        [Header("----------------------------", order = 0)]
        [Header("Gem", order = 1)]
        [Header("----------------------------", order = 2)]
        [SerializeField]
        [FormerlySerializedAs("Type")]
        public GemType GemType;

        [SerializeField]
        public float _value = 1f;

        [SerializeField]
        private const float RotateSpeed = 0.6f;

        public bool IsRequired = false;

        public override ObjectType Type => ObjectType.Gem;

        public override bool IsSlidable => true;

        public override void Awake()
        {
            base.Awake();
        }

        public virtual void OnMoved(MoveResult result)
        {
            if (!CustomNetworkManager.IsServer) return;

            if (result.Move.User == this) return;

            if (result.Moved == this) return;

            if (!_hasArrived) return;            

            if (
                IsSlidable &&
                _entered != null &&
                _entered is Slope &&
                !((Slope)_entered).IsStaircase)
            {
                Server_Slide();
            }
        }

        public override ReturnType GetEnterResults(
            Move move, 
            out EnterResult result, 
            out IEnumerable<MoveResult> moveResults)
        {
            result = null;
            moveResults = null;
            return ReturnType.Failed;
        }

        // Update is called once per frame
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _visual.Parent.transform.Rotate(Vector3.right * Time.deltaTime * RotateSpeed);
        }
    }
}