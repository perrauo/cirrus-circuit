using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cirrus.Circuit.Levels
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        public Objects.Characters.Character[] _characters;

        public Objects.Characters.Character[] Characters
        {
            get
            {
                return _characters;
            }
        }

        public int CharacterCount
        {
            get
            {
                return _characters.Length;
            }
        }
        
        [SerializeField]
        public GameObject _charactersParent;

        [SerializeField]
        public Objects.Gem[] _gems;

        [SerializeField]
        public GameObject _gemsParent;

        [SerializeField]
        public Objects.Door[] _doors;

        [SerializeField]
        public GameObject _doorsParent;

        public static float BlockSize = 2f;

        public static Level Instance;

        public void Awake()
        { 
            Instance = this;
        }

        public void UpdateColors(int player, Color color)
        {
            _characters[player].Color = color;

            foreach (Objects.Gem gem in _gems)
            {
                if (gem == null)
                    continue;

                if (gem.PlayerNumber == (Controls.PlayerNumber)player)
                {
                    gem.Color = color;
                }
            }


            foreach (Objects.Door door in _doors)
            {
                if (door == null)
                    continue;

                if (door.PlayerNumber == (Controls.PlayerNumber)player)
                {
                    door.Color = color;
                }
            }
        }

        public void OnValidate()
        {
            if (_gemsParent)
            {
                _gems = _gemsParent.GetComponentsInChildren<Objects.Gem>();
                _gemsParent = null;
            }

            if (_doorsParent)
            {
                _doors = _doorsParent.GetComponentsInChildren<Objects.Door>();
                _doorsParent = null;
            }


            if (_charactersParent)
            {
                _characters = _charactersParent.GetComponentsInChildren<Objects.Characters.Character>();
                _charactersParent = null;
            }
        }



    }
}