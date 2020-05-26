using UnityEngine;
using System.Collections;
using Cirrus.Circuit.World.Objects;
using System;
using Cirrus.Circuit.World;

namespace Cirrus.Circuit.Editor
{
    public enum TerrainDrawMode
    {
        Normal,
        Checkered,
        Dithered,
        Noise,
    }

    [CreateAssetMenu(menuName = "Cirrus Circuit/Editor.TerrainRule")]
    public class TerrainRule : BaseAsset//, IEditorTile
    {
        public bool IsAvailableInEditor => true;

        public const int MaxGroups = 10;
        
        [SerializeField]
        private TerrainDrawMode _terrainDrawMode;
        public TerrainDrawMode TerrainDrawMode => _terrainDrawMode;

        [SerializeField]
        public int _primaryGroupIndex = 0;
        [SerializeField]
        public int _secondaryGroupIndex = 1;

        //
        [SerializeField]
        public TerrainGroup[] _groups;

        public GameObject GetPreview(
            Level level, 
            Vector3Int position)
        {
            Debug.Log("Autotile terrain rules are not finished, and does not work.");
            return null;
        }

        public void Draw(
            Level level, 
            Vector3Int position)
        {

        }
    }
}
