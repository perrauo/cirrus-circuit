using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Cirrus.Circuit.Objects;

namespace Cirrus.Circuit
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        public static int BlockSize = 2;

        [SerializeField]
        private Vector3Int Dimension = new Vector3Int(10, 10, 10);

        private BaseObject[,,] Objects;


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
        public float DistanceLevelSelection = 35;

        [SerializeField]
        public float CameraSize = 10;

        public Vector3 TargetPosition;

        [SerializeField]
        public float _positionSpeed = 0.4f;
 
        public void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, TargetPosition, _positionSpeed);
        }

        public void Awake()
        {
            Objects = new BaseObject[Dimension.x, Dimension.y, Dimension.z];
        }


        public Vector3Int WorldToGrid(Vector3 pos)
        {
            return new Vector3Int(
                Mathf.RoundToInt(pos.x / BlockSize),
                Mathf.RoundToInt(pos.y / BlockSize),
                Mathf.RoundToInt(pos.z / BlockSize));
        }

        public Vector3 GridToWorld(Vector3Int pos)
        {
            return new Vector3Int(
                pos.x * BlockSize,
                pos.y * BlockSize,
                pos.z * BlockSize);
        }

        //public bool TryMove(BaseObject source, Vector3Int step, out BaseObject destination)
        //{
        //    Vector3Int pos = source.GridPosition + step;

        //    destination = Objects[pos.x, pos.y, pos.z];
        //    if (destination == null)
        //    {
        //        return true;
        //    }
        //    else if (TryMove(destination, step, position))
        //    {
                
        //    }

        //}

        //public bool TryEnter(BaseObject source, Vector3Int position, out BaseObject destination)
        //{

        //}


        public (Vector3, Vector3Int) RegisterObject(BaseObject obj)
        {
            Vector3Int gridPos = WorldToGrid(obj.transform.position);
            Objects[gridPos.x, gridPos.y, gridPos.z] = obj;
            return (GridToWorld(gridPos), gridPos);
        }

        public void OnBeginRound()
        {
            foreach (BaseObject obj in Objects)
            {
                if (obj == null)
                    continue;

                obj.TryChangeState(BaseObject.State.Idle);
            }
        }



        public void UpdateColors(int player, Color color)
        {
            if (_characters[player] == null)
                return;

            _characters[player].Color = color;

            foreach (Gem gem in _gems)
            {
                if (gem == null)
                    continue;

                if (gem.PlayerNumber == (Controls.PlayerNumber)player)
                {
                    gem.Color = color;
                }
            }

            foreach (Door door in _doors)
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

            if(_gems != null && _gems.Length == 0)
                _gems = gameObject.GetComponentsInChildren<Objects.Gem>();

            if (_doors != null && _doors.Length == 0)
                _doors = gameObject.GetComponentsInChildren<Objects.Door>();

            if(_characters != null && _characters.Length == 0)
                _characters = gameObject.GetComponentsInChildren<Objects.Characters.Character>();

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