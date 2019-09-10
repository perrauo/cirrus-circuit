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
        private UnityEngine.UI.Image _playerBack;
   
        [SerializeField]
        private UnityEngine.UI.Text _playerText;


        [SerializeField]
        private UnityEngine.UI.Image _gemsBack;

        [SerializeField]
        private UnityEngine.UI.Text _gemsText;

        private Color _color;

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
                _playerBack.color = _color;
                _gemsBack.color = new Color(_color.r * _gemsColorMult, _color.g * _gemsColorMult, _color.b * _gemsColorMult);
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

        public void OnGemCountChanged(int count)
        {


        }
        
        public bool TryChangeState(State state, params object[] args)
        {
            _state = state;

            switch (_state)
            {
                case State.Waiting:

                    gameObject.SetActive(true);

                    _color.a = _waitingAlpha;
                    this.Color = _color;
                    _playerText.text = "Press 'A' to join";

                    break;

                case State.Disabled:
                    gameObject.SetActive(false);

                    break;


                case State.Ready:
                              
                    int playerNumber = (int)args[0];
                    _color = Game.Instance.Lobby.Colors[playerNumber];
                    _color.a = _readyAlpha;
                    this.Color = _color;

                    _playerText.text = "player " + ( playerNumber +1);
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