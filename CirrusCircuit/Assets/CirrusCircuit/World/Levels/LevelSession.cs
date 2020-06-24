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
using Cirrus.Threading;

namespace Cirrus.Circuit.World
{
    public class LevelSession : CustomNetworkBehaviour
    {
        //public class MoveInfo
        //{
        //    public Vector3Int Position;
        //    public Vector3Int Direction;
        //    public Vector3 Offset;
        //}

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

        public Delegate<Level.Rule> OnLevelCompletedHandler;

        public Door.OnScoreValueAdded OnScoreValueAddedHandler;

        private static LevelSession _instance;

        public Delegate<MoveResult> OnMovedHandler;

        public Mutex _moveMutex = new Mutex();
        //public Mutex _resultLockMutex = new Mutex();
        //public bool _isResultLocked = false;

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
                CommandClient
                    .Instance
                    .Cmd_LevelSession_SetRequiredGemCount(
                        gameObject,
                        _requiredGemCount);
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
                CommandClient
                    .Instance
                    .Cmd_LevelSession_SetRequiredGems(
                        gameObject,
                        _requiredGems);
            }
        }

        #region Unity Engine

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

                if (Settings.AreGemsSpawned.Boolean)
                {
                    _randomDropRainTimer.OnTimeLimitHandler += Cmd_OnRainTimeout;
                }
            }
        }

        #endregion

        // TODO designated area for gem to fall through
        public Vector3Int GetFallPosition(bool isLandingGuaranteed = true)
        {
            const int FallTrials = 1000;

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
                for (
                    int i = 0;
                    i < Level.Dimensions.y;
                    i++)
                {
                    if (Get(
                        position.SetY(position.y - i),
                        out BaseObject target))
                    {
                        //if (target is Solid) continue;
                        if (target is Gem) continue;
                        if (target is Character) continue;
                        if (target is Door) continue;
                        if (target is Portal) continue;
                        if (target is Ladder) continue;

                        return position;
                    }
                }
            }

            Debug.Assert(false);
            return Vector3Int.zero;
        }

        public void OnRoundStarted(int i)
        {
            foreach (var obj in _objects)
            {
                if (obj == null) continue;

                obj.Cmd_FSM_SetState(ObjectState.Idle);
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
                        _portals[portal.Link] = Cirrus.Utils.MakePair(tuple.Item1, portal);
                    }
                    else _portals.Add(portal.Link, Cirrus.Utils.MakePair(portal, portal));

                }

                res.Color = PlayerManager.Instance.GetColor(res.ColorID);

                res.gameObject.SetActive(true);
                (res.Transform.position, res._levelPosition) = RegisterObject(res);
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
                    info.Session._object.ColorID = info.PlayerId;
                    info.Session._object.Color = player.Color;
                    info.Session._object._levelPosition = info.Position;
                    (info.Session._object.Transform.position, info.Session._object._levelPosition) = RegisterObject(info.Session._object);
                    info.Session._object._targetPosition = info.Session._object.Transform.position;

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
            if (_randomDropRainTimer != null) _randomDropRainTimer.OnTimeLimitHandler -= Cmd_OnRainTimeout;
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
                            Position = obj._levelPosition,
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
            Set(obj._levelPosition, null);
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

        #region Exit

        public bool GetExitResult(
            Move move,
            out ExitResult result,
            out IEnumerable<MoveResult> moveResults)
        {
            moveResults = new MoveResult[0];

            result = new ExitResult
            {
                Step = move.Step,
                Destination = move.Position + move.Step,
                Position = move.Position,
                Entered = null,
                Moved = null,
                Offset = Vector3.zero,
                MoveType = move.Type
            };


            if (move.Entered != null)
            {
                return move.Entered.GetExitResult(
                    move,
                    out result,
                    out moveResults
                    );
            }

            return true;
        }

        #endregion

        #region Enter

        public ReturnType GetEnterResults(
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

        #region Apply move result
        public void ApplyMoveResult(MoveResult result)
        {            
            if (
                result.Move.Entered == null &&
                Get(
                    result.Move.Position, 
                    out BaseObject previous))
            {
#if UNITY_EDITOR
                Debug.Assert(previous == result.User);
#endif
                Set(
                        result.Move.Position,
                        null);                
            }

            if (result.Entered == null)
            {
                Set(result.Destination, result.User);
            }
        }


        public void ApplyMoveResults(IEnumerable<MoveResult> results)
        {

            // Moves are immediately applied to the server instead
            foreach (var res in results)
            {
                if (res == null) continue;

                ApplyMoveResult(res);
                res.Move.User.ApplyMoveResult(res);                
            }

            foreach (var res in results)
            {
                if (res == null) continue;

                OnMovedHandler?.Invoke(res);
            }

            // Apply client moves
           Rpc_ApplyMoveResults(results
                .Select(x => x.ToNetworkMoveResult())
                .ToArray());
        }

        [ClientRpc]
        public void Rpc_ApplyMoveResults(NetworkMoveResult[] results)
        {
            if (CustomNetworkManager.IsServer) return;

            foreach (var res in results.Select(x => x.ToMoveResult()))
            {
                if (res == null) continue;

                res.Move.User.ApplyMoveResult(res);
            }
        }

        #endregion

        #region Move

        public bool IsMoveStale(Move move)
        {
            if (move.Position != move.User._levelPosition) return true;

            else if (move.Entered == null)
            {
                int i = VectorUtils.ToIndex(
                    move.Position,
                    Level.Instance.Dimensions.x,
                    Level.Instance.Dimensions.y);

                if (move.User != _objects[i]) return true;
            }
            else if (move.Entered._visitor != move.User) return true;            

            return false;
        }

        private ReturnType RecurseGetPullResults(
            Action src,
            List<MoveResult> results)
        {
            if (src.Target.GetMoveResults(
                new Move
                {
                    User = src.Target,
                    Source = src.Source,
                    Destination = src.Source._levelPosition,
                    Step = -src.Direction,
                    Entered = src.Target._entered,
                    Position = src.Target._levelPosition,
                    Type = MoveType.Moving
                },
                out IEnumerable<MoveResult> moveResults,
                isRecursiveCall: true) > 0)
            {
                results.AddRange(moveResults);

                foreach (var hld in src.Target._holding)
                {
                    if (hld == null) continue;

                    if (hld == src.Source) continue;

                    if (hld._heldAction.Direction == -src.Direction)
                    {
                        results.AddRange(moveResults);
                        RecurseGetPullResults(
                            hld._heldAction,
                            results);
                    }
                    else
                    {
                        hld.ReleaseHold();
                    }
                }

                return ReturnType.Succeeded_Next;
            }

            return ReturnType.Failed;            
        }

        public ReturnType GetPullResults(
            Action src,
            MoveResult result,
            out IEnumerable<MoveResult> results,
            bool isRecursiveCall = false)
        {
            results = new List<MoveResult>();

            if(src == null) return ReturnType.Failed;

            // If moving forward do not pull
            if (src.Direction == result.Move.Step.SetY(0)) return ReturnType.Failed;

            RecurseGetPullResults(
                src,
                (List<MoveResult>)results
                );

            return ((List<MoveResult>)results).Count != 0 ?
                ReturnType.Succeeded_Next :
                ReturnType.Failed;

        }

        // TODO move should even be type, typing is deduced here and sent back
        public ReturnType GetMoveResults(
            Move move,
            out IEnumerable<MoveResult> results,
            bool isRecursiveCall = false,
            // We do not lock the results if we simply query the level
            // without intent to change it
            bool lockResults = true)
        {
            results = new List<MoveResult>();
            IEnumerable<MoveResult> pullResults;
            ReturnType ret = ReturnType.Succeeded_Next;            

            var result = new MoveResult
            {
                MoveType = move.Type,
                Position = move.Position,
                Move = move,
            };

            if (!isRecursiveCall)
            {
                _moveMutex.WaitOne();
            }

            do
            {                
                // GEM Quicksand teleoport leads here:
                // / /TODO dix
                // GEM TELEPORT LEADS HERE/
                /// Check if move is stale
                if (IsMoveStale(move))
                {
                    ret = ReturnType.Failed;
                    result = null;
                    break;
                }

                if (!GetExitResult(
                    move,
                    out ExitResult exitResult,
                    out IEnumerable<MoveResult> exitMoveResults))
                {                    
                    ret = ReturnType.Failed;
                    result = null;
                    break;
                }

                result.Entered = exitResult.Entered;
                result.Destination = move.Position + exitResult.Step;
                result.Position = exitResult.Position;
                result.Offset = exitResult.Offset;
                result.MoveType = exitResult.MoveType;
                result.Direction = exitResult.Step.SetY(0);

                //result.MoveType =
                //    move.User._held != null ?
                //    MoveType.Pulling :
                //    result.MoveType;

                if (move.User._heldAction != null)
                {
                    // If pulling backward
                    if (move.User._heldAction.Direction != result.Move.Step.SetY(0))
                    {
                        result.Direction = -result.Direction;
                    }
                }

                if (result.MoveType == MoveType.Struggle)
                {
                    // Result of children passed but not me
                    ret = ReturnType.Succeeded_End;                    
                    break;
                }
                else if (result.MoveType == MoveType.Teleport)
                {
                    if (Get(move.Destination, out BaseObject _))
                    {
                        ret = ReturnType.Failed;
                        result = null;
                        break;
                    }

                    result.Entered = null;
                    result.Moved = null;
                    result.Position = move.Destination;
                    result.Destination = move.Destination;
                    result.MoveType = MoveType.Teleport;
                    result.Offset = Vector3.zero;
                    break;
                }
                else
                {
                    if (move.Type == MoveType.Falling) result.Direction = move.User._direction;                        

                    if (Get(
                        // Object pushed into
                        result.Destination,
                        out result.Moved))
                    {
                        if (result.Moved == move.User)                           
                        {
                            ret = ReturnType.Failed;
                            result = null;
                            break;
                        }
                        else if (result.Moved == move.Source)
                        {
                            // If pulled, then necessarily the moving slot is the src
                            ret = ReturnType.Succeeded_Next;
                            break;
                        }
                        else if ((
                            ret = result.Moved.GetMoveResults(
                            // Object moved into is movable
                            new Move
                            {
                                Position = result.Destination,
                                Entered = result.Moved._entered,
                                Source = move.User,
                                User = result.Moved,
                                Type = move.Type,
                                Step = exitResult.Step.SetY(0)
                            },
                            out IEnumerable<MoveResult> movedResults,
                            isRecursiveCall: true)) > 0)
                        {                            
                            ((List<MoveResult>)results).AddRange(movedResults);

                            if (ret < ReturnType.Succeeded_Next) result = null;
                            
                            break;
                        }
                        // Object moved into is enterable (or no object)
                        else if ((
                            ret =
                            GetEnterResults(
                            result.Moved,
                            // Current move
                            // VVVVVVVVVVVVVVVVV
                            new Move
                            {
                                Step = exitResult.Step,
                                Position = move.Position,
                                Entered = move.Entered,
                                Source = move.Source,
                                Type = move.Type,
                                User = move.User
                            },
                            out EnterResult enterResult,
                            out IEnumerable<MoveResult> moveResults)) > 0)
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
                            if (ret < ReturnType.Succeeded_Next) result = null;
                            break;
                        }
                        else
                        {
                            ret = ReturnType.Failed;
                            result = null;
                            break;
                        }
                    }
                    // No object moved into
                    else
                    {
                        if (move.Type == MoveType.Falling)
                        {
                            bool fallthrough = !Level.Instance.IsInsideBoundsY(result.Destination);
                            result.Entered = null;
                            result.Moved = null;
                            result.Position = move.Position;
                            result.Destination = fallthrough ? GetFallPosition(true) : result.Destination;
                            result.MoveType = fallthrough ? MoveType.Teleport : MoveType.Falling;
                            break;
                        }
                        else if (
                            Level.IsInsideBounds(
                            move.Position +
                            move.Step +
                            Vector3Int.down))
                        {
                            BaseObject below;

                            if (Get(
                               move.Position +
                               move.Step +
                               Vector3Int.down,
                               out below))
                            {
                                // Handle stepping on a slope
                                if (below is Slope)
                                {
                                    var slopeMove = move.Copy();
                                    slopeMove.Step = move.Step + Vector3Int.down;
                                    if ((ret = GetEnterResults(
                                        below,
                                        slopeMove,
                                        out EnterResult enterResult,
                                        out IEnumerable<MoveResult> downhillMoveResults)) 
                                        > 0)
                                    {
                                        if (ret < ReturnType.Succeeded_Next) result = null;
                                        else
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
                                        }       

                                        ((List<MoveResult>)results).AddRange(downhillMoveResults);
                                        break;
                                    }
                                    else
                                    {
                                        ret = ReturnType.Failed;
                                        result = null;
                                        break;
                                    }
                                }
                                else if (below is Quicksand)
                                {
                                    var quicksandMove = move.Copy();
                                    quicksandMove.Step = move.Step + Vector3Int.down;
                                    if ((ret = GetEnterResults(
                                        below,
                                        quicksandMove,
                                        out EnterResult enterResult,
                                        out IEnumerable<MoveResult> downResults)) > 0)
                                    {
                                        if(ret < ReturnType.Succeeded_Next) result = null;
                                        else
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
                                        }
      
                                        ((List<MoveResult>)results).AddRange(downResults);
                                        break;
                                    }
                                    else
                                    {
                                        ret = ReturnType.Failed;
                                        result = null;
                                        break;
                                    }
                                }
                            }

                            // not slope, do nothing, result is good
                            break;
                        }
                        // else we're simply walking
                    }
                }
            }
            while (false);

            if (!isRecursiveCall)
            {
                _moveMutex.ReleaseMutex();
            }

            if (result != null)
            {
                ((List<MoveResult>)results).Add(result);

                if (
                    result.MoveType == MoveType.Moving ||
                    result.MoveType == MoveType.UsingPortal)
                    //result.MoveType == MoveType
                {
                    //var pullResults = new List<MoveResult>();
                    if (GetPullResults(
                        move.User._heldAction,
                        result,
                        out pullResults,
                        false) > 0)
                    {
                        ((List<MoveResult>)results).AddRange(pullResults);
                    }
                }
            }

            return ret;
        }

        #endregion

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
                if (ObjectLibrary.Instance.Get(spawnId, out Spawnable spawnable))
                {
                    Spawn(
                        session,
                        spawnable,
                        pos);
                }
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
                (obj.Transform.position, obj._levelPosition) = RegisterObject(obj);

                if (Get(
                    obj._levelPosition + Vector3Int.down,
                    out BaseObject _))
                {
                    obj.Cmd_FSM_SetState(ObjectState.Idle);
                }
                else obj.Server_Fall();
            }
        }


        #endregion

        #region On Rain Timeout

        public void Cmd_OnRainTimeout()
        {
            Vector3Int position = GetFallPosition();

            Gem gem = ObjectLibrary.Instance.Gems[
                UnityEngine.Random.Range(
                    0,
                    ObjectLibrary.Instance.Gems.Length)];

            if (gem.TryGetComponent(out Spawnable spawn))
            {
                Rpc_OnRainTimeout(
                    position,
                    spawn.Id);
            }
        }

        [ClientRpc]
        public void Rpc_OnRainTimeout(Vector3Int pos, int objectId)
        {
            Cmd_Spawn(
                objectId,
                pos);
        }

        #endregion
    }
}