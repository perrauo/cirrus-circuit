using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit.Transitions
{
    public class Transition : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Image _image;

        [SerializeField]
        private float _transitionDownTime = 2f;

        [SerializeField]
        private float _transitionUpTime = 2f;

        [SerializeField]
        private float _transitionHeldTime = 2f;

        [SerializeField]
        private float _anticipationTime = 0.5f;

        [SerializeField]
        private float _anticipationHeldTime = 0.5f;

        [SerializeField]
        private float _distanceAway = 200f;

        [SerializeField]
        private float _distanceDown = -10;

        [SerializeField]
        private Game _game;

        private Vector3 _startPosition;

        public void OnValidate()
        {
            if(_game == null)
            _game = FindObjectOfType<Game>();
        }


        public void Awake()
        {
            _startPosition = _image.transform.position;
        }

        public void Start()
        {
            StartCoroutine(DoStart());
        }

        public IEnumerator DoTransition()
        {
            _image.transform.position = _startPosition + Vector3.up * _distanceAway;

            iTween.MoveTo(_image.gameObject, _startPosition, _transitionDownTime);

            yield return new WaitForSeconds(_transitionDownTime);
            
            yield return new WaitForSeconds(_transitionHeldTime);

            iTween.MoveTo(_image.gameObject, _startPosition + Vector3.up * _distanceDown, _anticipationTime);

            yield return new WaitForSeconds(_anticipationHeldTime);

            iTween.MoveTo(_image.gameObject, _startPosition + Vector3.up * _distanceAway, _transitionUpTime);

            yield return null;
        }

        public IEnumerator DoStart()
        {
            yield return new WaitForSeconds(_transitionHeldTime);

            iTween.MoveTo(_image.gameObject, _startPosition + Vector3.up * _distanceDown, _anticipationTime);

            yield return new WaitForSeconds(_anticipationHeldTime);

            iTween.MoveTo(_image.gameObject, _startPosition + Vector3.up * _distanceAway, _transitionUpTime);

            yield return null;


        }

        public void Perform()
        {
            StartCoroutine(DoTransition());
        }



    }
}
