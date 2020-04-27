using UnityEngine;
using System.Collections;
using System;

namespace Cirrus.Circuit.World.Objects.Characters
{

    public class CharacterAsset : ScriptableObject
    {
        [SerializeField]
        public Sprite Portrait;

        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("CharacterTemplate")]
        private Character Character;

        public int _id = -1;

        public int Id {

            get
            {
                if(_id < 0) _id = Array.IndexOf(CharacterLibrary.Instance.Characters, this);

                return _id;
            }
        }
        

        public void OnValidate()
        {
            if (_id < 0) _id = Id;
        }
    
        public string Name
        {
            get
            {
                string _name = name.Substring(name.IndexOf('.') + 1);
                _name = _name.Replace('.', ' ');
                return _name;
            }
        }


        public Character Create(Vector3 position, Transform parent)
        {
            return Instantiate(
                Character.gameObject,
                position,
                Quaternion.identity,
                parent)
                .GetComponent<Character>();
        }

        public Character Create(Vector3 position, Transform parent, Quaternion rotation)
        {
            return Instantiate(
                Character.gameObject,
                parent.position,
                rotation,
                parent)
                .GetComponent<Character>();
        }
    }

}