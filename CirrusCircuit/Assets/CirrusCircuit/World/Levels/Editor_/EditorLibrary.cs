using UnityEngine;
using System.Collections;
using Cirrus.Circuit.World.Objects;

namespace Cirrus.Circuit.World.Editor
{

    public class EditorLibrary : Resources.BaseAssetLibrary<EditorLibrary>
    {
        [SerializeField]
        public Level[] Levels;

        [SerializeField]
        public Material DimensionsMaterial;

        [SerializeField]
        public Material LayerMaterial;

        [SerializeField]
        public Material CursorMaterial;

        //[SerializeField]
        //private 


    }
}
