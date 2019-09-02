using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Gembalaya.Objects
{
    public class Door : BaseObject
    {
        private float _punchScaleAmount = 0.5f;

        private float _punchScaleTime = 0.5f;

        IEnumerator PunchScale()
        {
            iTween.Stop(gameObject);

            transform.localScale = new Vector3(1, 1, 1);

            yield return new WaitForSeconds(0.01f);

            iTween.PunchScale(gameObject,
                new Vector3(_punchScaleAmount, _punchScaleAmount, _punchScaleAmount),
                _punchScaleTime);
        }

        public override bool TryMove(Vector3 step, BaseObject incoming = null)
        {

            switch (incoming.Id)
            {
                case ObjectId.Gem:
                    iTween.Init(gameObject);
                    iTween.Stop(gameObject);
                    transform.localScale = new Vector3(1, 1, 1);
                    StartCoroutine(PunchScale());
                    return true;

                case ObjectId.Character:
                    return false;
                default:
                    return false;

            }

        }

        public override bool Accept(BaseObject incoming)
        {
            incoming._targetScale = 0;
            incoming.Enter();
            return true;
        }

    }
}