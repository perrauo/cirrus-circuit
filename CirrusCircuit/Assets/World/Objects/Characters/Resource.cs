using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit.World.Objects.Characters
{

    public class Resource : ScriptableObject
    {
        [SerializeField]
        public Sprite Portrait;

        [SerializeField]
        private GameObject CharacterTemplate;
    
    }

}