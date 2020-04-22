using UnityEngine;
using System.Collections;

using Cirrus.Circuit.World.Objects.Characters;
using Cirrus.Utils;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Cirrus.Circuit.UI
{
    public class CharacterSelectSlot : MonoBehaviour
    {
        [SerializeField]
        private World.Objects.Characters.Resources _characterResources;

        [SerializeField]
        private Image _imageTemplate;

        [SerializeField]
        private List<Image> _images;

        [SerializeField]
        private RectTransform _rect;

        [SerializeField]
        private RectTransform _maskRect;

        [SerializeField]
        private float _offset = -512;

        [SerializeField]
        private float _height = 256;

        [SerializeField]
        private float _bound = 0;

        [SerializeField]
        private CharacterSelect _characterSelect;

        [SerializeField]
        private GameObject _selection;

        [SerializeField]
        private Text _statusText;

        [SerializeField]
        private Text _up;

        [SerializeField]
        private Text _down;

        [SerializeField]
        private float _speed = 0.5f;

        [SerializeField]
        private float _selectPunchScale = 0.5f;

        [SerializeField]
        private float _selectPunchScaleTime = 1f;

        private Vector3 _startPosition;

        private Vector3 _targetPosition;

        [SerializeField]
        private Transform _characterSpotlightAnchor;

        [SerializeField]
        private float _characterSpotlightSize = 10f;

        [SerializeField]
        private float _characterSpotlightRotateSpeed = 20f;


        [SerializeField]
        private int _selectedIndex = 0;

        [SerializeField]
        private float _disabledArrowAlpha = 0.35f;

        public enum State
        {
            Ready,
            Selecting,
            Closed
        }

        [SerializeField]
        private State _state;

        [SerializeField]
        private CameraManager _camera;


        public void TryChangeState(State target)
        {
            switch (target)
            {
                case State.Closed:
                    _up.gameObject.SetActive(false);
                    _down.gameObject.SetActive(false);
                    _maskRect.gameObject.SetActive(false);
                    _statusText.text = "Press A to join";
                    break;

                case State.Selecting:
                    if (_state == State.Ready) _characterSelect._readyCount--;                
                    if (_characterSpotlightAnchor.transform.childCount != 0)
                        Destroy(_characterSpotlightAnchor.GetChild(0).gameObject);

                    _up.gameObject.SetActive(true);
                    _down.gameObject.SetActive(true);
                    _maskRect.gameObject.SetActive(true);                    
                    _statusText.text = "";
                    break;

                case State.Ready:
                    _characterSelect._readyCount++;

                    Vector3 position =
                    CameraManager.Instance.Camera.ScreenToWorldPoint(
                        _characterSpotlightAnchor.position);

                    // TODO maybe select locked character??
                    CharacterAsset resource = _characterResources.Characters[_selectedIndex];
                    Character character =
                        resource.Create(
                            position,
                            _characterSpotlightAnchor,
                            Quaternion.Euler(new Vector3(0, 180, 0)));

                    character.transform.localScale = new Vector3(
                        _characterSpotlightSize,
                        _characterSpotlightSize,
                        _characterSpotlightSize);

                    _up.gameObject.SetActive(false);
                    _down.gameObject.SetActive(false);
                    _maskRect.gameObject.SetActive(false);
                    _statusText.text = "Ready";
                    break;
            }

            _state = target;
        }

        private void OnValidate()
        {
            if (_characterSelect == null)
                _characterSelect = GetComponentInParent<CharacterSelect>();
            if (_camera == null) _camera = FindObjectOfType<CameraManager>();
            if (_rect == null) _rect = _selection.GetComponent<RectTransform>();


#if UNITY_EDITOR
            if (_characterResources == null)            
                Editor.AssetDatabase.FindObjectOfType<World.Objects.Characters.Resources>();
#endif
      
        }

        public void Awake()
        {
            if (_imageTemplate == null) DebugUtils.Assert(false, "Portrait template is null");
            else _imageTemplate.gameObject.SetActive(true);

            _images = new List<Image>();
            foreach (var res in _characterResources.Characters)
            {
                if (res == null) continue;

                var portrait = _imageTemplate.gameObject.Create(_imageTemplate.transform.parent)?.GetComponent<Image>();
                if (portrait != null)
                {
                    portrait.sprite = res.Portrait;
                    _images.Add(portrait);                    
                }
            }

            if (_imageTemplate != null)
            {
                _imageTemplate.gameObject.SetActive(false);
                _height = _imageTemplate.transform.parent.GetComponent<RectTransform>().rect.height / _images.Count;
            }
            _bound = (_height * _images.Count)/2;
        }

        private void Start()
        {
            _startPosition = _rect.localPosition - Vector3.up * _offset;

            TryChangeState(State.Closed);

            Scroll(true);
        }

        public IEnumerator PunchScale(bool previous)
        {
            iTween.Stop(_up.gameObject);
            iTween.Stop(_down.gameObject);

            _up.transform.localScale = new Vector3(1, 1, 1);
            _down.transform.localScale = new Vector3(1, 1, 1);

            yield return new WaitForSeconds(0.01f);

            if (previous)
            {

                iTween.PunchScale(
                    _up.gameObject,
                    new Vector3(_selectPunchScale,
                    _selectPunchScale, 0),
                    _selectPunchScaleTime);
            }
            else
            {
                iTween.PunchScale(
                    _down.gameObject,
                    new Vector3(_selectPunchScale,
                        _selectPunchScale, 0),
                        _selectPunchScaleTime);
            }
        }

        public void Scroll(bool up)
        {            
            _selectedIndex = up ? _selectedIndex - 1 : _selectedIndex + 1;
            _selectedIndex = Mathf.Clamp(_selectedIndex, 0, _characterResources.Characters.Length-1);
            _offset = up ? _offset - _height : _offset + _height;
            _offset = Mathf.Clamp(_offset, -_bound, _bound - _height);
            _targetPosition = _startPosition + Vector3.up * _offset;

            if (_selectedIndex == 0)
            {
                _up.color = _up.color.SetA(_disabledArrowAlpha);
                if (!up) StartCoroutine(PunchScale(false));
            }
            else if (_selectedIndex == _characterResources.Characters.Length - 1)
            {
                if (up) StartCoroutine(PunchScale(true));      
                _down.color = _down.color.SetA(_disabledArrowAlpha);
            }
            else
            {
                _up.color = _up.color.SetA(1f);
                _down.color = _down.color.SetA(1f);

                if (up) StartCoroutine(PunchScale(true));
                else StartCoroutine(PunchScale(false));
            }
        }

        public void HandleAction0()
        {
            switch (_state)
            {
                case State.Selecting:
                    break;

                case State.Ready:
                    _characterSelect.TryChangeState(CharacterSelect.State.Select);
                    TryChangeState(State.Selecting);
                    break;

                case State.Closed:
                    break;
            }
        }

        public void HandleAction1(params object[] args)
        {
            switch (_state)
            {
                case State.Selecting:
                    Controls.Player ctrl = (Controls.Player) args[0];
                    ctrl._characterResource = _characterResources.Characters[_selectedIndex];
                    TryChangeState(State.Ready);
                    break;                    

                case State.Ready:
                    _characterSelect.TryChangeState(CharacterSelect.State.Ready);
                    break;

                case State.Closed:
                    break;
            }            
        }      

        public void FixedUpdate()
        {
            _rect.localPosition = Vector3.Lerp(_rect.localPosition, _targetPosition, _speed);

            if(_characterSpotlightAnchor.childCount != 0)
                _characterSpotlightAnchor.GetChild(0)
                    .Rotate(Vector3.up * Time.deltaTime * _characterSpotlightRotateSpeed);

        }
    }
}
