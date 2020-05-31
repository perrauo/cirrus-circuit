using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cirrus.Circuit.World.Objects.Characters;
using System;
using System.Linq;

namespace Cirrus.Circuit.UI
{    
    //using DD = Dictionary<Tuple<int,int>, CharacterPreview>;
    public class CharacterRosterPreview : BaseSingleton<CharacterRosterPreview>
    {        

        [SerializeField]
        private CharacterPreview _previewTemplate;

        private Dictionary<Tuple<int, int>, CharacterPreview> _characterPlayerPreviews;

        public override void Awake() {
            _characterPlayerPreviews = new Dictionary<Tuple<int, int>, CharacterPreview>();
            _characterPlayerPreviews.Clear();
        }                

        public bool GetCharacterPreview(
            int playerId, 
            int characterId, 
            out CharacterPreview preview)
        {
            return
                _characterPlayerPreviews
                    .TryGetValue(
                        Utils.MakePair(playerId, characterId), 
                        out preview); 
        }
        

        public void AddPlayerPreviews(int slotIndex)
        {
            int i = 0;
            foreach (
                CharacterAsset res
                in CharacterLibrary.Instance.Characters)
            {
                if (res == null) continue;

                CharacterPreview preview = 
                    _previewTemplate.Create(
                        transform,
                        (slotIndex * CharacterLibrary.Instance.Characters.Length) + i);
                
                _characterPlayerPreviews.Add(
                    Utils.MakePair(slotIndex, res.Id),
                    preview);

                i++;
                
            }
        }
    }
}
