using Cirrus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit.World.Objects
{
    // NOTE:
    // Potential infinite loop whehn gem exit from the door

    public class Portal : BaseObject
    {
        // TODO
        // When coming out of the portal make it so that successive keypress in same direction
        // moves the character forward even if not the same direction
        [Header("----------------------------", order = 0)]
        [Header("Portal", order = 1)]
        [Header("----------------------------", order = 2)]
        [SerializeField]
        private Number _link;
        public int Link => (int)_link;


        public override ObjectType Type => ObjectType.Portal;
        
        private const float PunchScaleAmount = 1f;

        private const float PunchScaleTime = 1f;

        public const float EnterScale = 0.5f;

        IEnumerator PunchScaleCoroutine()
        {
            iTween.Init(_visual.Parent.gameObject);
            iTween.Stop(_visual.Parent.gameObject);
            _visual.Parent.gameObject.transform.localScale = new Vector3(1, 1, 1);

            yield return new WaitForSeconds(0.01f);

            iTween.PunchScale(_visual.Parent.gameObject,
                new Vector3(PunchScaleAmount, PunchScaleAmount, PunchScaleAmount),
                PunchScaleTime);

            yield return null;
        }

        public override void Awake()
        {
            base.Awake();
        }

        public override void Update()
        {
            base.Update();
        }


        #region Enter

        public override ReturnType GetMoveResults(
            Move move, 
            out IEnumerable<MoveResult> result,
            bool isRecursiveCall = false)
            //bool lockResults = true)
        {
            result = null;
            return ReturnType.Failed;
        }

        #endregion

        #region Exit


        // TODO Entered, Moved
        //public override bool GetExitResult(
        //    Move move,
        //    out ExitResult result,
        //    out IEnumerable<MoveResult> moveResults)
        //{
        //    //moveResults = new MoveResult[0];
        //    //result = new ExitResult
        //    //{
        //    //    Step = Transform.forward.ToVector3Int(),
        //    //    Position = _gridPosition,
        //    //};

        //    //result.Destination = _gridPosition + result.Step;
        //    //moveResults = new MoveResult[0];

        //    //if (LevelSession.Instance.Get(
        //    //    move.Position,
        //    //    out result.Moved))
        //    //{
        //    //    if (result.Moved.GetEnterResults(
        //    //        new Move
        //    //        {
        //    //            Type = MoveType.Moving,
        //    //            Entered = null, 
        //    //            Position = move.Position,
        //    //        },
        //    //        out EnterResult enterResult,
        //    //        out moveResults))
        //    //    { 


        //    //    }
        //    //}

        //    //return true;
        //}


        public override void ExitVisitor(BaseObject source)
        {
            base.ExitVisitor(source);

            StartCoroutine(PunchScaleCoroutine());
            //source.OnExited();
        }

        #endregion

        #region Enter

        public override ReturnType GetEnterResults(
            Move move,
            out EnterResult enterResult,
            out IEnumerable<MoveResult> moveResults)
        {
            if (base.GetEnterResults(
                move,
                out enterResult,
                out moveResults) > 0)
            {
                enterResult.Entered = null;
                moveResults = new MoveResult[0];

                if (LevelSession
                         .Instance
                         .GetOtherPortal(
                         this,
                         out Portal otherPortal))
                {
                    enterResult.Step = otherPortal.Transform.forward.ToVector3Int();
                    enterResult.MoveType = MoveType.UsingPortal;
                    enterResult.Position = otherPortal._levelPosition;
                    enterResult.Destination = enterResult.Position + enterResult.Step;
                    enterResult.Offset = Vector3.up * Level.CellSize / 2;
                    enterResult.Scale = EnterScale;

                    if (LevelSession.Instance.Get(
                        enterResult.Destination,
                        out enterResult.Moved))
                    {
                        if (enterResult.Moved.GetMoveResults(
                          new Move
                          {
                              Type = MoveType.Moving,
                              Entered = null,
                              Position = enterResult.Destination,
                              Step = enterResult.Step,
                              User = enterResult.Moved                              
                          },
                          out moveResults,
                          isRecursiveCall:true) > 0)
                        {
                            return ReturnType.Succeeded_Next;
                        }
                        else if (enterResult.Moved.GetEnterResults(
                           new Move
                           {
                               Type = MoveType.Moving,
                               Entered = null,
                               Position = enterResult.Position,
                               Step = enterResult.Step,
                               User = move.User,
                               Source = move.Source
                               
                           },
                          out EnterResult nextEnterResult,
                          out moveResults) > 0)
                        {
                            enterResult.Position = nextEnterResult.Position;
                            enterResult.Step = nextEnterResult.Step;
                            enterResult.Destination = nextEnterResult.Position + nextEnterResult.Step;
                            enterResult.Entered = nextEnterResult.Entered;
                            enterResult.Moved = nextEnterResult.Moved;
                            return ReturnType.Succeeded_Next;
                        }
                    }
                    else return ReturnType.Succeeded_Next;
                }
            }

            return ReturnType.Failed; ;
        }

        public override void Enter(BaseObject entered)
        {
            if (entered != null)
            {
                entered.EnterVisitor(this);
            }

            return;
        }

        public override void EnterVisitor(BaseObject visitor)
        {
            switch (visitor.Type)
            {
                case ObjectType.Gem:
                    iTween.Init(Transform.gameObject);
                    iTween.Stop(Transform.gameObject);
                    StartCoroutine(PunchScaleCoroutine());

                    //_visual.Parent.transform.localScale = new Vector3(1, 1, 1);
                    //StartCoroutine(PunchScale());
                    //source._targetScale = 0;

                    return;
                default:
                    return;
            }
        }


        #endregion

    }
}