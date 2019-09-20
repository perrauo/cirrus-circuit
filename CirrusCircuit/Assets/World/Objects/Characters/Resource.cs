using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit.World.Objects.Characters
{

    public class Resource : ScriptableObject
    {
        [SerializeField]
        public Sprite Portrait;

        [SerializeField]
        private Character CharacterTemplate;

        public Character Create(Vector3 position, Transform parent)
        {
            return Instantiate(
                CharacterTemplate.gameObject,
                position,
                Quaternion.identity,
                parent)
                .GetComponent<Character>();
        }

        public Character Create(Vector3 position, Transform parent, Quaternion rotation)
        {
            return Instantiate(
                CharacterTemplate.gameObject,
                parent.position,
                rotation,
                parent)
                .GetComponent<Character>();
        }
    }

}