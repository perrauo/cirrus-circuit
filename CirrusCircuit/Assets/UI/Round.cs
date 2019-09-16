using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit.UI
{
    public class Round : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Text _text;

        private int _number = 0;

        public int Number {
            get {
                return _number;
            }

            set
            {
                gameObject.SetActive(true);
                _number = value;
                _text.text = "Round " + (_number+1).ToString();
                
                _timer.Start();
            }

        }

        private float _time = 2f;

        private Circuit.Timer _timer;

        private bool _init = false;

        public void OnEnable()
        {
            if (!_init)
            {
                _init = true;

                _timer = new Circuit.Timer(_time, start: false, repeat: false);
                _timer.OnTimeLimitHandler += OnTimeOut;
            }
        }

        public void OnTimeOut()
        {
            gameObject.SetActive(false);
        }


    }
}