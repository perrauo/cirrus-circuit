using UnityEngine;
using System.Collections;
using System;

namespace Cirrus.Circuit.UI
{

    public class Timer : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Text _text;

        private Round _round;

        [SerializeField]
        private Game _game;


        public float Time
        {
            set
            {
                var span = new TimeSpan(0, 0, (int)value); //Or TimeSpan.FromSeconds(seconds); (see Jakob C´s answer)
                _text.text = string.Format(span.ToString(@"mm\:ss"));
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

        public void OnValidate()
        {
            if(_game == null)
                _game = FindObjectOfType<Game>();
        }

        public void Awake()
        {
            _game.OnNewRoundHandler += OnNewRound;
            _game.OnNewRoundHandler += OnRound;
            //_on
        }

        public void Update()
        {
            if (_round != null)
                Time = _round.Time;
        }

        public void OnNewRound(Round round)
        {
            _round = round;
            _round.OnIntermissionHandler += OnIntermission;
        }

        public void OnRound(Round round)
        {
            Enabled = true;
        }

        public void OnIntermission(int count)
        {
            Enabled = true;
        }



    }
}
