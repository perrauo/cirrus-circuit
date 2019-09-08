using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit.UI
{

    public class Seesaw : MonoBehaviour
    {
        [SerializeField]
        private float _seesawTime = 2f;

        [SerializeField]
        private float _seesawDelay = 2f;

        [SerializeField]
        private float _seesawAngle = 0.1f;

        private float direction = 1;

        // Update is called once per frame
        void Start()
        {
            StartCoroutine(DoSeesaw(_seesawAngle));
        }

        public IEnumerator DoSeesaw(float angle)
        {
            iTween.RotateBy(gameObject, new Vector3(0, 0, angle), _seesawTime);

            yield return new WaitForSeconds(_seesawDelay);

            direction *= -1;
            StartCoroutine(DoSeesaw(direction * _seesawAngle * 2));

            yield return null;
        }

    }

}
