using Cirrus.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit
{

    public class UILibrary : BaseAssetLibrary<UILibrary>
    {
        [SerializeField]
        public RenderTexture[] CharacterRenderTextures;
    }
}