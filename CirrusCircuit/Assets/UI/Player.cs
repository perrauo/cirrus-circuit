using UnityEngine;
using System.Collections;
using System;

namespace Cirrus.Circuit.UI
{
    public class Player : MonoBehaviour
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
        private UnityEngine.UI.Image _playerImage;
   
        [SerializeField]
        private UnityEngine.UI.Text _playerText;

        [SerializeField]
        private UnityEngine.UI.Image _scoreImage;

        [SerializeField]
        private UnityEngine.UI.Text _scoreText;

        [SerializeField]
        private float _scorePunchScaleAmount = 0.5f;

        [SerializeField]
        private float _scorePunchScaleTime = 1f;

        private Color _color;

        // TODO rename
        [SerializeField]
        private float _gemsColorMult = 1.5f;

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
                _playerImage.color = _color;
                _scoreImage.color = new Color(_color.r * _gemsColorMult, _color.g * _gemsColorMult, _color.b * _gemsColorMult);
            }
        }


        public string PlayerName
        {
            get
            {
                return _playerText.text;
            }

            set
            {
                _playerText.text  = value;
            }
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

        IEnumerator PunchValue()
        {
            iTween.Stop(_scoreText.gameObject);
            _scoreText.gameObject.transform.localScale = new Vector3(1, 1, 1);

            yield return new WaitForSeconds(0.01f);

            iTween.PunchScale(_scoreText.gameObject,
                new Vector3(_scorePunchScaleAmount, _scorePunchScaleAmount, _scorePunchScaleAmount),
                _scorePunchScaleTime);
            
            yield return null;
        }

        public void OnScoreChanged(float score)
        {
            _scoreText.text = score.ToString();
            iTween.Init(_scoreText.gameObject); //TODO move elsewhere
            iTween.Stop(_scoreText.gameObject);
            _scoreText.gameObject.transform.localScale = new Vector3(1, 1, 1);
            // TODO reset fade

            StartCoroutine(PunchValue());
        }

        public bool TryChangeState(State state, params object[] args)
        {
            _state = state;

            switch (_state)
            {
                case State.Waiting:
                    Enabled = true;
                    _color.a = _waitingAlpha;
                    Color = _color;
                    _playerText.text = "Press 'A' to join";
                    break;

                case State.Disabled:
                    Enabled = false;
                    break;

                case State.Ready:
                              
                    int playerNumber = (int)args[0];
                    _color = Game.Instance.Lobby.Colors[playerNumber];
                    _color.a = _readyAlpha;
                    Color = _color;

                    _playerText.text = "player " + ( playerNumber +1);
                    break;

                case State.Round:
                    break;
            }

            return true;
        }
    }
}