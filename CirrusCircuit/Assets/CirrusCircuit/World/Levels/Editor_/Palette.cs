using UnityEngine;
using System.Collections;
using Cirrus.Circuit.World.Objects;
using UnityEditor;
using SubjectNerd.Utilities;

namespace Cirrus.Circuit.World.Editor
{
    [CreateAssetMenu(menuName = "Cirrus Circuit/Editor.Palette")]
    public class Palette : BaseAsset
    {
        [SerializeField]
        [Reorderable]
        public BaseObject[] _tiles;
        public BaseObject[] Tiles => _tiles;
	}
}