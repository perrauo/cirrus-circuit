using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit.UI
{
    public class PlayerDisplay : MonoBehaviour
    {

        public enum State
        {
            Disconnected,
            Waiting,
            Disabled,
            Round
        }

        public State _state;

        [SerializeField]
        private UnityEngine.UI.Image _image;


        [SerializeField]
        private UnityEngine.UI.Text _text;

        private Color _color;


        private const float _disconnectedAlpha = 0.1f;

        private const float _disabledAlpha = 0.5f;

        public Color Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
                _image.color = _color;
            }
        }


        public string Text
        {
            get
            {
                return _text.text;
            }

            set
            {
                _text.text  = value;
            }
        }


        public bool TryChangeState(State state)
        {
            _state = state;

            switch (_state)
            {
                case State.Disconnected:
                    gameObject.SetActive(true);

                    _color.a = _disconnectedAlpha;
                    _image.color = _color;
                    _text.text = "Device disconnected";
                    break;


                case State.Waiting:
                    gameObject.SetActive(true);
  
                    _color.a = _disabledAlpha;
                    _image.color = _color;
                    _text.text = "Press 'A' to join";

                    break;

                case State.Disabled:
                    gameObject.SetActive(false);

                    break;

                case State.Round:
                    break;
            }

            return true;
        }


        public void FixedUpdate()
        {

        }

    }
}