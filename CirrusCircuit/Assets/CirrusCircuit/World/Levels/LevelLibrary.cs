using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit.World
{
    //TODO
    public class LevelLibrary : Resources.BaseAssetLibrary<LevelLibrary>
    {
        [SerializeField]
        public Level[] Levels;

    }
}
