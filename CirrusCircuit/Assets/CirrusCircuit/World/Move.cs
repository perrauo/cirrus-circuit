
using UnityEngine;
using System.Collections;

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
    public static class MoveUtils
    {
        public static MoveResult ToMoveResult(this NetworkMoveResult netMoveResult)
        {
            ObjectSession sess;
            return new MoveResult
            {
                Move = netMoveResult.Move.ToMove(),

                Destination = netMoveResult.Destination,

                Entered = netMoveResult.Entered == null ?
                    null :
                    netMoveResult.Entered.TryGetComponent(out sess) ?
                        sess._object :
                        null,

                Moved = netMoveResult.Moved == null ?
                    null :
                    netMoveResult.Moved.TryGetComponent(out sess) ?
                        sess._object :
                        null,

                Offset = netMoveResult.Offset,
                Step = netMoveResult.Step,

                State = netMoveResult.State
            };
        }

        public static NetworkMoveResult ToNetworkMoveResult(this MoveResult moveResult)
        {
            return new NetworkMoveResult
            {
                Move = moveResult.Move == null ? null : moveResult.Move.ToNetworkMove(),
                Destination = moveResult.Destination,
                Entered = moveResult.Entered == null ? null : moveResult.Entered._session.gameObject,
                Moved = moveResult.Moved == null ? null : moveResult.Moved._session.gameObject,
                State = moveResult.State,
                
                //MovedResult =
                //    (moveResult != null && moveResult.MovedResult != null) ?
                //    moveResult.MovedResult.ToNetworkMoveResult() : null,

                Offset = moveResult.Offset,
                Step = moveResult.Step
            };
        }


        public static Move ToMove(this NetworkMove netMove)
        {
            ObjectSession sess;
            return new Move
            {
                Position = netMove.Position,
                Step = netMove.Step,
                Source = netMove.Source == null ? null : netMove.Source.TryGetComponent(out sess) ? sess._object : null,
                User = netMove.User == null ? null : netMove.User.TryGetComponent(out sess) ? sess._object : null,
                Entered = netMove.Entered == null ? null : netMove.Entered.TryGetComponent(out sess) ? sess._object : null,
                State = netMove.State
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
                State = move.State

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
                State = result.State,
                Step = result.Step
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
                State = move.State,
                User = move.User
            };
        }
    }

    public class Move
    {
        public BaseObject Source;
        public BaseObject Entered; // Entered object from which we start
        public BaseObject User;
        public Vector3Int Step;
        public Vector3Int Position;
        public BaseObject.State State;
    }

    [Serializable]
    public class NetworkMove
    {
        public GameObject Source;
        public GameObject Entered; // Entered object from which we start
        public GameObject User;
        public Vector3Int Step;
        public Vector3Int Position;
        public BaseObject.State State;
    }

    public class MoveResult
    {
        public Move Move;
        public Vector3 Offset;
        public Vector3Int Destination;
        public Vector3Int Step;
        public BaseObject.State State;
        public BaseObject Moved;
        public BaseObject Entered;
    }

    [Serializable]
    public class NetworkMoveResult
    {
        public NetworkMove Move;
        public Vector3 Offset;
        public Vector3Int Destination;
        public Vector3Int Step;
        public BaseObject.State State;
        public GameObject Moved;
        public GameObject Entered;
    }

}