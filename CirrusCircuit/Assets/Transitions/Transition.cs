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
        private Canvas _canvas;

        [SerializeField]
        private RectTransform _canvasRectTransform;

        [SerializeField]
        private Game _game;

        private Vector3 _startPosition;

        public void OnValidate()
        {
            if(_game == null)
            _game = FindObjectOfType<Game>();

            if (_canvas == null)
                _canvas = GetComponentInParent<Canvas>();

            if (_canvasRectTransform == null)
                _canvasRectTransform = _canvas.GetComponent<RectTransform>();
        }

        public void Awake()
        {
            _startPosition = _image.transform.position;
            _distanceAway = _canvasRectTransform.rect.height;
            //_canvas.
            //_distanceAway = _canvas.rec
        }

        public void Start()
        {
            //StartCoroutine(DoStart());
        }

        private bool _enabled = false;

        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                _enabled = value;
                transform.GetChild(0).gameObject.SetActive(_enabled);
            }
        }

        public IEnumerator DoTransition()
        {
            Enabled = true;

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
            Enabled = true;

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
