using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit.UI
{
    public class PlayerDisplay : MonoBehaviour
    {

        public enum State
        {
            Waiting,
            Disabled,
            Round,
            Ready
        }

        public State _state;

        [SerializeField]
        private UnityEngine.UI.Image _image;


        [SerializeField]
        private UnityEngine.UI.Text _text;

        private Color _color;


        private const float _waitingAlpha = 0.5f;

        private const float _readyAlpha = 1f;

        [SerializeField]
        private Color _colorWaiting = Color.gray;

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


        public bool TryChangeState(State state, params object[] args)
        {
            _state = state;

            switch (_state)
            {
                case State.Waiting:

                    gameObject.SetActive(true);

                    _color.a = _waitingAlpha;
                    _image.color = _colorWaiting;
                    _text.text = "Press 'A' to join";

                    break;

                case State.Disabled:
                    gameObject.SetActive(false);

                    break;


                case State.Ready:
                              
                    int playerNumber = (int)args[0];
                    _color = Game.Instance.Lobby.Colors[playerNumber];
                    _color.a = _readyAlpha;
                    this.Color = _color;

                    _text.text = "Ready player " + ( playerNumber +1);
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