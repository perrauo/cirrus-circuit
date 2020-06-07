using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cirrus.Circuit.Networking;

namespace Cirrus.Circuit.World.Objects
{
    // Goop, Sink Liquid

    public class Quicksand : BaseObject
    {
        [SerializeField]
        private float _sinkTime = 2f;

        [SerializeField]
        private bool _isStruggleAllowed = true;

        [SerializeField]
        private int _numStruggleRequired = 3;

        private int _struggleCount = 0;

        [SerializeField]
        public float _struggleTimeLimit = 0.5f;

        public override ObjectType Type => ObjectType.Quicksand;

        public const float BaseOffset = .4f * Level.CellSize;
       
        public override bool IsSolid => false;

        private Timer _timer;

        public bool _doesDestroyObjects = false;

        // struggling out

        public override bool GetMoveResults(
            Move move,
            out IEnumerable<MoveResult> result,
            bool isRecursiveCall = false,
            bool lockResults = true)
        {
            result = null;
            return false;
        }

        public override void Awake()
        {
            base.Awake();

            _timer = new Timer(_sinkTime, start: false, repeat: false);
            
            if(CustomNetworkManager.IsServer) _timer.OnTimeLimitHandler += OnTimeout;
    }

        public override bool GetExitResult(
            Move move,
            out ExitResult exitResult,
            out IEnumerable<MoveResult> moveResults)
        {
            moveResults = new MoveResult[0];
            if (base.GetExitResult(
                move,
                out exitResult,
                out moveResults
                ))
            {
                exitResult.Offset = Vector3.zero;

                if (_struggleCount + 1 == _numStruggleRequired)
                {
                    exitResult.Step = move.Step + Vector3Int.up;
                    exitResult.MoveType = MoveType.Moving;
                    exitResult.Offset = Vector3.zero;
                }
                else
                {
                    exitResult.Step = Vector3Int.zero;
                    exitResult.MoveType = MoveType.Struggle;
                    exitResult.Entered = this;
                    exitResult.Destination = _levelPosition;
                    exitResult.Moved = null;                    
                }


                return true;
            }

            return false;
        }

        public override bool GetEnterResults(
            Move move,
            out EnterResult enterResult,
            out IEnumerable<MoveResult> moveResults
            )
        {
            if (base.GetEnterResults(
                move,
                out enterResult,
                out moveResults
                ))
            {
                return true;
            }

            return false;
        }

        public override void ReenterVisitor()
        {
            base.ReenterVisitor();

            _struggleCount++;
        }



        public void OnTimeout()
        { 
             // TODO
        
        }

        public override void EnterVisitor(BaseObject visitor)
        {
            base.EnterVisitor(visitor);
            _struggleCount = 0;
            _timer.Start(_sinkTime);
        }

                

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_visitor != null)
            {
                _visitor._offset = 
                    BaseOffset * Vector3.up -
                    (_timer.Time / _sinkTime) * (Level.CellSize) * Vector3.up;

            }
        }


    }

}
