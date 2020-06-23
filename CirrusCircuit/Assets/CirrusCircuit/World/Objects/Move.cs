
using Cirrus.Circuit.World.Objects;
using System;
using UnityEngine;

namespace Cirrus.Circuit.World
{
    public enum ReturnType
    {
        Failed = -1,
        Succeeded_End = 1,
        Succeeded_Next = 2,       
    }

    public enum MoveType
    { 
        Unknown,
        Sliding,
        Teleport,
        UsingPortal,
        Falling,
        Moving,
        Direction,
        Climbing,
        Struggle,
        Pulling,

        Disappear,
        Reappear,
    }

    //TODO
    public enum MoveResultType
    {
        Unknown,
        Sliding,
        Teleport,
        UsingPortal,
        Falling,
        Moving,
        Direction,
        Climbing,
        Struggle
    }

    public class Hold
    {
        public BaseObject Source;
        public BaseObject Target;
        public Vector3Int Direction;
    }


    [Serializable]
    public enum ActionType
    {
        Unknown,
        Land,
        Emote0,
        Emote1,
        Emote2,
        Landing,

        Color,
        Idle,
    }

    public class Action
    {
        public ActionType Type;
        public int ColorID;
    }

    public class Move
    {
        public BaseObject Source;
        public BaseObject Entered; // Entered object from which we start
        public BaseObject User;
        public Vector3Int Step;
        public Vector3Int Position;
        public Vector3Int Destination;
        public MoveType Type;
    }

    public class ExitResult
    {
        public Vector3 Offset;
        public Vector3Int Step;
        public BaseObject Entered;
        public BaseObject Moved;
        public Vector3Int Position;
        public Vector3Int Destination;
        public MoveType MoveType = MoveType.Unknown;
    }

    public class EnterResult
    {
        public BaseObject Entered;
        public BaseObject Moved;
        public Vector3 Offset;
        public float PitchAngle = 0;
        public Vector3Int Step;
        public Vector3Int Position;
        public Vector3Int Destination;
        public MoveType MoveType = MoveType.Unknown;
        public float Scale = 1;
    }

    public class MoveResult
    {        
        public Move Move;
        public Vector3 Offset;
        public float PitchAngle = 0;
        public Vector3Int Position;
        public Vector3Int Destination;
        public Vector3Int Direction;
        public MoveType MoveType;
        public BaseObject Moved;
        public BaseObject Entered;        
        public float Scale = 1;

        public BaseObject User => Move.User;
        public BaseObject PreviousEntered => Move.Entered;
        public Vector3Int PreviousPosition => Move.Position;
    }


    [Serializable]
    public class NetworkMove
    {
        public GameObject Source;
        public GameObject Entered; // Entered object from which we start
        public GameObject User;
        public MoveType Type;
        public Vector3Int Step;
        public Vector3Int Position;
        public Vector3Int Destination;
    }

    [Serializable]
    public class NetworkMoveResult
    {
        public NetworkMove Move;
        public Vector3 Offset;
        public float Angle = 0;
        public Vector3Int Position;
        public Vector3Int Destination;
        public Vector3Int Direction;
        public MoveType MoveType;
        public GameObject Moved;
        public GameObject Entered;
        //public GameObject Exited;
        public float Scale = 1;
    }
 

    public static class MoveUtils
    {
        public static bool IsLocking(this MoveType type)
        {
            switch (type)
            {
                case MoveType.Direction:
                case MoveType.Unknown:
                    return false;
                default: return true;
            }
        }

        public static MoveResult ToMoveResult(this NetworkMoveResult net)
        {
            ObjectSession sess;
            return new MoveResult
            {
                Move = net.Move.ToMove(),

                Destination = net.Destination,

                Entered = net.Entered == null ?
                    null :
                    net.Entered.TryGetComponent(out sess) ?
                        sess._object :
                        null,
                //Exited = net.Exited == null ?
                //    null :
                //    net.Exited.TryGetComponent(out sess) ?
                //        sess._object :
                //        null,


                Moved = net.Moved == null ?
                    null :
                    net.Moved.TryGetComponent(out sess) ?
                        sess._object :
                        null,

                Offset = net.Offset,
                PitchAngle = net.Angle,
                Direction = net.Direction,

                //State = net.State,
                MoveType = net.MoveType,
                Position = net.Position,
                Scale = net.Scale

            };
        }


        public static NetworkMoveResult ToNetworkMoveResult(this MoveResult result)
        {
            return new NetworkMoveResult
            {
                Move = result.Move == null ? null : result.Move.ToNetworkMove(),
                Destination = result.Destination,
                Entered = result.Entered == null ? null : result.Entered._session.gameObject,
                //Exited = result.Exited == null ? null : result.Exited._session.gameObject,
                Moved = result.Moved == null ? null : result.Moved._session.gameObject,
                //State = result.State,

                //MovedResult =
                //    (moveResult != null && moveResult.MovedResult != null) ?
                //    moveResult.MovedResult.ToNetworkMoveResult() : null,

                Offset = result.Offset,
                Angle = result.PitchAngle,
                Direction = result.Direction,
                MoveType = result.MoveType,
                Position = result.Position,
                Scale = result.Scale
            };
        }


        public static Move ToMove(this NetworkMove net)
        {
            ObjectSession sess;
            return new Move
            {
                Position = net.Position,
                Step = net.Step,
                Source = net.Source == null ? null : net.Source.TryGetComponent(out sess) ? sess._object : null,
                User = net.User == null ? null : net.User.TryGetComponent(out sess) ? sess._object : null,
                Entered = net.Entered == null ? null : net.Entered.TryGetComponent(out sess) ? sess._object : null,
                Type = net.Type,
                Destination = net.Destination
            };
        }

        public static NetworkMove ToNetworkMove(this Move move)
        {
            return new NetworkMove
            {
                Position = move.Position,
                Destination = move.Destination,
                Step = move.Step,
                Source = move.Source == null ? null : move.Source._session.gameObject,
                User = move.User == null ? null : move.User._session.gameObject,
                Entered = move.Entered == null ? null : move.Entered._session.gameObject,
                Type = move.Type,
            };
        }


        public static MoveResult Copy(this MoveResult result)
        {
            return new MoveResult
            {
                Destination = result.Destination,
                Move = result.Move,
                Entered = result.Entered,
                //Exited = result.Exited,                
                Moved = result.Moved,
                Offset = result.Offset,
                PitchAngle = result.PitchAngle,
                Position = result.Position,
                Direction = result.Direction,
                MoveType = result.MoveType,
                Scale = result.Scale
            };
        }

        public static Move Copy(this Move move)
        {
            return new Move
            {
                Entered = move.Entered,
                Step = move.Step,
                Position = move.Position,
                Source = move.Source,
                Type = move.Type,
                User = move.User,
                Destination = move.Destination
            };
        }
    }

}