using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Cirrus.Circuit.World.Objects;
using System;

using System.Threading;

namespace Cirrus.Circuit.World
{
    public class Level : MonoBehaviour
    {
        public Door.OnScoreValueAdded OnScoreValueAdded;

        [SerializeField]
        private Game _game;

        [SerializeField]
        public static int GridSize = 2;

        [SerializeField]
        private Vector3Int _offset = new Vector3Int(2, 2, 2);

        [SerializeField]
        private Vector3Int _dimension = new Vector3Int(20, 20, 20);

        Mutex _mutex;

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
        public Character[] _characters;

        public Character[] Characters
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

        private bool init = false;

        public void OnEnable()
        {
            if (!init)
            {
                _mutex = new Mutex(false);
                _objects = new BaseObject[_dimension.x, _dimension.y, _dimension.z];

                foreach (Door door in _doors)
                {
                    if (door == null)
                        continue;

                    door.OnScoreValueAddedHandler += OnGemEntered;

                    //Game.Instance.Lobby.Controllers[(int)door.PlayerNumber].On
                }

                init = true;
            }
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
                (pos.x > 0 && pos.x < _dimension.x &&
                pos.y > 0 && pos.y < _dimension.y &&
                pos.z > 0 && pos.z < _dimension.z);
        }

        public bool TryGet(Vector3Int pos, out BaseObject obj)
        {
            _mutex.WaitOne();
            obj = _objects[pos.x, pos.y, pos.z];
            _mutex.ReleaseMutex();
            return obj != null;            
        }

        public void Set(Vector3Int position, BaseObject obj)
        {
            _mutex.WaitOne();
            _objects[position.x, position.y, position.z] = obj;
            _mutex.ReleaseMutex();
        }


        public bool TryMove(BaseObject source, Vector3Int step, ref Vector3 offset, out BaseObject destination)
        {
            destination = null;

            Vector3Int direction = step;
            direction.y = 0;

            if (IsWithinBounds(source._gridPosition + step))
            {
                if (TryGet(source._gridPosition + step, out destination))
                {
                    if (destination.TryMove(direction, source))
                    {
                        destination = null;

                        // Only set occupying tile if not visiting
                        // Only set occupying tile if not visiting
                        if (source._destination == null)
                        {
                            Set(source._gridPosition, null);
                        }
                        else
                        {
                            source._destination._user = null;
                        }

                        Set(source._gridPosition + step, source);
                        return true;

                    }
                    else if (destination.TryEnter(direction, ref offset, source))
                    {
                        // Only set occupying tile if not visiting
                        if (source._destination == null)
                        {
                            Set(source._gridPosition, null);
                        }
                        else
                        {
                            source._destination._user = null;
                        }

                        return true;
                    }
                }
                else
                {
                    // Only set occupying tile if not visiting
                    // Only set occupying tile if not visiting
                    if (source._destination == null)
                    {
                        Set(source._gridPosition, null);
                    }
                    else
                    {
                        source._destination._user = null;
                    }

                    Set(source._gridPosition + step, source);
                    return true;
                }
            }

            return false;
        }

        public (Vector3, Vector3Int) RegisterObject(BaseObject obj)
        {
            Vector3Int gridPos = WorldToGrid(obj.transform.position);
            _objects[gridPos.x, gridPos.y, gridPos.z] = obj;
            return (GridToWorld(gridPos), gridPos);
        }

        public void OnRound()
        {
            foreach (BaseObject obj in _objects)
            {
                if (obj == null)
                    continue;

                obj.OnRound();

                //OnRoun obj.OnRoundEnd;
            }
        }

        public void OnBeginRound(int number)
        {
            foreach (BaseObject obj in _objects)
            {
                if (obj == null)
                    continue;

                obj.OnRoundBegin();

                obj.TryChangeState(BaseObject.State.Idle);
            }
        }

        public void OnRoundEnd()
        {
            foreach (BaseObject obj in _objects)
            {
                if (obj == null)
                    continue;

                obj.OnRoundEnd();

                obj.TryChangeState(BaseObject.State.Disabled);
            }
        }

        private void OnGemEntered(Controls.PlayerNumber player, float value)
        {
            OnScoreValueAdded?.Invoke(player, value);
        }
        
        public void OnValidate()
        {
            if (_game == null)
                _game = FindObjectOfType<Game>();

            _name = gameObject.name.Substring(gameObject.name.IndexOf('.')+1);
            _name = _name.Replace('.', ' ');

            if(_gems != null && _gems.Length == 0)
                _gems = gameObject.GetComponentsInChildren<Objects.Gem>();

            if (_doors != null && _doors.Length == 0)
                _doors = gameObject.GetComponentsInChildren<Objects.Door>();

            if(_characters != null && _characters.Length == 0)
                _characters = gameObject.GetComponentsInChildren<Objects.Character>();

        }



    }
}