using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cirrus.GemCircuit.Levels
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
        
        [SerializeField]
        public GameObject _charactersParent;

        [SerializeField]
        public Objects.Gem[] _gems;

        [SerializeField]
        public GameObject _gemsParent;

        public static float CubeSize = 2f;

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
        }

        public void OnValidate()
        {
            if (_gemsParent)
            {
                _gems = _gemsParent.GetComponentsInChildren<Objects.Gem>();
                _gemsParent = null;
            }

            if (_charactersParent)
            {
                _characters = _charactersParent.GetComponentsInChildren<Objects.Characters.Character>();
                _charactersParent = null;
            }
        }



    }
}