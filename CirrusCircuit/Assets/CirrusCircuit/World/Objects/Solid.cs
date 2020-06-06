using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit.World.Objects
{
    public class Solid : BaseObject
    {
        // TODO
        //public override bool IsNetworked => false;

        public override ObjectType Type => ObjectType.Solid;

        public override bool GetMoveResults(
            Move move, 
            out IEnumerable<MoveResult> res,
            bool isRecursiveCall = false,
            bool lockResults = true)
        {
            res = null;
            return false;
        }

        public override bool GetEnterResults(
            Move move,
            out EnterResult result,
            out IEnumerable<MoveResult> results)
        {
            result = new EnterResult();
            results = new MoveResult[0];
            return false;
        }

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
        }

        // TODO remove
        // Replaced by OnMoved
        public override void Cmd_Interact(BaseObject source)
        {
            
        }


        public override void FSM_FixedUpdate()
        {
            
        }

        public override void FSM_Update()
        {

        }


    }
}