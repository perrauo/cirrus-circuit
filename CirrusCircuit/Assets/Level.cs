using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cirrus.Circuit
{
    public class Level : MonoBehaviour
    {
        public static float BlockSize = 2f;

        [SerializeField]
        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
        }

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
        public Objects.Gem[] _gems;

        [SerializeField]
        public Objects.Door[] _doors;

        [SerializeField]
        public Objects.BaseObject[] _objects;

        [SerializeField]
        public float DistanceLevelSelection = 35;

        [SerializeField]
        public float CameraSize = 10;

        public Vector3 TargetPosition;

        [SerializeField]
        public float _positionSpeed = 0.4f;
 
        public void Awake()
        {

        }

        public void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, TargetPosition, _positionSpeed);
        }


        public void OnBeginRound()
        {
            foreach (Objects.BaseObject obj in _objects)
            {
                if (obj == null)
                    continue;

                obj.TryChangeState(Objects.FSM.State.Idle);
            }
        }



        public void UpdateColors(int player, Color color)
        {
            if (_characters[player] == null)
                return;

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
            _name = gameObject.name.Substring(gameObject.name.IndexOf('.')+1);
            _name = _name.Replace('.', ' ');

            if(_gems.Length == 0)
                _gems = gameObject.GetComponentsInChildren<Objects.Gem>();

            if (_doors.Length == 0)
                _doors = gameObject.GetComponentsInChildren<Objects.Door>();

            if(_characters.Length == 0)
                _characters = gameObject.GetComponentsInChildren<Objects.Characters.Character>();

            if (_objects.Length == 0)
                _objects = gameObject.GetComponentsInChildren<Objects.BaseObject>();


            for (int i = 0; i < _characters.Length; i++)
            {
                if (_characters[i] != null && _characters[i].gameObject.activeInHierarchy)
                {
                    if (_characters[i])
                        _characters[i].PlayerNumber = (Controls.PlayerNumber)i;

                    UpdateColors(i, Game.Instance.Lobby.Colors[i]);
                }
            }

        }



    }
}