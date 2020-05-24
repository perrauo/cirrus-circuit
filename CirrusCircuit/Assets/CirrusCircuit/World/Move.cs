
using Cirrus.Circuit.World.Objects;
using System;
using UnityEngine;

namespace Cirrus.Circuit.World
{
    public static class MoveUtils
    {
        public static MoveResult ToActionResult(this NetworkMoveResult net)
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

                Moved = net.Moved == null ?
                    null :
                    net.Moved.TryGetComponent(out sess) ?
                        sess._object :
                        null,

                Offset = net.Offset,
                PitchAngle = net.Angle,
                Direction = net.Direction,

                //State = net.State,
                MoveType =  net.MoveType,                
                
            };
        }

        public static NetworkMoveResult ToNetworkActionResult(this MoveResult result)
        {
            return new NetworkMoveResult
            {
                Move = result.Move == null ? null : result.Move.ToNetworkMove(),
                Destination = result.Destination,
                Entered = result.Entered == null ? null : result.Entered._session.gameObject,
                Moved = result.Moved == null ? null : result.Moved._session.gameObject,
                //State = result.State,
                
                //MovedResult =
                //    (moveResult != null && moveResult.MovedResult != null) ?
                //    moveResult.MovedResult.ToNetworkMoveResult() : null,

                Offset = result.Offset,
                Angle = result.PitchAngle,
                Direction = result.Direction,
                MoveType =  result.MoveType,                
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
            };
        }

        public static NetworkMove ToNetworkMove(this Move move)
        {
            return new NetworkMove
            {
                Position = move.Position,
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
                Moved = result.Moved,
                Offset = result.Offset,
                PitchAngle = result.PitchAngle,
                Direction = result.Direction,
                MoveType = result.MoveType
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
                User = move.User
            };
        }
    }

    public enum MoveType
    { 
        Unknown,
        Sliding,
        Teleport,
        Falling,
        Moving,
        Direction
    }

    public class Move
    {
        public BaseObject Source;
        public BaseObject Entered; // Entered object from which we start
        public BaseObject User;
        public Vector3Int Step;        
        public Vector3Int Position;
        public MoveType Type;
    }

    public class ExitResult
    {
        public Vector3 Offset;
        public Vector3Int Step;        
    }

    public class EnterResult
    {
        public BaseObject Entered;
        public BaseObject Moved;
        public Vector3 Offset;
        public float PitchAngle = 0;
        public Vector3Int Step;
        public Vector3Int Destination;
        //public MoveType MoveType;
    }

    public class MoveResult
    {        
        public Move Move;
        public Vector3 Offset;
        public float PitchAngle = 0;
        public Vector3Int Destination;
        public Vector3Int Direction;
        public MoveType MoveType;
        public BaseObject Moved;
        public BaseObject Entered;
    }


    #region Network

    [Serializable]
    public class NetworkMove
    {
        public GameObject Source;
        public GameObject Entered; // Entered object from which we start
        public GameObject User;
        public MoveType Type;
        public Vector3Int Step;
        public Vector3Int Position;
    }

    [Serializable]
    public class NetworkMoveResult
    {
        public NetworkMove Move;
        public MoveType MoveType;
        public Vector3 Offset;
        public float Angle = 0;
        public Vector3Int Destination;
        public Vector3Int Direction;        
        public GameObject Moved;
        public GameObject Entered;
    }

    #endregion

}