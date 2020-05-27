using UnityEngine;
using System.Collections;
using Cirrus.Circuit.World.Objects;
using UnityEngine.Serialization;

namespace Cirrus.Circuit.Editor
{
    public class EditorLibrary : Resources.BaseAssetLibrary<EditorLibrary>
    {
        [SerializeField]
        public Material DimensionsMaterial;

        [SerializeField]
        public Material LayerMaterial;

        [SerializeField]
        [FormerlySerializedAs("CursorMaterial")]
        public Material CursorDefaultMaterial;

        [SerializeField]
        public Material CursorSelectedMaterial;

        [SerializeField]
        public Palette[] Palettes;

    }
}
