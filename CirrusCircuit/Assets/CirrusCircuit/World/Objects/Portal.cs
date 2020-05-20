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


        public override bool Move(
            BaseObject source, 
            Vector3Int step)
        {
            return false;
        }


        #region Exit

        public override bool GetExitValue(
            BaseObject source,
            Vector3Int step,
            out Vector3 offset,
            out Vector3Int gridDest,
            out Vector3Int stepDest,
            out BaseObject dest)
        {
            offset = Vector3.zero;
            stepDest = Transform.forward.ToVector3Int();
            gridDest = _gridPosition + stepDest;
            LevelSession.Instance.Get(gridDest, out dest);

            return true;
        }


        public override void Exit(BaseObject source)
        {
            StartCoroutine(PunchScaleCoroutine());
            source.OnExited();
        }

        #endregion

        #region Enter

        public override bool GetEnterValues(
            BaseObject source, 
            Vector3Int step, 
            out Vector3 offset, 
            out Vector3Int gridDest, 
            out Vector3Int stepDest, 
            out BaseObject dest)
        {
            if (base.GetEnterValues(
                source,
                step,                
                out offset,
                out gridDest,
                out stepDest,
                out dest))
            {
                switch (source.Type)
                {
                    case ObjectType.Gem:

                        if (LevelSession
                            .Instance
                            .GetOtherPortal(
                            this,
                            out Portal other))
                        {
                            source.Transform.position =
                                LevelSession.Instance.Level.GridToWorld(
                                    other._gridPosition);

                            other.GetExitValue(
                                source,
                                step,
                                out offset,
                                out gridDest,
                                out stepDest,
                                out dest);

                            offset += Vector3.up * Level.CellSize / 2;
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
            BaseObject source,
            Vector3Int gridDest,
            Vector3Int step)            
        {
            StartCoroutine(PunchScaleCoroutine());

            if (LevelSession
                .Instance
                .GetOtherPortal(
                    this,
                    out Portal other))
            {
                other.Exit(source);
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