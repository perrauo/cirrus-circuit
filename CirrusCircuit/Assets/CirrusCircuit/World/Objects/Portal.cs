using Cirrus.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit.World.Objects
{
    // NOTE:
    // Potential infinite loop whehn gem exit from the door

    public class Portal : BaseObject
    {
        public override ObjectType Type => ObjectType.Portal;

        [SerializeField]
        private Number _connection;
        public int Connection => (int) _connection;        

        private const float PunchScaleAmount = 1f;

        private const float PunchScaleTime = 1f;

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

        protected override void Awake()
        {
            base.Awake();
        }

        public override void Update()
        {
            base.Update();
        }


        #region Enter

        public override bool GetMoveResults(Move move, out IEnumerable<MoveResult> result)
        {
            result = null;
            return false;
        }

        #endregion

        #region Exit

        public override bool GetExitResult(
            Move move,
            out ExitResult result)
        {
            result = new ExitResult();
            //LevelSession.Instance.Get(move.Position, out result.Moved);

            return true;
        }


        public override void Exit(BaseObject source)
        {
            StartCoroutine(PunchScaleCoroutine());
            source.OnExited();
        }

        #endregion

        #region Enter

        public override bool GetEnterResults(
            Move move,
            out EnterResult enterResult,
            out IEnumerable<MoveResult> results)                                    
        {
            if (base.GetEnterResults(
                move,
                out enterResult,
                out results))
            {
                switch (move.User.Type)
                {
                    case ObjectType.Gem:

                        if (LevelSession
                            .Instance
                            .GetOtherPortal(
                            this,
                            out Portal other))
                        {
                            move.User.Transform.position =
                                LevelSession.Instance.Level.GridToWorld(
                                    other._gridPosition);

                            other.GetExitResult(move, out ExitResult exitResult);

                            // TODO remove out offset instead
                            exitResult.Offset += Vector3.up * Level.CellSize / 2;
                            return true;
                        }

                        return false;

                    case ObjectType.Character:
                        return false;
                    default:
                        return false;
                }
            }

            return false;
        }

        public override void Enter(BaseObject visitor)
        {
            StartCoroutine(PunchScaleCoroutine());
            return;
        }


        #endregion



        public override void Accept(BaseObject source)
        {
            switch (source.Type)
            {
                case ObjectType.Gem:
                    iTween.Init(Transform.gameObject);
                    iTween.Stop(Transform.gameObject);

                    //_visual.Parent.transform.localScale = new Vector3(1, 1, 1);
                    //StartCoroutine(PunchScale());
                    //source._targetScale = 0;

                    return;
                default:
                    return;
            }
        }
    }
}