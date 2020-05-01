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
using Cirrus.Circuit.Controls;

namespace Cirrus.Circuit.Networking
{
    public class LevelSession : NetworkBehaviour
    {
        [Serializable]
        public class PlaceholderInfo
        {
            public GameObject _session = null;
            public ObjectSession Session => _session == null ? null : _session.GetComponent<ObjectSession>();
            public int PlayerId = -1;
            public int _characterId = -1;
            public CharacterAsset Character => CharacterLibrary.Instance.Characters[_characterId];
            public Vector3Int Position;
            public Quaternion Rotation;
        }

        [Serializable]
        public class PlaceholderInfoSyncList : SyncList<PlaceholderInfo> { }

        [SerializeField]
        public BaseObject[] _objects;

        [SyncVar]
        [SerializeField]
        public GameObjectSyncList _objectSessions = new GameObjectSyncList();
        public IEnumerable<ObjectSession> ObjectSessions => _objectSessions.Select(x => x.GetComponent<ObjectSession>());

        [SyncVar]
        [SerializeField]
        public PlaceholderInfoSyncList _placeholderInfos = new PlaceholderInfoSyncList();
        public IEnumerable<PlaceholderInfo> PlaceholderInfos => _placeholderInfos;

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
                CommandClient.Instance.Cmd_LevelSession_SetRequiredGemCount(gameObject, _requiredGemCount);
            }
        }


        [SyncVar]
        [SerializeField]
        public int _requiredGems = 0;
        public int RequiredGems
        {
            get => _requiredGems;
            set
            {
                _requiredGems = value;
                CommandClient.Instance.Cmd_LevelSession_SetRequiredGems(gameObject, _requiredGems);
            }
        }

        public void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, TargetPosition, Level._positionSpeed);
        }

        public void Awake()
        {
            _mutex = new Mutex(false);

            if (CustomNetworkManager.IsServer)
            {
                _randomDropRainTimer = new Timer(
                    Level.RandomDropRainTime,
                    start: false,
                    repeat: true);

                _randomDropRainTimer.OnTimeLimitHandler += Cmd_OnRainTimeout;
            }
        }

        // TODO multiple object per square
        public override void OnStartClient()
        {
            base.OnStartClient();

            _objects = new BaseObject[Level.Size];

            List<Placeholder> placeholders = new List<Placeholder>();

            foreach (var obj in Level.Objects)
            {
                if (obj == null) continue;

                if (obj is Placeholder) continue;

                var res = obj.Create(
                    obj.Transform.position,
                    obj.Transform.rotation,
                    transform
                    );

                if (res is Door)
                {
                    var door = (Door)res;
                    door.OnScoreValueAddedHandler += OnGemEntered;
                }

                res._levelSession = this;
                res._level = Level;
                res.Color = PlayerManager.Instance.GetColor(res.ColorId);

                res.gameObject.SetActive(true);
                (res.Transform.position, res._gridPosition) = RegisterObject(res);
            }
            
            foreach (var info in PlaceholderInfos)
            {                
                PlayerSession player = GameSession.Instance.GetPlayer(info.PlayerId);
                Player localPlayer = PlayerManager.Instance.GetPlayer(player.LocalId);
                player.Score = 0;

                info.Session._object = 
                    info.Character.Create(
                        Level.GridToWorld(info.Position),
                        transform,
                        info.Rotation);                
                info.Session._object._session = info.Session;
                info.Session._object._levelSession = this;
                info.Session._object._level = Level;
                info.Session._object.ColorId = info.PlayerId;
                info.Session._object.Color = player.Color;
                info.Session._object._gridPosition = info.Position;
                RegisterObject(info.Session._object);

                if (player.ServerId == localPlayer.ServerId)
                    localPlayer._character = (Character)info.Session._object;
            }

            foreach (var session in ObjectSessions)
            {
                if (session.Index >= _objects.Length) continue;

                BaseObject obj = null;
                if ((obj = _objects[session.Index]) != null)
                {
                    obj._session = session;
                    session._object = obj;

                    obj.TrySetState(BaseObject.State.Idle);
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
                    List<PlaceholderInfo> placeholders = new List<PlaceholderInfo>();

                    foreach (var obj in GameSession.Instance.SelectedLevel.Objects)
                    {
                        if (obj != null)
                        {

                            if (ServerUtils.TryCreateNetworkObject(
                                NetworkingLibrary.Instance.ObjectSession.gameObject,
                                out gobj,                                
                                false))
                            {
                                if ((objectSession = gobj.GetComponent<ObjectSession>()) != null)
                                {
                                    objectSession.Index = i;
                                    levelSession._objectSessions.Add(objectSession.gameObject);

                                    NetworkServer.Spawn(gobj, NetworkServer.localConnection);

                                    if (obj is Gem)
                                    {
                                        Gem gem = (Gem)obj;
                                        levelSession.RequiredGems += gem.IsRequired ? 1 : 0;
                                    }
                                    else if (obj is Placeholder)
                                    {
                                        var info = new PlaceholderInfo()
                                        {
                                            _session = gobj,
                                            Position = obj._gridPosition,
                                            Rotation = obj.Transform.rotation
                                        };

                                        placeholders.Add(info);
                                        levelSession._placeholderInfos.Add(info);
                                    }
                                }
                            }
                        }

                        i++;
                    }

                    i = 0;
                    while (!placeholders.IsEmpty())
                    {
                        PlayerSession player = null;
                        if ((player = GameSession.Instance.GetPlayer(i)) != null)
                        {
                            PlaceholderInfo info = placeholders.RemoveRandom();
                            info.PlayerId = player.ServerId;
                            info._characterId = player.CharacterId;

                            //if (
                            //    CustomNetworkManager.Instance.ServerHandler.TryGetConnection(
                            //        player._connectionId,
                            //        out ClientPlayer client))
                            //{
                            //    info.Session.netIdentity.AssignClientAuthority(
                            //        client.connectionToClient);
                            //}

                        }

                        i++;
                    }


                    NetworkServer.Spawn(levelSession.gameObject, NetworkServer.localConnection);
                    return levelSession;
                }


            }

            return null;
        }

        private bool DoIsMoveAllowed(
            BaseObject source,
            Vector3Int position,
            Vector3Int direction)
        {
            BaseObject pushed = null;

            if (TryGet(position, out pushed))
            {
                if (pushed.IsMoveAllowed(direction, source)) return true;

                else if (pushed.IsEnterAllowed(direction, source)) return true;
            }
            else return true;

            return false;
        }

        public bool IsMoveAllowed(
            BaseObject source,
            Vector3Int step)
        {
            Vector3Int direction = step;//.SetXYZ(step.x, 0, step.z);

            Vector3Int position = source._gridPosition + step;

            if (Level.IsWithinBounds(position))
                return DoIsMoveAllowed(source, position, direction);

            return false;
        }

        // https://softwareengineering.stackexchange.com/questions/212808/treating-a-1d-data-structure-as-2d-grid
        public void SetObject(Vector3Int pos, BaseObject obj)
        {
            int i = VectorUtils.ToIndex(pos, Level.Dimension.x, Level.Dimension.y);

            _objects[i] = obj;

        }

        public (Vector3, Vector3Int) RegisterObject(BaseObject obj)
        {
            Vector3Int pos = Level.WorldToGrid(obj.transform.position);

            int i = VectorUtils.ToIndex(pos, Level.Dimension.x, Level.Dimension.y);

            //Debug.Log("Registered: " + obj);
            _objects[i] = obj;

            return (Level.GridToWorld(pos), pos);
        }


        public void UnregisterObject(BaseObject obj)
        {
            SetObject(obj._gridPosition, null);
        }


        public bool TryGet(Vector3Int pos, out BaseObject obj)
        {
            obj = null;
            if (!Level.IsWithinBounds(pos)) return false;

            _mutex.WaitOne();

            int i = VectorUtils.ToIndex(pos, Level.Dimension.x, Level.Dimension.y);
            obj = _objects[i];

            _mutex.ReleaseMutex();
            return obj != null;
        }


        private bool DoTryMove(BaseObject source, Vector3Int position, Vector3Int direction, ref Vector3 offset, out BaseObject pushed, out BaseObject destination)
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
            Vector3Int direction = step;
            position = source._gridPosition + step;

            if (Level.IsWithinBounds(position))
            {
                return DoTryMove(
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

            if (!Level.IsWithinBoundsY(position.y))
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

                            return DoTryMove(source, position, direction, ref offset, out BaseObject pushed, out destination);
                        }
                    }
                }
            }

            return false;
        }        
        
        public void Cmd_Spawn(Spawnable spawnable, Vector3Int pos)
        {
            CommandClient.Instance.Cmd_LevelSession_Spawn(
                gameObject, 
                spawnable.Id, pos);
        }

        public void Cmd_Spawn(int spawnId, Vector3Int pos)
        {
            CommandClient.Instance.Cmd_LevelSession_Spawn(
                gameObject,
                spawnId, pos);
        }

        [ClientRpc]
        public void Rpc_Spawn(GameObject sessionObj, int spawnId, Vector3Int pos)
        {
            ObjectSession session = null;

            if ((session = sessionObj.GetComponent<ObjectSession>()) != null)
            {
                _Spawn(
                    session,
                    ObjectLibrary.Instance.Get(spawnId),
                    pos);
            }
        }

        public void _Spawn(ObjectSession session, Spawnable template, Vector3Int pos)
        {
            GameObject gobj = template.gameObject.Create(
                Level.GridToWorld(pos), 
                transform);

            if(gobj.TryGetComponent(out BaseObject obj))            
            {
                session._object = obj;
                obj._session = session;
                obj._levelSession = this;
                (obj.Transform.position, obj._gridPosition) = RegisterObject(obj);
                obj.TrySetState(BaseObject.State.Idle);
            }            
        }



        public void Cmd_OnRainTimeout()
        {
            Vector3Int position = new Vector3Int(
                UnityEngine.Random.Range(Level.Offset.x, Level.Dimension.x - Level.Offset.x - 1),
                Level.Dimension.y - 1,
                UnityEngine.Random.Range(Level.Offset.x, Level.Dimension.z - Level.Offset.z - 1));

            Gem gem = ObjectLibrary.Instance.Gems[UnityEngine.Random.Range(0, ObjectLibrary.Instance.Gems.Length)];

            if (gem.TryGetComponent(out Spawnable spawn))
            {
                CommandClient.Instance.Cmd_LevelSession_OnRainTimeout(gameObject, position, spawn.Id);
            }
        }

        [ClientRpc]
        public void Rpc_OnRainTimeout(Vector3Int pos, int objectId)
        {
            _OnRainTimeout(pos, objectId);
        }

        public void _OnRainTimeout(Vector3Int pos, int objectId)
        {            
            Cmd_Spawn(objectId, pos);        
        }

        public void OnRoundStarted(int id)
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