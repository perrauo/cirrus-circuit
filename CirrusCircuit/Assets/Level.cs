using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Cirrus.Circuit.Objects;
using System;

namespace Cirrus.Circuit
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        public static int GridSize = 2;

        [SerializeField]
        private Vector3Int _offset = new Vector3Int(2, 2, 2);

        [SerializeField]
        private Vector3Int _dimension = new Vector3Int(20, 20, 20);

        private BaseObject[,,] _objects;


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
        public Gem[] _gems;

        [SerializeField]
        public Door[] _doors;

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
            _objects = new BaseObject[_dimension.x, _dimension.y, _dimension.z];
        }


        public Vector3Int WorldToGrid(Vector3 pos)
        {
            return new Vector3Int(
                Mathf.RoundToInt(pos.x / GridSize) + _offset.x,
                Mathf.RoundToInt(pos.y / GridSize) + _offset.y,
                Mathf.RoundToInt(pos.z / GridSize) + _offset.z);
        }

        public Vector3 GridToWorld(Vector3Int pos)
        {
            return new Vector3Int(
                (pos.x - _offset.x) * GridSize,
                (pos.y - _offset.y) * GridSize,
                (pos.z - _offset.z) * GridSize);
        }

        public bool IsWithinBounds(Vector3Int pos)
        {
            return
                (pos.x < 0 || pos.x > _dimension.x ||
                pos.y < 0 || pos.y > _dimension.y ||
                pos.z < 0 || pos.z > _dimension.z);
        }

        public bool TryGetObject(Vector3Int pos, out BaseObject obj)
        {
            if (pos.x < 0 || pos.x > _dimension.x ||
                pos.y < 0 || pos.y > _dimension.y ||
                pos.z < 0 || pos.z > _dimension.z)
            {
                obj = null;
                return false;
            }

            obj = _objects[pos.x, pos.y, pos.z];
            return obj == null;            
        }

        //public bool TryEnter(BaseObject source, Vector3Int position, out BaseObject destination)
        //{

        //}


        public (Vector3, Vector3Int) RegisterObject(BaseObject obj)
        {
            Vector3Int gridPos = WorldToGrid(obj.transform.position);

            try
            {
                _objects[gridPos.x, gridPos.y, gridPos.z] = obj;
            }
            catch (Exception e)
            {
                Debug.Log("");
            }

            return (GridToWorld(gridPos), gridPos);
        }

        public void OnBeginRound()
        {
            foreach (BaseObject obj in _objects)
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