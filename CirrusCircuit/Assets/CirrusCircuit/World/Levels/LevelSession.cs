using Cirrus.Circuit.Controls;
using Cirrus.Circuit.World;
using Cirrus.Circuit.World.Objects;
using Cirrus.Circuit.World.Objects.Characters;
//using Cirrus.Events;
using Cirrus.MirrorExt;
using Cirrus;
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

        private List<Character> _characters = new List<Character>();
        public IEnumerable<Character> Characters => _characters;


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

        public Delegate<Level.Rule> OnLevelCompletedHandler;

        public Door.OnScoreValueAdded OnScoreValueAddedHandler;

        private static LevelSession _instance;

        public Delegate<MoveResult> OnMovedHandler;

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

        public bool GetOtherPortal(
            Portal portal, 
            out Portal other)
        {
            other = null;

            if (_portals.TryGetValue(
                portal.Link,
                out Tuple<Portal, Portal> tuple))
            {
                other = portal == tuple.Item1 ? tuple.Item2 : tuple.Item1;
                return true;
            }

            return false;
        }

        public void OnRoundInit()
        {
            RoundSession.Instance.OnRoundStartHandler += OnRoundStarted;

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
                else if (res is Gem) OnMovedHandler += ((Gem)res).OnMoved;
                //else if (res is Character) _characters.Add(res as Character);
                else if (res is Portal)
                {
                    var portal = (Portal)res;
                    if (_portals.TryGetValue(
                        portal.Link,
                        out Tuple<Portal, Portal> tuple))
                    {
                        _portals[portal.Link] = Utils.MakePair(tuple.Item1, portal);
                    }
                    else _portals.Add(portal.Link, Utils.MakePair(portal, portal));
                        
                }                

                res.Color = PlayerManager.Instance.GetColor(res.ColorId);

                res.gameObject.SetActive(true);
                (res.Transform.position, res._gridPosition) = RegisterObject(res);
            }

            foreach (var info in PlaceholderInfos)
            {
                PlayerSession player = GameSession.Instance.GetPlayer(info.PlayerId);
                if (PlayerManager.Instance.GetPlayer(
                    player.LocalId, 
                    out Player localPlayer))
                {    
                    player.Score = 0;

                    info.Session._object =
                        info.Character.Create(
                            Level.GridToWorld(info.Position),
                            transform,
                            info.Rotation);
                    _characters.Add((Character)info.Session._object);
                    info.Session._object._session = info.Session;
                    info.Session._object.ColorId = info.PlayerId;
                    info.Session._object.Color = player.Color;
                    info.Session._object._gridPosition = info.Position;
                    (info.Session._object.Transform.position, info.Session._object._gridPosition) = RegisterObject(info.Session._object);

                    if (player.ServerId == localPlayer.ServerId)
                        localPlayer._character = (Character)info.Session._object;
                }
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
            if(_randomDropRainTimer != null) _randomDropRainTimer.OnTimeLimitHandler -= Cmd_OnRainTimeout;
            Game.Instance.OnRoundInitHandler -= OnRoundInit;

            foreach (var obj in _objects)
            {
                if (obj == null) continue;
                if (obj.gameObject == null) continue;
                Destroy(obj.gameObject);
            }

            if (CustomNetworkManager.IsServer)
            {
                foreach (var sess in ObjectSessions)
                {
                    if (sess == null) continue;
                    if (sess.gameObject == null) continue;

                    NetworkServer.Destroy(sess.gameObject);
                    Destroy(sess.gameObject);                    
                }

                NetworkServer.Destroy(gameObject);
                Destroy(gameObject);
            }

            _instance = null;
        }

        public static LevelSession Create()
        {
            int i = 0;
            LevelSession levelSession = NetworkingLibrary.Instance.LevelSession.Create();
            List<PlaceholderInfo> placeholders = new List<PlaceholderInfo>();

            foreach (
                var obj in
                GameSession.Instance.SelectedLevel.Objects)
            {               
                if (obj != null && 
                    obj.IsNetworked)
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
            if (!Level.IsInsideBounds(pos)) return false;

            _mutex.WaitOne();

            int i = VectorUtils.ToIndex(pos, Level.Dimensions.x, Level.Dimensions.y);
            obj = _objects[i];

            _mutex.ReleaseMutex();
            return obj != null;
        }


        public void Move(MoveResult result)
        {
            if (
                result.Move.Entered == null &&
                Get(result.Move.Position, out BaseObject previous) &&
                previous == result.Move.User)
            {
                Set(result.Move.Position, null);
            }

            if (result.Entered == null) Set(result.Destination, result.Move.User);

            OnMovedHandler?.Invoke(result);
        }

        #region Exit

        public bool GetExitResult(
            Move move,
            out ExitResult result,
            out IEnumerable<MoveResult> moveResults)
        {
            moveResults = new MoveResult[0];
            
            result = new ExitResult { Step = move.Step };
            if (move.User._entered != null)
            {
                return move.User._entered.GetExitResult(
                    move,
                    out result,
                    out moveResults
                    );                                                                    
            }          

            return true;
        }

        #endregion

        #region Enter

        public bool GetEnterResults(
            BaseObject entered,
            Move move,
            out EnterResult enterResult,
            out IEnumerable<MoveResult> moveResults)            
        {
            return entered.GetEnterResults(
                move,
                out enterResult,
                out moveResults
                );
        }

        #endregion

        public bool GetMoveResults(
            Move move, 
            out IEnumerable<MoveResult> results)
        {
            results = new List<MoveResult>();
            
            var result = new MoveResult { 
                MoveType = move.Type,
                Position = move.Position,
                Move = move,                                
            };

            if (!GetExitResult(
                move,
                out ExitResult exitResult,
                out IEnumerable<MoveResult> exitMoveResults))                          
            {
                return false;
            }

            result.Direction = move.Type == MoveType.Falling ? 
                move.User._direction : 
                exitResult.Step.SetY(0);

            result.Destination = move.Position + exitResult.Step;
            result.Position = exitResult.Position;
            result.Offset = exitResult.Offset;

            if (!Level.IsInsideBounds(result.Destination)) return false;

            if (Get(
                // Object pushed into
                move.Position + exitResult.Step,
                out result.Moved))
            {
                // Object moved into is movable
                if (result.Moved.GetMoveResults(
                    new Move
                    {
                        Position = result.Moved._gridPosition,
                        Entered = result.Moved._entered,
                        Source = move.User,
                        User = result.Moved,
                        Type = move.Type,
                        Step = exitResult.Step.SetY(0)
                    },
                    out IEnumerable<MoveResult> movedResults))
                {
                    ((List<MoveResult>)results).AddRange(movedResults);
                }
                // Object moved into is enterable (or no object)
                else if (GetEnterResults(
                    result.Moved,
                    new Move {
                        Step = exitResult.Step,
                        Position = move.Position,
                        Entered = move.Entered,
                        Source = move.Source,
                        Type = move.Type,
                        User = move.User                      
                    },
                    out EnterResult enterResult,
                    out IEnumerable<MoveResult> moveResults))
                {
                    result.Moved = enterResult.Moved;
                    result.Entered = enterResult.Entered;
                    result.Position = enterResult.Position;
                    result.Scale = enterResult.Scale;
                    result.Direction = 
                        enterResult.MoveType == MoveType.Climbing ?
                            result.Direction : 
                            enterResult.Step.SetY(0);                    
                    result.Destination = enterResult.Destination;
                    result.Offset = enterResult.Offset;
                    result.PitchAngle = enterResult.PitchAngle;
                    result.MoveType = enterResult.MoveType;
                    //result.MoveType = enterResult.MoveType;

                    ((List<MoveResult>)results).AddRange(moveResults);
                }
                else result = null;
            }
            // No object moved into
            else if (Level.IsInsideBounds(
                move.Position + 
                move.Step + 
                Vector3Int.down))
            {
                if (Get(
                    move.Position + 
                    move.Step + 
                    Vector3Int.down,
                    out BaseObject slope))
                {                        
                    // Handle stepping on a slope
                    if (slope is Slope)
                    {
                        var slopeMove = move.Copy();
                        slopeMove.Step = move.Step + Vector3Int.down;
                        if (GetEnterResults(
                            slope,
                            slopeMove,
                            out EnterResult enterResult,
                            out IEnumerable<MoveResult> downhillMoveResults))
                        {
                            result.Moved = enterResult.Moved;
                            result.Entered = enterResult.Entered;
                            result.Direction = enterResult.Step.SetY(0);
                            result.Destination = enterResult.Destination;
                            result.Offset = enterResult.Offset;
                            result.PitchAngle = enterResult.PitchAngle;
                            result.MoveType = enterResult.MoveType;
                            result.Position = enterResult.Position;
                            result.Scale = enterResult.Scale;

                            ((List<MoveResult>)results).AddRange(downhillMoveResults);
                        }
                        else result = null;// cant move object in the slope, cant move myself
                    }
                }                
            }

            // Add action result
            if(result != null) ((List<MoveResult>)results).Add(result);
            return result != null;
        }

        public Vector3Int GetFallThroughPosition(
            bool isLandingGuaranteed = true)
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
                        position.SetY(position.y - i),
                        out BaseObject target))
                    {                            
                        if (target is Gem) continue;
                        if (target is Character) continue;
                        if (target is Door) continue;
                        if (target is Portal) continue;
                        if (target is Slope) continue;

                        return position;
                    }
                }
            }

            Debug.Assert(false);
            return Vector3Int.zero;
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