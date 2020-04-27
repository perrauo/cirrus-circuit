using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit.World.Objects.Characters
{
    public class CharacterLibrary : Cirrus.Resources.BaseAssetLibrary<CharacterLibrary>
    {
        [SerializeField]
        public CharacterAsset[] Characters;   
    }

}