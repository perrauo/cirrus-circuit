using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit.Objects
{
    public class Door : BaseObject
    {
        [SerializeField]
        private float _punchScaleAmount = 1f;

        [SerializeField]
        private float _punchScaleTime = 1f;

        IEnumerator PunchScale()
        {
            iTween.Stop(_visual.Parent.gameObject);
            _visual.Parent.gameObject.transform.localScale = new Vector3(1, 1, 1);

            yield return new WaitForSeconds(0.01f);

            iTween.PunchScale(_visual.Parent.gameObject,
                new Vector3(_punchScaleAmount, _punchScaleAmount, _punchScaleAmount),
                _punchScaleTime);

            yield return null;
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
                        iTween.Init(_visual.Parent.gameObject);
                        iTween.Stop(_visual.Parent.gameObject);

                        _visual.Parent.gameObject.transform.localScale = new Vector3(1, 1, 1);
                        StartCoroutine(PunchScale());

                        incoming._targetScale = 0;
                        offset += Vector3.up * Level.GridSize / 2;

                        return true;

                    case ObjectId.Character:
                        return false;
                    default:
                        return false;
                }
            }

            return false;
        }


        public override bool TryFall(BaseObject incoming = null)
        {
            return false;
        }

        public override void Accept(BaseObject incoming)
        {
            switch (incoming.Id)
            {
                case ObjectId.Gem:
                    iTween.Init(Object);
                    iTween.Stop(Object);
                    
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