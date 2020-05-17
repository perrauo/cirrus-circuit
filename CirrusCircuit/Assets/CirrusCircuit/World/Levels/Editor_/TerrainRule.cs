using UnityEngine;
using System.Collections;
using Cirrus.Circuit.World.Objects;

namespace Cirrus.Circuit.World.Editor
{
    public class TerrainRule : BaseAsset
    {
        ///
        [SerializeField]
        public BaseObject _topLeft;

        [SerializeField]
        public BaseObject _topCenter;

        [SerializeField]
        public BaseObject _topRight;

        //
        [SerializeField]
        public BaseObject _midLeft;

        [SerializeField]
        public BaseObject _midCenter;

        [SerializeField]
        public BaseObject _midRight;

        // 
        [SerializeField]
        public BaseObject _bottomLeft;

        [SerializeField]
        public BaseObject _bottomCenter;

        [SerializeField]
        public BaseObject _bottomRight;

    }
}
