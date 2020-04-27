using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit.World.Objects
{
    public class ObjectLibrary : Cirrus.Resources.BaseAssetLibrary<ObjectLibrary>
    {
        [SerializeField]
        public Gem[] SimpleGems;

        [SerializeField]
        public GameObject DebugObject;

    }
}