using UnityEngine;
using System.Collections;
using System;

namespace Cirrus.Circuit.World.Objects.Characters
{


    public class CharacterAsset : ScriptableObject
    {
        public float Preview_OffsetZ = 2.5f;
        public float Preview_OffsetY = -0.5f;
        public float Preview_FOV = 60f;
        public bool Preview_IsLookAtEnabled = false;
        public float Preview_PitchAngle = -10f;
        public float Preview_YawAngle = -10f;

        [SerializeField]
        private string _name;
        public string Name => _name;            

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

        public Character Create(Vector3 position, Transform parent)
        {
            return Instantiate(
                Character.gameObject,
                position,
                Quaternion.identity,
                parent)
                .GetComponent<Character>();
        }


        public Character Create(Transform parent, Quaternion rotation)
        {
            return Instantiate(
                Character.gameObject,
                parent.position,
                rotation,
                parent)
                .GetComponent<Character>();
        }


        public Character Create(Vector3 position, Transform parent, Quaternion rotation)
        {
            return Instantiate(
                Character.gameObject,
                position,
                rotation,
                parent)
                .GetComponent<Character>();
        }
    }

}