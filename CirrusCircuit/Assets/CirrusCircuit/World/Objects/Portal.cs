using Cirrus.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit.World.Objects
{
    // NOTE:
    // Potential infinite loop whehn gem exit from the door

    public class Portal : BaseObject
    {
        public override ObjectId Id => ObjectId.Portal;

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


        public override bool TryMove(Vector3Int step, BaseObject incoming = null)
        {
            return false;
        }


        public override bool TryEnter(Vector3Int step, ref Vector3 offset, BaseObject incoming = null)
        {
            if (base.TryEnter(step, ref offset, incoming))
            {
                //_user = incoming;

                switch (incoming.Id)
                {
                    case ObjectId.Gem:
                        StartCoroutine(PunchScaleCoroutine());

                        if(LevelSession.Instance.TryGetOtherPortal(this, out Portal other))
                        {
                            //other.TryExitFrom(incoming);
                        }

                        incoming._targetScale = 0;
                        offset += Vector3.up * Level.CellSize / 2;

                        return true;

                    case ObjectId.Character:
                        return false;
                    default:
                        return false;
                }
            }

            return false;
        }

        public void TryExitFrom(BaseObject incoming)
        {
            if (incoming == null) return;

            StartCoroutine(PunchScaleCoroutine());

            Vector3Int step = Transform.forward.ToVector3Int();
            incoming.TryExit(this, _gridPosition, step);
        }


        public override void Cmd_TryFall()
        {

        }

        public override void Cmd_TryFallThrough(Vector3Int step)
        {

        }

        public override void Local_TryFall()
        {

        }

        public override void Accept(BaseObject incoming)
        {
            switch (incoming.Id)
            {
                case ObjectId.Gem:
                    iTween.Init(Transform.gameObject);
                    iTween.Stop(Transform.gameObject);

                    //_visual.Parent.transform.localScale = new Vector3(1, 1, 1);
                    //StartCoroutine(PunchScale());
                    //incoming._targetScale = 0;

                    return;
                default:
                    return;
            }
        }
    }
}