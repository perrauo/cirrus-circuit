using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit.World
{
    public class LevelLibrary : Cirrus.Resources.BaseAssetLibrary<LevelLibrary>
    {
        [SerializeField]
        public Level[] Levels;

    }
}
