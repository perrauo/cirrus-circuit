using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Cirrus.Circuit.World.Objects;
using Cirrus.Utils;
using System;

using System.Threading;
using Cirrus.Utils;

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

        [SerializeField]
        private Vector3Int _dimension = new Vector3Int(20, 20, 20);

        Mutex _mutex;

        [SerializeField]
        private BaseObject[] _objects;

        [SerializeField]
        private string _name;

        public string Name => _name;        

        [SerializeField]
        public Objects.Characters.Character[] _characters;

        public Objects.Characters.Character[] Characters => _characters;

        public int CharacterCount => _characters.Length;        

        [SerializeField]
        public Gem[] _gems;

        [SerializeField]
        public Door[] _doors;

        [SerializeField]
        public Objects.Characters.Placeholder[] _characterPlaceholders;

        [SerializeField]
        public float DistanceLevelSelection = 35;

        [SerializeField]
        public float CameraSize = 10;

        public Vector3 TargetPosition;

        [SerializeField]
        public float _positionSpeed = 0.4f;

        private Timer _randomDropRainTimer;

        [SerializeField]
        private float _randomDropRainTime = 2f;

        private Timer _randomDropSpawnTimer;

        [SerializeField]
        private float _randomDropSpawnTime = 2f;

        public void OnValidate()
        {
            _name = gameObject.name.Substring(gameObject.name.IndexOf('.') + 1);
            _name = _name.Replace('.', ' ');

            if (_gems != null && _gems.Length == 0)
                _gems = gameObject.GetComponentsInChildren<Gem>();

            if (_doors != null && _doors.Length == 0)
                _doors = gameObject.GetComponentsInChildren<Door>();

            if (_characters != null && _characters.Length == 0)
                _characters = gameObject.GetComponentsInChildren<Objects.Characters.Character>();

            if (_characterPlaceholders != null && _characterPlaceholders.Length == 0)
                _characterPlaceholders = gameObject.GetComponentsInChildren<Objects.Characters.Placeholder>();

        }

        public void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, TargetPosition, _positionSpeed);
        }

        public void Awake()
        {
            _mutex = new Mutex(false);
            _objects = new BaseObject[_dimension.x * _dimension.y * _dimension.z];


            _randomDropRainTimer = new Timer(_randomDropRainTime, start: false, repeat: true);
            _randomDropRainTimer.OnTimeLimitHandler += OnRainTimeout;


            _randomDropSpawnTimer = new Timer(_randomDropSpawnTime, start: false, repeat: false);
            _randomDropSpawnTimer.OnTimeLimitHandler += OnSpawnTimeout;


            Game.Instance.OnNewRoundHandler += OnNewRound;
            //Game.Instance.On

            foreach (Door door in _doors)
            {
                if (door == null)
                    continue;

                door.OnScoreValueAddedHandler += OnGemEntered;

                //Game.Instance.Lobby.Controllers[(int)door.PlayerNumber].On
            }            
        }

        private int _requiredGems = 0;

        private int _requiredGemCount = 0;

        public void OnNewRound(Round round)
        {
            if (this == null)
                return;

            if(!gameObject.activeInHierarchy)
                return;

            round.OnRoundBeginHandler += OnBeginRound;
            round.OnRoundEndHandler += OnRoundEnd;

            foreach (BaseObject obj in _objects)
            {
                if (obj == null)
                    continue;

                obj.TryChangeState(BaseObject.State.Disabled);

                if (obj is Gem)
                {
                    Gem gem = obj as Gem;
                    _requiredGems += gem.IsRequired ? 1 : 0;
                }

                if (obj is Objects.Characters.Character)
                    continue;

                foreach (Controls.Player players in Game.Instance._localPlayers)
                {
                    //if (obj.ColorId == players._colorId)
                    //{
                    //    obj.ColorId = players.ServerId;
                    //    obj.Color = players.Color;
                    //    break;
                    //}
                }
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

        public bool TryGet(Vector3Int pos, out BaseObject obj)
        {
            obj = null;
            if (!IsWithinBounds(pos))
                return false;

            _mutex.WaitOne();

            int i = pos.x + _dimension.x * pos.y + _dimension.x * _dimension.y * pos.z;

            obj = _objects[i];
            _mutex.ReleaseMutex();
            return obj != null;            
        }

        // https://softwareengineering.stackexchange.com/questions/212808/treating-a-1d-data-structure-as-2d-grid
        public void Set(Vector3Int pos, BaseObject obj)
        {
            _mutex.WaitOne();

            int i = pos.x + _dimension.x * pos.y + _dimension.x * _dimension.y * pos.z;

            _objects[i] = obj;


            _mutex.ReleaseMutex();
        }


        private bool InnerTryMove(BaseObject source, Vector3Int position, Vector3Int direction, ref Vector3 offset, out BaseObject pushed, out BaseObject destination)
        {
            pushed = null;
            destination = null;

            if (TryGet(position, out pushed))
            {
                if (pushed.TryMove(direction, source))
                {
                    // Only set occupying tile if not visiting
                    // Only set occupying tile if not visiting
                    if (source._destination == null) Set(source._gridPosition, null);
                    else source._destination._user = null;

                    Set(position, source);
                    return true;

                }
                else if (pushed.TryEnter(direction, ref offset, source))
                {
                    destination = pushed;

                    // Only set occupying tile if not visiting
                    if (source._destination == null) Set(source._gridPosition, null);
                    else source._destination._user = null;

                    return true;
                }
            }
            else
            {
                // Only set occupying tile if not visiting
                if (source._destination == null) Set(source._gridPosition, null);
                else source._destination._user = null;

                Set(position, source);
                return true;
            }

            return false;
        }

        public bool TryMove(
            BaseObject source, 
            Vector3Int step, 
            ref Vector3 offset,
            out Vector3Int position, 
            out BaseObject pushed, 
            out BaseObject destination)
        {
            destination = null;
            pushed = null;

            Vector3Int direction = step;//.SetXYZ(step.x, 0, step.z);

            position = source._gridPosition + step;

            if (IsWithinBounds(position))
            {
                return InnerTryMove(source, position, direction, ref offset, out pushed, out destination);
            }

            return false;
        }

        bool once = false;
        public bool TryFallThrough(BaseObject source, Vector3Int step, ref Vector3 offset, out Vector3Int position, out BaseObject destination)
        {
            destination = null;            
            Vector3Int direction = step;//.SetXYZ(step.x, 0, step.z);
            position = source._gridPosition + step;

            //bool once = false;

            if(!IsWithinBoundsY(position.y))
            {
                for (int k = 0; k < FallTrials; k++)
                {
                    position = new Vector3Int(
                        UnityEngine.Random.Range(_offset.x, _dimension.x - _offset.x),
                        _dimension.y - 1,
                        UnityEngine.Random.Range(_offset.x, _dimension.z - _offset.z));

                    for (int i = 0; i < _dimension.y; i++)
                    {                                          
                        //Spawn(_objectResources.DebugObject, position.Copy().SetY(position.y - i));
                     
                        if (TryGet(position.Copy().SetY(position.y - i), out BaseObject target))
                        {
                            if (target is Gem) continue;
                            if (target is Objects.Characters.Character) continue;
                            if (target is Door) continue;

                            return InnerTryMove(source, position, direction, ref offset, out BaseObject pushed, out destination);
                        }
                    }                    
                }           
            }

            return false;
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

            obj.TryChangeState(BaseObject.State.Idle);

            return obj;
        }


        public GameObject Spawn(GameObject template, Vector3Int pos)
        {

            GameObject obj = template.Create(GridToWorld(pos), transform);

            return obj;
        }


        public void OnRainTimeout()
        {
            Vector3Int position = new Vector3Int(
                UnityEngine.Random.Range(_offset.x, _dimension.x - _offset.x - 1),
                _dimension.y - 1,
                UnityEngine.Random.Range(_offset.x, _dimension.z - _offset.z - 1));

            Rain(
                ObjectLibrary.Instance.SimpleGems[UnityEngine.Random.Range(0, ObjectLibrary.Instance.SimpleGems.Length)]);
        }

        public (Vector3, Vector3Int) RegisterObject(BaseObject obj)
        {
            Vector3Int pos = WorldToGrid(obj.transform.position);

            int i = pos.x + _dimension.x * pos.y + _dimension.x * _dimension.y * pos.z;

            _objects[i] = obj;
            return (GridToWorld(pos), pos);
        }

        public void UnregisterObject(BaseObject obj)
        {
            Set(obj._gridPosition, null);
        }


        public void OnBeginRound(int number)
        {
            foreach (BaseObject obj in _objects)
            {
                if (obj == null) continue;

                obj.TryChangeState(BaseObject.State.Idle);
            }

            _randomDropRainTimer.Start();
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

            _randomDropRainTimer.Stop();
        }

        private void OnGemEntered(Gem gem, int player, float value)
        {

            OnScoreValueAddedHandler?.Invoke(gem, player, value);

            if (gem.IsRequired)
            {
                _requiredGemCount++;
                if (_requiredGemCount >= _requiredGems)
                {
                    OnLevelCompletedHandler?.Invoke(Rule.RequiredGemsCollected);
                }
            }
        }
        
        public void OnLevelSelect()
        {
            foreach (BaseObject obj in _objects)
            {
                if (obj == null)
                    continue;

                obj.TryChangeState(BaseObject.State.LevelSelect);
            }
        }
    }
}