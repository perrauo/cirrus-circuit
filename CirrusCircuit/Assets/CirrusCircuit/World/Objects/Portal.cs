using Cirrus.Utils;
using System.Collections;
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


        public override bool Move(Move move)
        {
            return false;
        }


        #region Exit

        public override bool GetExitValue(
            Move move,
            out MoveResult result)
        {
            result = new MoveResult();
            LevelSession.Instance.Get(move.Position, out result.Moved);

            return true;
        }


        public override void Exit(BaseObject source)
        {
            StartCoroutine(PunchScaleCoroutine());
            source.OnExited();
        }

        #endregion

        #region Enter

        public override bool GetEnterResult(
            Move move,
            out MoveResult result)                                    
        {
            if (base.GetEnterResult(
                move,
                out result))
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

                            other.GetExitValue(move, out MoveResult exitResult);

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

        public override void Enter(
            Move move, 
            MoveResult result)
        {
            StartCoroutine(PunchScaleCoroutine());

            if (LevelSession
                .Instance
                .GetOtherPortal(
                    this,
                    out Portal other))
            {
                other.GetExitValue(
                    move,
                    out result
                    );
            }
        }


        #endregion


        public override void Cmd_Fall()
        {

        }

        public void OnDrawGizmos()
        {
            
            GraphicsUtils.DrawLine(
                Transform.position, 
                Transform.position + Transform.forward,
                2f);
        }

        public override void Cmd_FallThrough(Vector3Int step)
        {

        }

        public override void Fall()
        {

        }

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