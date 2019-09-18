using UnityEngine;
using System.Collections;

using Cirrus.Circuit.World.Objects.Characters;

namespace Cirrus.Circuit.UI
{
    public class CharacterSelectSlot : MonoBehaviour
    {
        [SerializeField]
        private World.Objects.Characters.Resources _characterResources;

        private Resource _selected;

        [SerializeField]
        private UnityEngine.UI.Image[] _images;

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
        private GameObject _selection;

        [SerializeField]
        private UnityEngine.UI.Text _statusText;


        [SerializeField]
        private float _speed = 0.5f;

        private Vector3 _startPosition;

        private Vector3 _targetPosition;

        private int _selectedIndex = 0;

        public enum State
        {
            Ready,
            Selecting,
            Closed
        }

        [SerializeField]
        private State _state;

        public void TryChangeState(State target)
        {
            switch (target)
            {
                case State.Closed:
                    _maskRect.gameObject.SetActive(false);
                    _statusText.text = "Press A to join";
                    break;

                case State.Selecting:
                    _maskRect.gameObject.SetActive(true);                    
                    _statusText.text = "";
                    break;

                case State.Ready:
                    _statusText.text = "Ready";
                    break;
            }

            _state = target;
        }

        private void OnValidate()
        {
            if (_rect == null)
                _rect = _selection.GetComponent<RectTransform>();

            if (_characterResources == null)            
                Utils.AssetDatabase.FindObjectOfType<World.Objects.Characters.Resources>();
            

            int i = 0;
            foreach (var res in _characterResources.Characters)
            {
                _images[i].sprite = res.Portrait;
                i++;
            }

            _bound = (_height * _characterResources.Characters.Length) / 2;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Scroll(true);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Scroll(false);
            }
            else if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                TryChangeState(State.Selecting);
            }
        }

        private void Start()
        {
            _startPosition = _rect.localPosition - Vector3.up * _offset;
            TryChangeState(State.Closed);
            Scroll(true);
        }


        public void Scroll(bool up)
        {            
            _selectedIndex = up ? _selectedIndex - 1 : _selectedIndex + 1;
            _selectedIndex = Mathf.Clamp(_selectedIndex, 0, _characterResources.Characters.Length-1);

            _offset = up ? _offset - _height : _offset + _height;
            _offset = Mathf.Clamp(_offset, -_bound, _bound-_height);

            _targetPosition = _startPosition + Vector3.up * _offset;            
        }


        public void FixedUpdate()
        {
            _rect.localPosition = Vector3.Lerp(_rect.localPosition, _targetPosition, _speed);
        }
    }
}
