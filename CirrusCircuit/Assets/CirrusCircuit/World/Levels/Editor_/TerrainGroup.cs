using UnityEngine;
using System.Collections;
using Cirrus.Circuit.World.Objects;
using System;

namespace Cirrus.Circuit.World.Editor
{
    public enum TerrainGroupTilingMode
    { 
        _9 = 9,
        _48 = 48,
        _200 = 200
    }

    [Serializable]
    public class TerrainRuleLayer
    {
        [SerializeField]
        public TerrainGroupTilingMode _tiling = TerrainGroupTilingMode._9;
        public int TerrainGroupTiling => (int)_tiling;

        [Header("Top to bottom, Left to right")]
        [SerializeField]
        public BaseObject[] Tiles;

    }

    [CreateAssetMenu(menuName = "Cirrus Circuit/Editor.TerrainGroup")]
    public class TerrainGroup : BaseAsset
    {
        public const int MaxLayers = 3;

        [SerializeField]
        public BaseObject Default;

        [SerializeField]
        public TerrainRuleLayer[] Layers;

        public override void OnValidate()
        {
            base.OnValidate();

            if (Layers.Length == 0) return;

            if (Layers.Length > 3) Array.Resize(ref Layers, MaxLayers);

            foreach (var layer in Layers)
            {
                if (layer == null) continue;
                if (layer.Tiles.Length != layer.TerrainGroupTiling) 
                    Array.Resize(
                        ref layer.Tiles, 
                        layer.TerrainGroupTiling);
            }

        }
    }
}