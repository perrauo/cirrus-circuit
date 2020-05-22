using Cirrus.Circuit.Controls;
using Cirrus.Circuit.World;
using Cirrus.Circuit.World.Objects;
using Cirrus.Circuit.World.Objects.Characters;
using Cirrus.Events;
using Cirrus.MirrorExt;
using Cirrus.Utils;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Cirrus.Circuit.Networking;
using UnityEditor.Experimental.GraphView;

namespace Cirrus.Circuit.World
{    
    public class LevelSession : CustomNetworkBehaviour
    {
        public class MoveInfo
        {
            public Vector3Int Position;
            public Vector3Int Direction;
            public Vector3 Offset;
        }

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

        private Dictionary<int, Tuple<Portal, Portal>> _portals = new Dictionary<int, Tuple<Portal, Portal>>();

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
            transform.position = Vector3.Lerp(
                transform.position,
                TargetPosition,
                Level._positionSpeed);
        }

        public void Awake()
        {
            _mutex = new Mutex(false);

            Game.Instance.OnRoundInitHandler += OnRoundInit;
            Game.Instance.OnRoundHandler += OnRound;

            if (CustomNetworkManager.IsServer)
            {
                _randomDropRainTimer = new Timer(
                    Level.RandomDropRainTime,
                        start: false,
                        repeat: true);

                if (Game.Instance.IsRainEnabled)
                {
                    _randomDropRainTimer.OnTimeLimitHandler += Cmd_OnRainTimeout;
                }
            }
        }

        public bool GetOtherPortal(Portal portal, out Portal other)
        {
            other = null;

            if (_portals.TryGetValue(
                portal.Connection,
                out Tuple<Portal, Portal> tuple))
            {
                other = portal == tuple.Item1 ? tuple.Item2 : tuple.Item1;
                return true;
            }

            return false;
        }

        public void OnRound()
        {
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

                if (res is Door) ((Door)res).OnScoreValueAddedHandler += OnGemEntered;

                if (res is Portal)
                {
                    var portal = (Portal)res;
                    if (_portals.TryGetValue(portal.Connection, out Tuple<Portal, Portal> tuple))
                    {
                        _portals[portal.Connection] =
                            new Tuple<Portal, Portal>(
                                tuple.Item1,
                                portal);
                    }
                    else _portals.Add(
                        portal.Connection,
                        new Tuple<Portal, Portal>(portal, portal));
                }

                res.Color = PlayerManager.Instance.GetColor(res.ColorId);

                res.gameObject.SetActive(true);
                (res.Transform.position, res._gridPosition) = RegisterObject(res);
            }

            foreach (var plceholderInfo in PlaceholderInfos)
            {
                PlayerSession player = GameSession.Instance.GetPlayer(plceholderInfo.PlayerId);
                Player localPlayer = PlayerManager.Instance.GetPlayer(player.LocalId);
                player.Score = 0;

                plceholderInfo.Session._object =
                    plceholderInfo.Character.Create(
                        Level.GridToWorld(plceholderInfo.Position),
                        transform,
                        plceholderInfo.Rotation);
                plceholderInfo.Session._object._session = plceholderInfo.Session;
                plceholderInfo.Session._object.ColorId = plceholderInfo.PlayerId;
                plceholderInfo.Session._object.Color = player.Color;
                plceholderInfo.Session._object._gridPosition = plceholderInfo.Position;
                (plceholderInfo.Session._object.Transform.position, plceholderInfo.Session._object._gridPosition) = RegisterObject(plceholderInfo.Session._object);

                if (player.ServerId == localPlayer.ServerId)
                    localPlayer._character = (Character)plceholderInfo.Session._object;
            }

            foreach (var session in ObjectSessions)
            {
                if (session.Index >= _objects.Length) continue;

                BaseObject obj = null;
                if ((obj = _objects[session.Index]) != null)
                {
                    obj._session = session;
                    session._object = obj;

                    obj.Disable();
                }
            }
        }

        // TODO multiple object per square
        public override void OnStartClient()
        {
            base.OnStartClient();

        }

        public override void Destroy()
        {
            _randomDropRainTimer.OnTimeLimitHandler -= Cmd_OnRainTimeout;

            foreach (var obj in _objects) if (obj != null) Destroy(obj.gameObject);

            if (CustomNetworkManager.IsServer)
            {
                foreach (var sess in ObjectSessions)
                {
                    if (sess != null)
                    {
                        NetworkServer.Destroy(sess.gameObject);
                        Destroy(sess.gameObject);
                    }
                }

                if (gameObject != null)
                {
                    Game.Instance.OnRoundInitHandler -= OnRoundInit;
                    Game.Instance.OnRoundHandler -= OnRound;

                    NetworkServer.Destroy(gameObject);
                    Destroy(gameObject);
                }
            }

            _instance = null;
        }

        public static LevelSession Create()
        {
            int i = 0;
            LevelSession levelSession = NetworkingLibrary.Instance.LevelSession.Create();
            List<PlaceholderInfo> placeholders = new List<PlaceholderInfo>();

            var objs = GameSession.Instance.SelectedLevel.Objects.Where(x => x != null).FirstOrDefault();

            foreach (
                var obj in
                GameSession.Instance.SelectedLevel.Objects)
            {
                if (obj != null)
                {
                    var objectSession = NetworkingLibrary.Instance.ObjectSession.Create();
                    objectSession.Index = i;
                    levelSession._objectSessions.Add(objectSession.gameObject);

                    NetworkServer.Spawn(
                        objectSession.gameObject,
                        NetworkServer.localConnection);

                    if (obj is Gem)
                    {
                        Gem gem = (Gem)obj;
                        levelSession.RequiredGems += gem.IsRequired ? 1 : 0;
                    }
                    else if (obj is Placeholder)
                    {
                        var info = new PlaceholderInfo()
                        {
                            _session = objectSession.gameObject,
                            Position = obj._gridPosition,
                            Rotation = obj.Transform.rotation
                        };

                        placeholders.Add(info);
                        levelSession._placeholderInfos.Add(info);
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
                    //    CustomNetworkManager.Instance.ServerHandler.GetConnection(
                    //        player._connectionId,
                    //        out ClientPlayer client))
                    //{
                    //    info.Session.netIdentity.AssignClientAuthority(
                    //        client.connectionToClient);
                    //}

                }

                i++;
            }

            levelSession.OnScoreValueAddedHandler +=
                (Gem gem, int player, float value) =>
                {
                    GameSession.Instance.GetPlayer(player).Score += value;
                };

            NetworkServer.Spawn(levelSession.gameObject, NetworkServer.localConnection);
            return levelSession;
        }

        public void Set(
            Vector3Int pos,
            BaseObject obj)
        {
            int i = VectorUtils.ToIndex(
                pos, 
                Level.Dimensions.x, 
                Level.Dimensions.y);

            _objects[i] = obj;

        }

        public (Vector3, Vector3Int) RegisterObject(BaseObject obj)
        {
            Vector3Int pos = Level.WorldToGrid(obj.Transform.position);

            int i = VectorUtils.ToIndex(
                pos,
                Level.Dimensions.x,
                Level.Dimensions.y);

            //Debug.Log("Registered: " + obj);
            _objects[i] = obj;

            return (Level.GridToWorld(pos), pos);
        }


        public void UnregisterObject(BaseObject obj)
        {
            Set(obj._gridPosition, null);
        }


        public bool Get(
            Vector3Int pos,
            out BaseObject obj)
        {
            obj = null;
            if (!Level.IsWithinBounds(pos)) return false;

            _mutex.WaitOne();

            int i = VectorUtils.ToIndex(pos, Level.Dimensions.x, Level.Dimensions.y);
            obj = _objects[i];

            _mutex.ReleaseMutex();
            return obj != null;
        }

        #region Exit

        public bool GetExitResult(
            Move move,
            out MoveResult result)
        {
            result = new MoveResult();

            if (Level.IsWithinBounds(move.Position + move.Step + Vector3Int.up))
            {
                if (Get(
                  move.Position + move.Step + Vector3Int.up,
                  out result.Entered))
                {
                    // Handle stepping on a slope
                    if (result.Entered is Slope)
                    {
                        result.Step = move.Step - Vector3Int.up;
                    }
                }
            }

            return true;
        }

        public void Exit(
            Move move,
            MoveResult result)
        {
            // Only set/free occupying tile if not visiting
            if (move.Source._entered == null) Set(move.Source._gridPosition, null);
            else move.Source._entered._visitor = null;
        }

        public void ApplyResult(MoveResult result)
        {
            if(result.Move.Entered == null) Set(result.Move.Position, null);
            if(result.Entered == null) Set(result.Destination, result.Move.User);
        }


        #endregion

        #region Enter

        public bool GetEnterResult(
            Move move,
            out MoveResult result)            
        {
            result = new MoveResult();

            if (Get(
                move.Position + move.Step, 
                out result.Entered))
            {
                return
                    result.Entered.GetEnterResult(
                        move,
                        out result
                        );
            }

            return true;
        }

        #endregion



        public bool GetMoveResults(
            Move move, 
            out IEnumerable<MoveResult> results)
        {
            results = new List<MoveResult>();
            
            var result = new MoveResult { 
                Move = move,
                Step = move.Step,
                Destination = move.Position + move.Step,
                State = BaseObject.State.Moving
            };

            ((List<MoveResult>)results).Add(result);

            if (!Level.IsWithinBounds(move.Position + move.Step))
            {                
                return false;
            }

            if (!GetExitResult(
                move,
                out MoveResult exitResult))                          
            {
                return false;
            }

            if (Get(
                // Object pushed into
                move.Position + move.Step,
                out result.Moved))
            {
                // Object moved into is movable
                if (result.Moved.GetMoveResults(
                    move, 
                    out IEnumerable<MoveResult> movedResults))                    
                {
                    ((List<MoveResult>)results).AddRange(movedResults);
                    return true;
                }
                // Object moved into is enterable
                else if (GetEnterResult(
                    move, 
                    out MoveResult enterResult))
                {
                    // If moving out of entered object
                    if (enterResult.Entered != result.Moved)
                    {
                        IEnumerable<MoveResult> enteredMovedResults = null;

                        if (
                            enterResult.Moved == null ||
                            enterResult.Moved.GetMoveResults(
                                new Move
                            {
                                Position = enterResult.Destination,
                                Step = enterResult.Step,
                                User = move.User,
                                Source = move.Source
                            },
                            out enteredMovedResults))
                        {
                            if(enterResult.Move != null) ((List<MoveResult>)results).AddRange(enteredMovedResults);
                            return true;
                        }
                    }
                    // If moving in entered object
                    else
                    {
                        result.Moved = null;
                        result.Entered = result.Moved;
                        return true;
                    }                                
                }
            }
            // No object moved into
            else
            {
                if (Level.IsWithinBounds(move.Position + move.Step + Vector3Int.down))
                {

                    if (Get(
                      move.Position + move.Step + Vector3Int.down,
                      out BaseObject slope))
                    {
                        // Handle stepping on a slope
                        if (slope is Slope)
                        {
                            var slopeMove = move.Copy();
                            slopeMove.Step = move.Step + Vector3Int.down;
                            if (GetMoveResults(
                                slopeMove,
                                out IEnumerable<MoveResult> downhillResults))
                            {
                                ((List<MoveResult>)results).AddRange(downhillResults);
                                return true;
                            }
                            else return false;// cant move object in the slope, cant move myself
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public Vector3Int GetFallThroughPosition(bool isLandingGuaranteed = true)
        {
            for (int k = 0; k < FallTrials; k++)
            {
                Vector3Int position = new Vector3Int(

                    UnityEngine.Random.Range(
                        0,
                        Level.Dimensions.x),

                    Level.Dimensions.y - 1,

                    UnityEngine.Random.Range(
                        0,
                        Level.Dimensions.z));

                if (!isLandingGuaranteed) return position;

                // Check for valid surface to fall on
                for (int i = 0; i < Level.Dimensions.y; i++)
                {
                    if (Get(
                        position.Copy().SetY(position.y - i),
                        out BaseObject target))
                    {
                        if (target is Gem) continue;
                        if (target is Character) continue;
                        if (target is Door) continue;
                        if (target is Portal) continue;

                        return position;
                    }
                }
            }

            Debug.Assert(false);
            return Vector3Int.zero;
        }

        public bool FallThrough(
            BaseObject source,
            Vector3Int step,
            ref Vector3 offset,
            out Vector3Int destinationPosition,
            out BaseObject destination)
        {
            destination = null;
            Vector3Int direction = step;//.SetXYZ(step.x, 0, step.z);
            destinationPosition = source._gridPosition + step;

            //bool once = false;

            if (!Level.IsWithinBoundsY(destinationPosition.y))
            {
                destinationPosition = GetFallThroughPosition();

                for (int i = 0; i < Level.Dimensions.y; i++)
                {
                    if (Get(destinationPosition.Copy().SetY(destinationPosition.y - i), out BaseObject target))
                    {
                        if (target is Gem) continue;
                        if (target is Character) continue;
                        if (target is Door) continue;

                        return GetMoveResults(
                            new Move { 
                                User = source,
                                Step = step,
                                Position = source._gridPosition,
                                Source = null
                            },
                            out IEnumerable<MoveResult> result);
                    }
                }

            }

            return false;
        }

        #region Spawn

        public void Cmd_Spawn(Spawnable spawnable, Vector3Int pos)
        {
            Cmd_Spawn(spawnable.Id, pos);
        }

        public void Cmd_Spawn(int spawnId, Vector3Int pos)
        {
            CommandClient.Instance.Cmd_LevelSession_Spawn(
                gameObject,
                spawnId,
                pos);
        }

        [ClientRpc]
        public void Rpc_Spawn(
            GameObject sessionObj,
            int spawnId,
            Vector3Int pos)
        {
            if (sessionObj.TryGetComponent(out ObjectSession session))
            {
                Spawn(
                    session,
                    ObjectLibrary.Instance.Get(spawnId),
                    pos);
            }
        }

        public void Spawn(
            ObjectSession session,
            Spawnable template,
            Vector3Int pos)
        {
            GameObject gobj = template.gameObject.Create(
                Level.GridToWorld(pos),
                transform);

            if (gobj.TryGetComponent(out BaseObject obj))
            {
                session._object = obj;                
                obj._session = session;                
                (obj.Transform.position, obj._gridPosition) = RegisterObject(obj);
                obj.Cmd_Idle();
            }
        }


        #endregion


        #region On Rain Timeout

        public void Cmd_OnRainTimeout()
        {
            Vector3Int position = GetFallThroughPosition();

            Gem gem = ObjectLibrary.Instance.Gems[
                UnityEngine.Random.Range(
                    0,
                    ObjectLibrary.Instance.Gems.Length)];

            if (gem.TryGetComponent(out Spawnable spawn))
            {
                CommandClient.Instance.Cmd_LevelSession_OnRainTimeout(gameObject, position, spawn.Id);
            }
        }

        [ClientRpc]
        public void Rpc_OnRainTimeout(Vector3Int pos, int objectId)
        {
            OnRainTimeout(pos, objectId);
        }

        public void OnRainTimeout(Vector3Int pos, int objectId)
        {
            Cmd_Spawn(objectId, pos);
        }


        #endregion


        public void OnRoundInit()
        {
            RoundSession.Instance.OnRoundStartHandler += OnRoundStarted;
        }

        public void OnRoundStarted(int i)
        {
            foreach (var obj in _objects)
            {
                if (obj == null) continue;

                obj.Cmd_Idle();
            }

            if (CustomNetworkManager.IsServer)
            {
                _randomDropRainTimer.Start();
            }
        }

        public void OnRoundEnd()
        {
            //foreach (BaseObject obj in _objects)
            //{
            //    if (obj == null)
            //        continue;

            //    obj.OnRoundEnd();

            //    obj.SetState(BaseObject.State.Disabled);
            //}

            //foreach (GameObject obj in _objects)
            //{
            //    if (obj == null)
            //        continue;

            //    //obj.OnRoundEnd();

            //    //obj.SetState(BaseObject.State.Disabled);
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

            //    obj.SetState(BaseObject.State.LevelSelect);
            //}
        }
    }
}