using Cirrus.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit.World.Objects
{
    public enum SlopeType
    {
        Staircase,
        Ramp,
    }


    public class Slope : BaseObject
    {
        public override ObjectType Type => ObjectType.Slope;
        public bool _isStaircase = false;
        public bool IsStaircase => _isStaircase;

        public override bool Move(
            BaseObject source, 
            Vector3Int step)
        {
            switch (source.Type)
            {
                default:
                    return false;
            }
        }

        public override bool GetExitValue(
            BaseObject source,
            Vector3Int step,
            out Vector3 offset,
            out Vector3Int gridDest, 
            out Vector3Int stepDest, 
            out BaseObject dest)
        {
            offset = Vector3.zero;
            gridDest = Vector3Int.zero;
            stepDest = Vector3Int.zero;
            dest = null;

            // Same direction (Look up)
            //if (step.Copy().SetY(0) == _destination._direction)
            //{
            //    gridOffset = Vector3Int.up;
            //}
            //// Opposing direction (look down)
            //else if (step.Copy().SetY(0) == -_destination._direction)
            //{
            //    gridOffset = -Vector3Int.up;
            //}

            //if (_levelSession.Move(
            //    this,
            //    step + gridOffset,
            //    out Vector3 offset,
            //    out Vector3Int gridDest,
            //    out BaseObject moved,
            //    out BaseObject destination))
            //{
            //    if (moved) moved.Cmd_Interact(this);
            //    _destination = destination;
            //    _gridPosition = gridDest;
            //    _targetPosition = _level.GridToWorld(_gridPosition);
            //    _targetPosition += offset;
            //    _direction = step;

            //    InitState(State.SlopeMoving, source);
            //}
            return true;
        }


        public override void Enter(
            BaseObject source,
            Vector3Int step,
            Vector3Int gridDest)
        {
            //offset = Vector3.zero;
            //stepDest = step;
            //gridDest = source._gridPosition;
            //dest = this;

            //// Moving up
            //if (step.y >= 0)
            //{
            //    if (step.Copy().SetY(0) == _direction)
            //    {
            //        if (base.Enter(
            //            source,
            //            step,
            //            out offset,
            //            out gridDest,
            //            out stepDest,
            //            out dest))
            //        {
            //            _visitor = source;
            //            offset += Vector3.up * Level.CellSize / 2;
            //            return true;
            //        }
            //    }
            //}
            //// Moving down
            //// Mus be going in opposite direction
            //else if (step.Copy().SetY(0) == -_direction)
            //{
            //    if (base.Enter(
            //        source,
            //        step,
            //        out offset,
            //        out gridDest,
            //        out stepDest,
            //        out dest))
            //    {
            //        _visitor = source;
            //        offset += Vector3.up * Level.CellSize / 2;
            //        return true;
            //    }
            //}
        }







        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
        }

        public override void FSM_FixedUpdate()
        {

        }

        public override void FSM_Update()
        {

        }

        public override void Accept(BaseObject source)
        {
            //source.RampIdle();
        }
    }
}