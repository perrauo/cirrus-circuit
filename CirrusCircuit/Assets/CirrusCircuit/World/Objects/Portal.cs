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

        [SerializeField]
        private float _punchScaleAmount = 1f;

        [SerializeField]
        private float _punchScaleTime = 1f;

        IEnumerator PunchScaleCoroutine()
        {
            iTween.Init(_visual.Parent.gameObject);
            iTween.Stop(_visual.Parent.gameObject);
            _visual.Parent.gameObject.transform.localScale = new Vector3(1, 1, 1);

            yield return new WaitForSeconds(0.01f);

            iTween.PunchScale(_visual.Parent.gameObject,
                new Vector3(_punchScaleAmount, _punchScaleAmount, _punchScaleAmount),
                _punchScaleTime);

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


        public override bool Move(Vector3Int step, BaseObject source = null)
        {
            return false;
        }


        public override bool Enter(
            Vector3Int step,
            BaseObject source,
            out Vector3 offset,
            out Vector3Int gridDest,
            out Vector3Int stepDest,
            out BaseObject dest)
        {
            if (base.Enter(
                step, 
                source, 
                out offset, 
                out gridDest,
                out stepDest,
                out dest))
            {
                switch (source.Type)
                {
                    case ObjectType.Gem:
                        StartCoroutine(PunchScaleCoroutine());

                        if(LevelSession
                            .Instance
                            .GetOtherPortal(
                            this, 
                            out Portal other))
                        {
                            other.Exit(
                                source, 
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

        public virtual void Exit(
            BaseObject source, 
            out Vector3Int gridDest,
            out Vector3Int stepDest,
            out BaseObject dest)
        {
            StartCoroutine(PunchScaleCoroutine());

            stepDest = Transform.forward.ToVector3Int();

            gridDest = _gridPosition + stepDest;

            LevelSession.Instance.Get(gridDest, out dest);

            source.OnExited();
        }


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