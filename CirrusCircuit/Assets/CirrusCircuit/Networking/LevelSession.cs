using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cirrus.Circuit.World.Objects;
using Cirrus.Utils;
using System;
using System.Threading;
using Cirrus.Utils;
using Mirror;

using Cirrus.Circuit.World.Objects;
using Cirrus.Circuit.World;
using Cirrus.Circuit.World.Objects.Characters;
using Cirrus.Events;
using Cirrus.MirrorExt;
using System.Linq;

namespace Cirrus.Circuit.Networking
{
    public class LevelSession : NetworkBehaviour
    {
        private const int FallTrials = 100;

        public Event<Level.Rule> OnLevelCompletedHandler;

        public Door.OnScoreValueAdded OnScoreValueAddedHandler;

        private static LevelSession _instance;

        public static LevelSession Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<LevelSession>();
                return _instance;
            }
        }

        Mutex _mutex;        

        public Vector3 TargetPosition;

        private Timer _randomDropRainTimer;

        private Timer _randomDropSpawnTimer;

        private int _requiredGems = 0;


        [SerializeField]
        public BaseObject[] _objects;
        
        [SyncVar]
        [SerializeField]
        public GameObjectSyncList _objectSessions = new GameObjectSyncList();
        public IEnumerable<ObjectSession> ObjectSessions => _objectSessions.Select(x => x.GetComponent<ObjectSession>());


        public Level Level => GameSession.Instance.SelectedLevel;

        [SyncVar]
        [SerializeField]
        public int _requiredGemCount = 0;

        public int RequiredGemCount
        {
            get => _requiredGemCount;
            set
            {
                _requiredGemCount = value;
                ClientPlayer.Instance.Cmd_LevelSession_SetRequiredGemCount(gameObject, _requiredGemCount);
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            _objects = new BaseObject[Level.Size];

            foreach (var obj in Level.Objects)
            {
                if (obj == null) continue;

                var res = obj.Create(obj.transform.position, transform);
                res.gameObject.SetActive(true);
                RegisterObject(res);
            }

            // TODO multiple object per square
            Debug.Log("SESSION OBJECT COUNT " + _objectSessions.Count);
            foreach (var session in ObjectSessions)
            {
                if (session.Index >= _objects.Length) continue;

                BaseObject obj = null;                
                if ((obj = _objects[session.Index]) != null)
                {
                    obj._session = session;
                    session._object = obj;
                }
            }

        }




        public static LevelSession Create()
        {
            GameObject gobj;
            LevelSession levelSession = null;
            if (ServerUtils.TryCreateNetworkObject(
                NetworkingLibrary.Instance.LevelSession.gameObject,
                out gobj,                
                false))
            {
                if ((levelSession = gobj.GetComponent<LevelSession>()) != null)
                {
                    int i = 0;
                    ObjectSession objectSession;                    
                    foreach (var obj in GameSession.Instance.SelectedLevel.Objects)
                    {                    
                        if (obj != null)
                        {
                            if (ServerUtils.TryCreateNetworkObject(                 
                                NetworkingLibrary.Instance.ObjectSession.gameObject,
                                out gobj,                                 
                                true))
                            {
                                if ((objectSession = gobj.GetComponent<ObjectSession>()) != null)
                                {
                                    objectSession.Index = i;
                                    levelSession._objectSessions.Add(objectSession.gameObject);

                                    NetworkServer.Spawn(gobj);
                                }
                            }
                        }

                        i++;
                    }

                    NetworkServer.Spawn(levelSession.gameObject, NetworkServer.localConnection);
                    return levelSession;
                }


            }

            return null;
        }


        // https://softwareengineering.stackexchange.com/questions/212808/treating-a-1d-data-structure-as-2d-grid
        public void SetObject(Vector3Int pos, BaseObject obj)
        {
            int i = pos.x + Level.Dimension.x * pos.y + Level.Dimension.x * Level.Dimension.y * pos.z;

            //_objects[i] = obj;

        }

        public (Vector3, Vector3Int) RegisterObject(BaseObject obj)
        {
            Vector3Int pos = Level.WorldToGrid(obj.transform.position);

            int i = pos.x + Level.Dimension.x * pos.y + Level.Dimension.x * Level.Dimension.y * pos.z;

            //Debug.Log("Registered: " + obj);
            _objects[i] = obj;

            return (Level.GridToWorld(pos), pos);
        }


        public void UnregisterObject(BaseObject obj)
        {
            SetObject(obj._gridPosition, null);
        }

        public void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, TargetPosition, Level._positionSpeed);
        }

        public void Awake()
        {
            _mutex = new Mutex(false);

            _randomDropRainTimer = new Timer(Level.RandomDropRainTime, start: false, repeat: true);
            _randomDropRainTimer.OnTimeLimitHandler += OnRainTimeout;
            _randomDropSpawnTimer = new Timer(Level.RandomDropSpawnTime, start: false, repeat: false);
            _randomDropSpawnTimer.OnTimeLimitHandler += OnSpawnTimeout;      
        }

        public bool TryGet(Vector3Int pos, out BaseObject obj)
        {
            obj = null;
            if (!Level.IsWithinBounds(pos))
                return false;

            _mutex.WaitOne();

            int i = pos.x + Level.Dimension.x * pos.y + Level.Dimension.x * Level.Dimension.y * pos.z;

            //obj = _objects[i];
            _mutex.ReleaseMutex();
            return obj != null;            
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
                    if (source._destination == null) SetObject(source._gridPosition, null);
                    else source._destination._user = null;

                    SetObject(position, source);
                    return true;

                }
                else if (pushed.TryEnter(direction, ref offset, source))
                {
                    destination = pushed;

                    // Only set occupying tile if not visiting
                    if (source._destination == null) SetObject(source._gridPosition, null);
                    else source._destination._user = null;

                    return true;
                }
            }
            else
            {
                // Only set occupying tile if not visiting
                if (source._destination == null) SetObject(source._gridPosition, null);
                else source._destination._user = null;

                SetObject(position, source);
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

            if (Level.IsWithinBounds(position))
            {
                return InnerTryMove(
                    source, 
                    position, 
                    direction, 
                    ref offset, 
                    out pushed, 
                    out destination);
            }

            return false;
        }
        
        public bool TryFallThrough(
            BaseObject source, 
            Vector3Int step, ref 
            Vector3 offset, 
            out Vector3Int position, 
            out BaseObject destination)
        {
            destination = null;            
            Vector3Int direction = step;//.SetXYZ(step.x, 0, step.z);
            position = source._gridPosition + step;

            //bool once = false;

            if(!Level.IsWithinBoundsY(position.y))
            {
                for (int k = 0; k < FallTrials; k++)
                {
                    position = new Vector3Int(
                        UnityEngine.Random.Range(Level.Offset.x, Level.Dimension.x - Level.Offset.x),
                        Level.Dimension.y - 1,
                        UnityEngine.Random.Range(Level.Offset.x, Level.Dimension.z - Level.Offset.z));

                    for (int i = 0; i < Level.Dimension.y; i++)
                    {                                          
                        //Spawn(_objectResources.DebugObject, position.Copy().SetY(position.y - i));
                     
                        if (TryGet(position.Copy().SetY(position.y - i), out BaseObject target))
                        {
                            if (target is Gem) continue;
                            if (target is Character) continue;
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
                UnityEngine.Random.Range(Level.Offset.x, Level.Dimension.x - Level.Offset.x - 1),
                Level.Dimension.y - 1,
                UnityEngine.Random.Range(Level.Offset.x, Level.Dimension.z - Level.Offset.z - 1));

            Spawn(template, position);
        }


        public BaseObject Spawn(BaseObject template, Vector3Int pos)
        {
            BaseObject obj = template.Create(Level.GridToWorld(pos), transform);

            //obj.Register(this);

            obj.TrySetState(BaseObject.State.Idle);

            return obj;
        }


        public GameObject Spawn(GameObject template, Vector3Int pos)
        {

            GameObject obj = template.Create(Level.GridToWorld(pos), transform);

            return obj;
        }

        public void OnRainTimeout()
        {
            Vector3Int position = new Vector3Int(
                UnityEngine.Random.Range(Level.Offset.x, Level.Dimension.x - Level.Offset.x - 1),
                Level.Dimension.y - 1,
                UnityEngine.Random.Range(Level.Offset.x, Level.Dimension.z - Level.Offset.z - 1));

            Rain(
                ObjectLibrary.Instance.SimpleGems[UnityEngine.Random.Range(0, ObjectLibrary.Instance.SimpleGems.Length)]);
        }


        public void OnBeginRound(int number)
        {
            //foreach (BaseObject obj in _objects)
            //{
            //    if (obj == null) continue;

            //    obj.TrySetState(BaseObject.State.Idle);
            //}

            _randomDropRainTimer.Start();
        }

        public void OnRoundEnd()
        {
            //foreach (BaseObject obj in _objects)
            //{
            //    if (obj == null)
            //        continue;

            //    obj.OnRoundEnd();

            //    obj.TrySetState(BaseObject.State.Disabled);
            //}

            //foreach (GameObject obj in _objects)
            //{
            //    if (obj == null)
            //        continue;

            //    //obj.OnRoundEnd();

            //    //obj.TrySetState(BaseObject.State.Disabled);
            //}

            _randomDropRainTimer.Stop();
        }

        private void OnGemEntered(Gem gem, int player, float value)
        {
            OnScoreValueAddedHandler?.Invoke(gem, player, value);

            if (gem.IsRequired)
            {
                RequiredGemCount++;
                if (RequiredGemCount >= _requiredGems)
                {
                    OnLevelCompletedHandler?.Invoke(Level.Rule.RequiredGemsCollected);
                }
            }
        }
        
        public void OnLevelSelect()
        {
            //foreach (BaseObject obj in _objects)
            //{
            //    if (obj == null) continue;

            //    obj.TrySetState(BaseObject.State.LevelSelect);
            //}
        }
    }
}