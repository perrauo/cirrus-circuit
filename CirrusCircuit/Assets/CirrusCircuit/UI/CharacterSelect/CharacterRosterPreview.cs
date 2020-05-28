using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cirrus.Circuit.World.Objects.Characters;

namespace Cirrus.Circuit.UI
{
    public class CharacterRosterPreview : BaseSingleton<CharacterRosterPreview>
    {
        [SerializeField]
        private List<CharacterPreview> _previews;

        [SerializeField]
        private CharacterPreview _previewTemplate;

        public Dictionary<int, RenderTexture> _characterTextures = new Dictionary<int, RenderTexture>();
        public bool TryGetCharacterTexture(int id, out RenderTexture texture)
        {
            return _characterTextures
                .TryGetValue(
                    id,
                    out texture);
        }

        public override void Awake()
        {
            int index = 0;
            foreach (
                CharacterAsset res
                in CharacterLibrary.Instance.Characters)
            {
                if (res == null) continue;
                if (_previewTemplate.Create(
                        transform,
                        index, 
                        out CharacterPreview preview)) _previews.Add(preview);

                _characterTextures.Add(
                    CharacterLibrary.Instance.Characters[index].Id,
                    UILibrary.Instance.CharacterRenderTextures[index]
                    );

                index++;
            }            

        }
    }
}
