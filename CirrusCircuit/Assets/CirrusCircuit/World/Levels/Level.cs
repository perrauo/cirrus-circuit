using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Cirrus.Circuit.World.Objects;
using Cirrus.Utils;
using System;

using System.Threading;
using Cirrus.Utils;
using Cirrus.Circuit.Networking;

namespace Cirrus.Circuit.World
{
    public class Level : MonoBehaviour
    {
        private const int FallTrials = 100;

        public enum Rule
        {
            Timeout,
            RequiredGemsCollected,
        }

        public delegate void OnLevelCompleted(Rule rule);

        public OnLevelCompleted OnLevelCompletedHandler;

        public Door.OnScoreValueAdded OnScoreValueAddedHandler;

        [SerializeField]
        public static int GridSize = 2;

        [SerializeField]
        private Vector3Int _offset = new Vector3Int(2, 2, 2);

        public Vector3Int Offset => _offset;

        [SerializeField]
        private Vector3Int _dimension = new Vector3Int(20, 20, 20);

        public Vector3Int Dimension => _dimension;    
        public int Size => Dimension.x * Dimension.y * Dimension.z;

        public const int MaxX = 25;
        public const int MaxY = 25;
        public const int MaxZ = 25;
        public const int MaxSize = MaxX * MaxY * MaxZ;

        Mutex _mutex;

        [SerializeField]
        private BaseObject[] _objects;

        public IEnumerable<BaseObject> Objects => _objects;

        [SerializeField]
        private string _name;

        public string Name => _name;        

        [SerializeField]
        public float DistanceLevelSelection = 35;

        [SerializeField]
        public float CameraSize = 10;

        public Vector3 TargetPosition;

        [SerializeField]
        public float _positionSpeed = 0.4f;

        [SerializeField]
        private float _randomDropRainTime = 2f;
        public float RandomDropRainTime => _randomDropRainTime;

        [SerializeField]
        private float _randomDropSpawnTime = 2f;
        public float RandomDropSpawnTime => _randomDropSpawnTime;

        public void OnValidate()
        {
            _name = gameObject.name.Substring(gameObject.name.IndexOf('.') + 1);
            _name = _name.Replace('.', ' ');
        }

        public void FixedUpdate()
        {
            transform.position = Vector3.Lerp(
                transform.position, 
                TargetPosition, 
                _positionSpeed);
        }

        public void Awake()
        {
            _mutex = new Mutex(false);
            _objects = new BaseObject[_dimension.x * _dimension.y * _dimension.z];

            //_randomDropRainTimer = new Timer(_randomDropRainTime, start: false, repeat: true);
            //_randomDropRainTimer.OnTimeLimitHandler += OnRainTimeout;

            //_randomDropSpawnTimer = new Timer(_randomDropSpawnTime, start: false, repeat: false);
            //_randomDropSpawnTimer.OnTimeLimitHandler += OnSpawnTimeout;
            
            //Game.Instance.On

            //foreach (Door door in _doors)
            //{
            //    if (door == null)
            //        continue;

            //    door.OnScoreValueAddedHandler += OnGemEntered;

            //    //Game.Instance.Lobby.Controllers[(int)door.PlayerNumber].On
            //}            
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
                (pos.x >= 0 && pos.x < _dimension.x &&
                pos.y >= 0 && pos.y < _dimension.y &&
                pos.z >= 0 && pos.z < _dimension.z);
        }

        public bool IsWithinBoundsX(int pos)
        {
            return pos >= 0 && pos < _dimension.x;
        }

        public bool IsWithinBoundsY(int pos)
        {
            return pos >= 0 && pos < _dimension.y;
        }

        public bool IsWithinBoundsZ(int pos)
        {
            return pos >= 0 && pos < _dimension.z;
        }


        public Vector3Int GetOverflow(Vector3Int pos)
        {
            return _dimension - pos;
        }

        //public bool TryGet(Vector3Int pos, out BaseObject obj)
        //{
        //    obj = null;
        //    if (!IsWithinBounds(pos))
        //        return false;

        //    _mutex.WaitOne();

        //    int i = VectorUtils.ToIndex(pos, Dimension.x, Dimension.y);

        //    obj = _objects[i];
        //    _mutex.ReleaseMutex();
        //    return obj != null;            
        //}

        // https://softwareengineering.stackexchange.com/questions/212808/treating-a-1d-data-structure-as-2d-grid
        public void Set(Vector3Int pos, BaseObject obj)
        {
            _mutex.WaitOne();

            int i = VectorUtils.ToIndex(pos, Dimension.x, Dimension.y);

            _objects[i] = obj;


            _mutex.ReleaseMutex();
        }
    
        private void OnSpawnTimeout()
        {
            // TODO
        }

        public void Rain(BaseObject template)
        {
            Vector3Int position = new Vector3Int(
                UnityEngine.Random.Range(_offset.x, _dimension.x - _offset.x - 1),
                _dimension.y - 1,
                UnityEngine.Random.Range(_offset.x, _dimension.z - _offset.z - 1));

            Spawn(template, position);
        }


        public BaseObject Spawn(BaseObject template, Vector3Int pos)
        {
            BaseObject obj = template.Create(GridToWorld(pos), transform);

            obj.Register(this);

            obj.TrySetState(BaseObject.State.Idle);

            return obj;
        }


        public GameObject Spawn(GameObject template, Vector3Int pos)
        {
            return template.Create(
                GridToWorld(pos), 
                transform);            
        }


        public void OnRainTimeout()
        {
            Vector3Int position = new Vector3Int(
                UnityEngine.Random.Range(_offset.x, _dimension.x - _offset.x - 1),
                _dimension.y - 1,
                UnityEngine.Random.Range(_offset.x, _dimension.z - _offset.z - 1));

            Rain(
                ObjectLibrary.Instance.SimpleGems[
                    UnityEngine.Random.Range(
                        0, 
                        ObjectLibrary.Instance.SimpleGems.Length)]);
        }

        public (Vector3, Vector3Int) RegisterObject(BaseObject obj)
        {
            Vector3Int pos = WorldToGrid(obj.transform.position);

            int i = VectorUtils.ToIndex(pos, Dimension.x, Dimension.y);

            _objects[i] = obj;
            return (GridToWorld(pos), pos);
        }

        public void UnregisterObject(BaseObject obj)
        {
            Set(obj._gridPosition, null);
        }
        
        public void OnLevelSelect()
        {
            foreach (BaseObject obj in _objects)
            {
                if (obj == null)
                    continue;

                obj.TrySetState(BaseObject.State.LevelSelect);
            }
        }
    }
}